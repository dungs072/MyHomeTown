using UnityEngine;

public class ManagerSingleton : MonoBehaviour
{
    [SerializeField] private ActionManager actionManager;
    [SerializeField] private GridSystem gridSystem;
    private static ManagerSingleton instance;
    public static ManagerSingleton Instance => instance;

    public ActionManager ActionManager => actionManager;
    public GridSystem GridSystem => gridSystem;
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
