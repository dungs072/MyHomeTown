using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ActivityElement : MonoBehaviour
{
    [SerializeField] private TMP_Text contentText;
    private Image backgroundImage;
    private RectTransform rectTransform;
    void Awake()
    {
        backgroundImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }


    public void SetContent(string content)
    {
        contentText.text = content;
    }
    public void SetRectPosition(Vector3 position)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        rectTransform.position = position;
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
        var fadeInAnim = backgroundImage.DOFade(1f, 1f);
        yield return fadeInAnim;
    }

    public IEnumerator FadeOut()
    {
        var fadeOutAnim = backgroundImage.DOFade(0f, 1f);
        yield return fadeOutAnim;
    }

}
