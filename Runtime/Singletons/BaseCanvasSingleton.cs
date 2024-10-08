﻿using System;
using System.Threading.Tasks;
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
                if(transform.childCount == 0)
                    new GameObject().transform.SetParent(transform);
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
    public static void Open() => Open(null);
    public static void Close() => Close(null);
    public static void Open(Action callback) => Instance.OpenInstance(callback); 
    public static void Close(Action callback) => Instance.CloseInstance(callback);

    public static async void CloseAsync()
    {
        bool ready = false;
        void SetReady() => ready = true;
        Instance.CloseInstance(SetReady);
        while (!ready)
            await Task.Yield();
    }
    
    public static async void OpenAsync()
    {
        bool ready = false;
        void SetReady() => ready = true;
        Instance.OpenInstance(SetReady);
        while (!ready)
            await Task.Yield();
    }
    
    public static void Show() => Instance.ShowInstance();
    public static void Hide() => Instance.HideInstance();

    

    protected virtual void ShowInstance() => mainContainer.Show();

    protected virtual void HideInstance() => mainContainer.Hide();
    
    protected virtual void OpenInstance(Action callback)
    {
        gameObject.SetActive(true);
        mainContainer.Open(callback);
    }

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