﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

/// <summary>
/// Conteiner generico para animar canvas con 'coroutines'.
/// Si la animacion por movimiento no encaja bien, revisar que el Root del container este bien posicionados
/// </summary>
public class AnimatedContainer : MonoBehaviour
{
    public Action<bool> OnOpenOrClose;
    public event Action OnOpen;
    public event Action OnClose;
    public enum Direction { Right, Left, Up, Down }
    public enum TimeType { Scaled, Unscaled }

    public enum Order { Open, Close, Show, Hide }

    public float durationIn = .3f;

    public float durationOut = .3f;
    private IEnumerator _routine;
    [SerializeField] private RectTransform parent;

    public bool blockRaycasts = true;
    private RectTransform Parent
    {
        get
        {
            if (!parent)
                parent = transform.parent as RectTransform;
            return parent;
        }
    }

    [SerializeField] private RectTransform rectTransform;

    private RectTransform RectTransform
    {
        get
        {
            if (!rectTransform)
                rectTransform = transform as RectTransform;
            return rectTransform;
        }
    }

    public bool deactivateOnHide = true;
    public bool startHidden = true;
    private bool _alreadyStarted = false;

    private bool _isOpen;

    [ShowInInspector] public bool IsOpen 
    {
        get => _isOpen; 
        private set
        {
            _isOpen = value;
            OnOpenOrClose?.Invoke(_isOpen);
            if(_isOpen)     OnOpen?.Invoke();
            else            OnClose?.Invoke();
        }
    }
    [ShowInInspector] public bool IsClosed => !IsOpen;
    public bool InAnimation { get; private set; }

    public TimeType timeType = TimeType.Unscaled;

    [TabGroup("Alpha")] public bool useAlpha = true;
    public CanvasGroup canvasGroup;

    [EnableIf("useAlpha"), TabGroup("Alpha")] public AnimationCurve alphaCurveIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [EnableIf("useAlpha"), TabGroup("Alpha")] public AnimationCurve alphaCurveOut = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [TabGroup("Scale")] public bool useScale = false;
    [EnableIf("useScale"), TabGroup("Scale")] public AnimationCurve curveInScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [EnableIf("useScale"), TabGroup("Scale")] public Vector3 targetScale = new Vector3(1, 0, 1);
    [EnableIf("useScale"), TabGroup("Scale")] public AnimationCurve curveOutScale = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [TabGroup("Movement")] public bool useMovement = false;
    [EnableIf("useMovement"), TabGroup("Movement")] public Direction direction = Direction.Down;
    [EnableIf("useMovement"), TabGroup("Movement")] public AnimationCurve curveIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [EnableIf("useMovement"), TabGroup("Movement")] public AnimationCurve curveOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Vector3 targetPosition;

    private bool initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void OnValidate()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        if (RectTransform == null)
            rectTransform = transform as RectTransform;

        parent = transform.parent as RectTransform;

        if (useMovement)
        {
            SetDirection(direction);
            RectTransform.anchoredPosition = Vector3.zero;
            RectTransform.localScale = Vector3.one;
        }

        if (_alreadyStarted) return;
        if (startHidden)
            Hide();
        else
            Show();

    }


    public void SetDirection(Direction direction, bool overrideCurrentPosition = false)
    {
        switch (direction)
        {
            case Direction.Right:
                targetPosition = Vector3.right * Parent.rect.width;
                break;
            case Direction.Left:
                targetPosition = Vector3.left * Parent.rect.width;
                break;
            case Direction.Up:
                targetPosition = Vector3.up * Parent.rect.height;
                break;
            case Direction.Down:
                targetPosition = Vector3.down * Parent.rect.height;
                break;
            default:
                break;
        }
        if (overrideCurrentPosition)
            rectTransform.anchoredPosition = targetPosition;
    }

    public Vector3 GetTargetPosition()
    {
        switch (direction)
        {
            case Direction.Right:
                return targetPosition = Vector3.right * Parent.rect.width;
            case Direction.Left:
                return targetPosition = Vector3.left * Parent.rect.width;
            case Direction.Up:
                return targetPosition = Vector3.up * Parent.rect.height;
            case Direction.Down:
                return targetPosition = Vector3.down * Parent.rect.height;
            default:
                break;
        }
        return targetPosition = Vector3.zero;
    }


    [Button, ButtonGroup("Animated")]
    public void Close()
    {
        _alreadyStarted = true;
        if (IsOpen)
            Close(null);
    }

    public void Close(Action postAction = null)
    {
        //Debug.Log($"{this} Close");

        if (IsOpen && gameObject.activeSelf && gameObject.activeInHierarchy)
        {
            canvasGroup.interactable = false;
            if (_routine != null) StopCoroutine(_routine);
            _routine = CloseRoutine(postAction);
            StartCoroutine(_routine);
            IsOpen = false;
        }
        else
        {
            postAction?.Invoke();
        }
    }

    [Button, ButtonGroup("Animated")]
    public void Open()
    {
        Open(null);
    }

    public void Open(Action postAction)
    {
        _alreadyStarted = true;
        gameObject.SetActive(true);
        if (!IsOpen)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = OpenRoutine(postAction);
            StartCoroutine(_routine);
            IsOpen = true;
        }
        else
        {
            canvasGroup.blocksRaycasts = blockRaycasts;
            canvasGroup.interactable = true;
            postAction?.Invoke();
        }
    }

    [Button, ButtonGroup("Snap")]
    public void Hide()
    {
        _alreadyStarted = true;

        if (useMovement)
            RectTransform.anchoredPosition = targetPosition;

        if (useScale)
            RectTransform.localScale = targetScale;

        if (useAlpha)
            canvasGroup.alpha = 0;

        IsOpen = false;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (deactivateOnHide)
            gameObject.SetActive(false);
    }

    [Button, ButtonGroup("Snap")]
    public void Show()
    {
        _alreadyStarted = true;
        //Debug.Log($"{this} Show");

        gameObject.SetActive(true);

        if (useMovement) RectTransform.anchoredPosition = Vector3.zero;
        SetValue(1, curveIn, curveInScale, alphaCurveIn);
        IsOpen = true;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = blockRaycasts;
    }

    private IEnumerator CloseRoutine(Action postAction = null)
    {
        InAnimation = true;
        yield return AnimationRutine(durationOut, targetPosition, curveOut, targetScale, curveOutScale, 0, alphaCurveOut,
            () =>
            {
                InAnimation = false;
                Hide();
                postAction?.Invoke();
            });
    }

    private IEnumerator OpenRoutine(Action postAction = null)
    {
        InAnimation = true;
        yield return AnimationRutine(durationIn, Vector3.zero, curveIn, Vector3.one, curveInScale, 1, alphaCurveIn,
            () =>
            {
                InAnimation = false;
                Show();
                postAction?.Invoke();
            });
    }

    private IEnumerator AnimationRutine(float duration,
        Vector3 targetPosition, AnimationCurve movementCurve,
        Vector3 targetScale, AnimationCurve scaleCurve,
        float targetAlpha, AnimationCurve alphaCurve,
        Action postAction)
    {
        float t = 0;
        Vector3 startPosition = RectTransform.anchoredPosition;
        Vector3 startScale = RectTransform.localScale;
        float startAlpha = 1 - targetAlpha;

        Action step = null;

        if (useMovement) step += () => RectTransform.anchoredPosition = Vector3.LerpUnclamped(startPosition, targetPosition, movementCurve.Evaluate(t));
        if (useScale) step += () => RectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleCurve.Evaluate(t));
        if (useAlpha) step += () => canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, alphaCurve.Evaluate(t));

        do
        {
            t += ((timeType == TimeType.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime) / duration;
            step?.Invoke();
            yield return null;
        } while (t < 1);
        postAction?.Invoke();
    }

    private float lastValue = 0;

    [Button]
    internal void SetValue(float currentFill)
    {
        if (lastValue < currentFill)
            SetValue(currentFill, curveIn, curveInScale, alphaCurveIn);
        else
            SetValue(currentFill, curveOut, curveOutScale, alphaCurveOut);

        lastValue = currentFill;
    }

    public void SetValue(float currentFill, AnimationCurve movementCurve, AnimationCurve scaleCurve, AnimationCurve alphaCurveIn)
    {
        Vector3 startPosition = GetTargetPosition();
        if (useMovement) RectTransform.anchoredPosition = Vector3.LerpUnclamped(startPosition, Vector3.zero, movementCurve.Evaluate(currentFill));
        if (useScale) RectTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, scaleCurve.Evaluate(currentFill));
        if (useAlpha) canvasGroup.alpha = Mathf.LerpUnclamped(0, 1, alphaCurveIn.Evaluate(currentFill));
    }

    public void Toggle(bool animated)
    {
        if (animated)
        {
            if (IsOpen)
                Close();
            else
                Open();
        }
        else
        {
            if (IsOpen)
                Hide();
            else
                Show();
        }
    }

    public void Execute(Order order, Action callback = null)
    {
        switch (order)
        {
            case Order.Open:
                if (gameObject.activeInHierarchy)
                {
                    Open(callback);
                    break;
                }
                else goto case Order.Show;
            case Order.Close:
                if (gameObject.activeInHierarchy)
                {
                    Close(callback);
                    break;
                }
                else goto case Order.Hide;
            case Order.Show:
                Show();
                callback?.Invoke();
                break;
            case Order.Hide:
                Hide();
                callback?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(order), order, null);
        }
    }
}
    
