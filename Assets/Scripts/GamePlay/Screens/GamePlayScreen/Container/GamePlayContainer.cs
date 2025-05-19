
using GamePlayContainerElements;
using UnityEngine;

public class GamePlayContainer : MonoBehaviour
{
    [SerializeField] private Header header;
    [SerializeField] private Footer footer;

    public Footer Footer => footer;
    public Header Header => header;
}
