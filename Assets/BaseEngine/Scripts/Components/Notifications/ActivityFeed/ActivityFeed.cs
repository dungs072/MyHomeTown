using System.Collections.Generic;
using UnityEngine;

public class ActivityFeed : MonoBehaviour
{
    [SerializeField] private ActivityElement activityElementPrefab;
    [SerializeField] private float elementSpacing = 10f;
    private readonly List<ActivityElement> activityElementsPool = new();
    private readonly List<ActivityElement> activeElements = new();
    private RectTransform elementTransform;
    void Awake()
    {
        elementTransform = activityElementPrefab.GetComponent<RectTransform>();
    }
    public void AddActivity(string content)
    {
        var element = GetFreeActivityElement();
        activeElements.Add(element);
        element.SetContent(content);
        element.SetOriginalPosition(GetNextPosition());
        element.PrepareShowing();
        StartCoroutine(element.ShowAsync(() =>
        {
            activeElements.Remove(element);
            element.gameObject.SetActive(false);
            HandleWhenElementRemoved(element);
        }));
    }
    private void HandleWhenElementRemoved(ActivityElement element)
    {
        for (int i = 0; i < activeElements.Count; i++)
        {
            var activeElement = activeElements[i];
            var currentPosition = GetCurrentPosition(i);
            StartCoroutine(activeElement.MoveToAsync(currentPosition));
        }
    }
    private Vector3 GetNextPosition()
    {
        var index = activeElements.Count - 1;
        return GetCurrentPosition(index);
    }
    private Vector3 GetCurrentPosition(int index)
    {
        var basePosition = Vector3.zero;
        if (index <= 0) return basePosition;
        var elementHeight = elementTransform.rect.height;
        var position = new Vector3(
                basePosition.x,
                basePosition.y - (elementHeight + elementSpacing) * index,
                basePosition.z
        );

        return basePosition + position;
    }

    private ActivityElement GetFreeActivityElement()
    {
        foreach (var element in activityElementsPool)
        {
            if (element.gameObject.activeSelf) continue;
            element.gameObject.SetActive(true);
            return element;
        }
        var newElement = Instantiate(activityElementPrefab, transform);
        activityElementsPool.Add(newElement);
        return newElement;
    }
}
