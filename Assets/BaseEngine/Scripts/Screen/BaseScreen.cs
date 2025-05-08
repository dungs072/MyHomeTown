using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all screens in the game. This class is responsible for setting up the screen's parent to be the canvas.
/// It also provides a reference to the canvas for derived classes to use.
/// </summary>
public class BaseScreen : MonoBehaviour
{
    public static Action<BaseScreen> OnScreenCreated;
    public static Action<BaseScreen> OnScreenDestroyed;
    protected Canvas canvas;
    private string screenName = string.Empty;

    public string ScreenName => screenName;
    void Awake()
    {
        OnScreenCreated?.Invoke(this);
        InitScreen();
    }
    void OnDestroy()
    {
        OnScreenDestroyed?.Invoke(this);
    }


    private void InitScreen()
    {
        screenName = gameObject.name;
        if (transform.parent)
        {
            canvas = transform.parent.GetComponent<Canvas>();
            if (!canvas)
            {
                Debug.LogError("Canvas not found in the parent of the screen object.");
                return;
            }
            return;
        }

        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            canvas = canvasObj.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Canvas not found in the scene.");
        }

        gameObject.transform.SetParent(canvas.transform, true);
    }


    public virtual void OpenScreen()
    {
        // Just override this method in the derived class to implement the screen opening logic.
        gameObject.SetActive(true);
    }
    public virtual void CloseScreen()
    {
        // Just override this method in the derived class to implement the screen closing logic.
        gameObject.SetActive(false);
    }

}
