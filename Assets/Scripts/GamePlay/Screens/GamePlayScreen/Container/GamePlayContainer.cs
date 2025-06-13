
using GamePlayContainerElements;
using UnityEngine;

public class GamePlayContainer : MonoBehaviour
{
    [SerializeField] private Header header;
    [SerializeField] private Footer footer;

    [Header("Middle Elements")]
    [SerializeField] private InfoPanel infoPanel;

    public Footer Footer => footer;
    public Header Header => header;
    public InfoPanel InfoPanel => infoPanel;
}
