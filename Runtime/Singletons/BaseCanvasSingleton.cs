using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseCanvasSingleton<T> : BaseMonoSingleton<T> where T : BaseCanvasSingleton<T>
{
    [SerializeField] private AnimatedContainer mainContainer;
    protected AnimatedContainer MainContainer => mainContainer;

    public bool deactivateOnClose = true;

       
    protected virtual void OnValidate()
    {
        if (mainContainer == null)
        {
            foreach (Transform child in transform)
            {
                mainContainer = child.GetComponent<AnimatedContainer>();
                if (mainContainer)
                    return;
            }

            if (!mainContainer)
            {
                Transform firstChild = transform.GetChild(0);
                if (firstChild)
                    firstChild.gameObject.AddComponent<AnimatedContainer>();
                else
                {
                    firstChild = new GameObject("MainContainer", typeof(AnimatedContainer)).transform;
                    firstChild.transform.SetParent(transform);
                    AnimatedContainer animatedContainer = firstChild.GetComponent<AnimatedContainer>();
                    animatedContainer.Show();
                    animatedContainer.Hide();
                }
            }
        }
    }

    public static void Open(Action callback=null) => Instance.OpenInstance(callback); 
    
    public static void Close(Action callback=null) => Instance.CloseInstance(callback);

    
    public static void Show() => Instance.ShowInstance();
    public static void Hide() => Instance.HideInstance();

    
    [Button("Show"), ButtonGroup("ShowHide")]

    protected virtual void ShowInstance() => mainContainer.Show();
    [Button("Hide"), ButtonGroup("ShowHide")]

    protected virtual void HideInstance() => mainContainer.Hide();
    
    [Button("Open")]
    protected virtual void OpenInstance(Action callback)
    {
        gameObject.SetActive(true);
        mainContainer.Open(callback);
    }

    [Button("Close")]
    protected virtual void CloseInstance(Action callback)
    {
        if (deactivateOnClose)
            mainContainer.Close
                (()=>
                {
                    gameObject.SetActive(false);
                    callback?.Invoke();
                });
        else
            mainContainer.Close();
    }
    
    public void ToggleInstance()
    {
        if (MainContainer.IsClosed)
            MainContainer.Open();
        else
            MainContainer.Close();
    }

    public static void Toggle() => Instance.ToggleInstance();
}