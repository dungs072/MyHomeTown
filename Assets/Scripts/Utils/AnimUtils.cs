using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public static class AnimUtils
{
    public static Tweener PlayNumberCounterAnim(TMP_Text text, int from, int to, float duration = 0.5f)
    {
        return DOTween.To(() => from, x =>
        {
            from = x;
            text.text = from.ToString();
        }, to, duration);
    }
    public static IEnumerator PlayNumberCounterAnimAsync(TMP_Text text, int from, int to, float duration = 0.5f)
    {
        var tween = PlayNumberCounterAnim(text, from, to, duration);
        yield return tween.WaitForCompletion();
    }
}