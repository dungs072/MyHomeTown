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
    protected Transform screenHolder;
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
            screenHolder = transform.parent;
            if (!screenHolder)
            {
                Debug.LogError("Screen holder not found in the parent of the screen object.");
                return;
            }
            return;
        }

        screenHolder = GameObject.FindGameObjectWithTag("ScreenHolder").transform;
        if (!screenHolder)
        {
            Debug.LogError("Screen holder not found in the scene. Please make sure to set the screen holder in the scene.");
            return;
        }
        transform.SetParent(screenHolder, true);
    }


    public virtual IEnumerator OpenScreenAsync()
    {
        // Just override this method in the derived class to implement the screen opening logic.
        gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
    }
    public virtual IEnumerator CloseScreenAsync()
    {
        // Just override this method in the derived class to implement the screen closing logic.
        gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }

}
