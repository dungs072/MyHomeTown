using UnityEngine;

public class ManagerSingleton : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    private static ManagerSingleton instance;
    public static ManagerSingleton Instance => instance;

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
