using UnityEngine;

public class ManagerSingleton : MonoBehaviour
{
    [SerializeField] private ActionManager actionManager;
    private static ManagerSingleton instance;
    public static ManagerSingleton Instance => instance;

    public ActionManager ActionManager => actionManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
