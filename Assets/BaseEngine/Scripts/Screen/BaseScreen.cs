using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for all screens in the game. This class is responsible for setting up the screen's parent to be the canvas.
/// It also provides a reference to the canvas for derived classes to use.
/// </summary>

namespace BaseEngine
{
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
            DestroyScreen();
        }
        public virtual void InitScreen()
        {

        }
        public virtual void DestroyScreen()
        {

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

}
