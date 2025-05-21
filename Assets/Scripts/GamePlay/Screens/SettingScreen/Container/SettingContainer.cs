using UnityEngine;
using SettingContainerElements;
public class SettingContainer : MonoBehaviour
{
    [SerializeField] private Middle middle;

    public Middle Middle => middle;
}
