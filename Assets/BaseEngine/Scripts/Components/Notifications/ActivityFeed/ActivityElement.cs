using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ActivityElement : MonoBehaviour
{
    [SerializeField] private TMP_Text contentText;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }
    void OnDisable()
    {
        DOTween.Kill(gameObject);
    }

    public void SetContent(string content)
    {
        contentText.text = content;
    }
    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }
    public void SetRectPosition(Vector3 position)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        rectTransform.anchoredPosition = position;
    }

    public void PrepareShowing()
    {
        canvasGroup.alpha = 0f;
        var hidePosition = originalPosition + new Vector3(100, 0, 0);
        SetRectPosition(hidePosition);
    }

    public IEnumerator ShowAsync(System.Action onComplete = null)
    {
        yield return FadeIn();
        yield return new WaitForSeconds(2f);
        yield return FadeOut();
        onComplete?.Invoke();
    }

    public IEnumerator FadeIn()
    {
        var fadeInAnim = canvasGroup.DOFade(1f, 0.45f);
        var moveAnim = rectTransform.DOAnchorPos(originalPosition, 0.45f);
        yield return DOTween.Sequence().Join(fadeInAnim).Join(moveAnim).WaitForCompletion();
    }

    public IEnumerator FadeOut()
    {
        var fadeOutAnim = canvasGroup.DOFade(0f, 0.34f);
        yield return fadeOutAnim.WaitForCompletion();
    }
    public IEnumerator MoveToAsync(Vector3 targetPosition)
    {
        var moveAnim = rectTransform.DOAnchorPos(targetPosition, 0.45f);
        yield return moveAnim.WaitForCompletion();
    }

}
