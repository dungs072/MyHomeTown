using UnityEngine;

namespace BaseEngine
{
    [RequireComponent(typeof(RectTransform))]
    public class MagicButton : MonoBehaviour
    {
        [SerializeField] private float increasePercentHitArea = 0.1f;

        private RectTransform rectTransform;

        void Awake()
        {
            InitComponents();
            IncreaseHitArea();
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

    }

}

