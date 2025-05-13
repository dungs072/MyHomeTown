using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaseEngine
{
    public enum ButtonType
    {
        NONE,
        SCALE,
    }
    [RequireComponent(typeof(RectTransform))]
    public class MagicButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Action OnClicked;
        [SerializeField] private float increasePercentHitArea = 0.1f;
        [SerializeField] private bool isClickedOnHitArea = true;
        [SerializeField] private ButtonType buttonType = ButtonType.NONE;

        [Header("Scale button")]
        [SerializeField] private float scaleDownFactor = 0.9f;
        [SerializeField] private float scaleUpFactor = 1f;
        [SerializeField] private float scaleDuration = 0.3f;
        private RectTransform rectTransform;

        void Awake()
        {
            InitComponents();
            IncreaseHitArea();
        }
        public void AddListener(Action action)
        {
            OnClicked += action;
        }
        private void InitComponents()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void IncreaseHitArea()
        {
            if (!rectTransform) return;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.x += sizeDelta.x * increasePercentHitArea;
            sizeDelta.y += sizeDelta.y * increasePercentHitArea;
            rectTransform.sizeDelta = sizeDelta;

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopAllCoroutines();
            transform.DOKill();
            if (buttonType == ButtonType.SCALE)
            {
                transform.DOScale(scaleDownFactor, scaleDuration);
            }
            Debug.Log("Pointer Down");

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Pointer Up");
            StartCoroutine(HandleButtonClicked(eventData));
        }
        private IEnumerator HandleButtonClicked(PointerEventData eventData)
        {
            yield return PlayPointerUpAnim();
            var canClick = true;
            if (isClickedOnHitArea && !IsPointerOverUI(eventData))
            {
                canClick = false;
            }
            if (!canClick) yield break;
            OnClicked?.Invoke();
        }
        private IEnumerator PlayPointerUpAnim()
        {
            if (buttonType == ButtonType.SCALE)
            {
                var scaleDownAnim = transform.DOScale(scaleUpFactor, scaleDuration);
                yield return scaleDownAnim.WaitForCompletion();
            }
        }
        private bool IsPointerOverUI(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            eventData.position,
            eventData.enterEventCamera);
        }
    }

}

