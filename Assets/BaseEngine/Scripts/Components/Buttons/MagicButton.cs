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
    public enum ButtonState
    {
        NONE,
        BUBBLE,
    }
    [RequireComponent(typeof(RectTransform))]
    public class MagicButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Action OnClicked;
        [SerializeField] private float increasePercentHitArea = 0.1f;
        [SerializeField] private bool isClickedOnHitArea = true;
        [SerializeField] private ButtonType buttonType = ButtonType.NONE;
        // for scale button only
        [Header("Scale button")]
        [SerializeField] private float scaleDownFactor = 0.9f;
        [SerializeField] private float scaleUpFactor = 1f;
        [SerializeField] private float scaleDuration = 0.3f;
        [Header("State")]
        [SerializeField] private ButtonState buttonState = ButtonState.NONE;
        // for bubble config only
        [SerializeField] private float bubbleUpFactor = 1.2f;
        [SerializeField] private float bubbleDownFactor = 0.95f;
        [SerializeField] private float bubbleUpDuration = 0.3f;
        [SerializeField] private float bubbleDownDuration = 0.5f;

        private RectTransform rectTransform;
        private Coroutine pointerDownAnim;
        private bool isClicking = false;

        void Awake()
        {
            InitComponents();
            InitButtonState();
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
        private void InitButtonState()
        {
            if (buttonState == ButtonState.BUBBLE)
            {
                rectTransform.DOScale(bubbleUpFactor, bubbleUpDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    rectTransform.DOScale(bubbleDownFactor, bubbleDownDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });
            }
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
            if (isClicking) return;
            isClicking = true;
            StopAllCoroutines();
            transform.DOKill();
            pointerDownAnim = null;
            if (buttonType == ButtonType.SCALE)
            {
                pointerDownAnim = StartCoroutine(PlayPointerDownAnim());
            }

        }
        private IEnumerator PlayPointerDownAnim()
        {
            if (buttonType == ButtonType.SCALE)
            {
                var scaleDownAnim = transform.DOScale(scaleDownFactor, scaleDuration);
                yield return scaleDownAnim.WaitForCompletion();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {

            StartCoroutine(HandleButtonClicked(eventData));
        }
        private IEnumerator HandleButtonClicked(PointerEventData eventData)
        {
            if (pointerDownAnim != null)
            {
                yield return pointerDownAnim;
            }
            yield return PlayPointerUpAnim();
            isClicking = false;
            var canClick = true;
            if (isClickedOnHitArea && !IsPointerOverUI(eventData))
            {
                canClick = false;
            }
            if (!canClick)
            {
                InitButtonState();
                yield break;
            }
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

