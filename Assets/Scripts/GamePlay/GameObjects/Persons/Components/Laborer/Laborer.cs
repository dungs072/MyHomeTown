using System;
using System.Collections;
using UnityEngine;

public class Laborer : MonoBehaviour
{
    private Coroutine _curWorkCoroutine;
    public void DoWork(Doable payload)
    {
        if (_curWorkCoroutine != null)
        {
            StopCoroutine(_curWorkCoroutine);
        }
        _curWorkCoroutine = StartCoroutine(DoWorkCoroutine(payload));
    }
    private IEnumerator DoWorkCoroutine(Doable payload)
    {
        yield return new WaitForSeconds(payload.duration);
        payload.finishedAction?.Invoke();
    }
}