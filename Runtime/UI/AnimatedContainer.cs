using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

/// <summary>
/// Conteiner generico para animar canvas con 'coroutines'.
/// Si la animacion por movimiento no encaja bien, revisar que el Root del container este bien posicionados
/// </summary>
public class AnimatedContainer : MonoBehaviour
{
    public InitState initializationState = InitState.Hide;
    [field:SerializeField, FormerlySerializedAs("timeType")] public TimeScale Time { get; private set; } = TimeScale.Unscaled;
    
    public bool debugPrint = false;
    public event Action OnInitialize;
    public event Action<State> OnStateChange;
    public event Action<bool> OnOpenOrCloseStart;
    public event Action<bool> OnOpenOrCloseEnd;
    public event Action OnOpeningStart;
    public event Action OnOpeningEnd;
    public event Action OnClosingStart;
    public event Action OnClosingEnd;
    
    private RectTransform _parent;
    private RectTransform Parent => _parent ??= transform.parent as RectTransform;

    private RectTransform _rectTransform;
    private RectTransform RectTransform => _rectTransform ??= transform as RectTransform;

    public enum Direction { Right, Left, Up, Down }
    public enum TimeScale { Scaled, Unscaled }
    public enum Order { Open, Close, Show, Hide }

    [SerializeField] private CanvasGroup canvasGroup;

    
    [FormerlySerializedAs("durationIn"), SerializeField, HorizontalGroup("Duration")] private float openDuration = .3f;
    [FormerlySerializedAs("durationOut"), SerializeField, HorizontalGroup("Duration")] private float closeDuration = .3f;
    
    [SerializeField] private bool hideOnDisable = true;
    [SerializeField] private bool openOnEnable = true;
    [SerializeField] private bool deactivateWhenClosed = true;
    
    [FoldoutGroup("CanvasGroup", GroupName = "BlockRaycast & Interactivity"), HorizontalGroup("CanvasGroup/Horizontal")]
    [VerticalGroup("CanvasGroup/Horizontal/BlockRaycast"),SerializeField] private bool setBlockRaycast = true;
    [EnableIf(nameof(setBlockRaycast)),SerializeField, VerticalGroup("CanvasGroup/Horizontal/BlockRaycast"), LabelText("When Open")] 
    private bool blockRaycastWhenOpened = true;
    [EnableIf(nameof(setBlockRaycast)),SerializeField, VerticalGroup("CanvasGroup/Horizontal/BlockRaycast"), LabelText("When Close")]
    private bool blockRaycastWhenClosed = false;

    [VerticalGroup("CanvasGroup/Horizontal/Interactivity"),SerializeField] private bool setInteractivity = true;
    [EnableIf(nameof(setInteractivity)),SerializeField, VerticalGroup("CanvasGroup/Horizontal/Interactivity"),LabelText("When Open")] 
    private bool interactivityWhenOpened = true;
    [EnableIf(nameof(setInteractivity)),SerializeField, VerticalGroup("CanvasGroup/Horizontal/Interactivity"),LabelText("When Close")] 
    private bool interactivityWhenClosed = false;
    
    public enum InitState
    {
        Show,
        Hide
    }

    public enum State
    {
        None,
        Open,
        Close,
        Opening,
        Closing
    }

    [ShowInInspector, ReadOnly] public State CurrentState { get; private set; } = State.None;

    public bool IsOpen => CurrentState == State.Open;
    public bool IsClosed => CurrentState == State.Close;
    public bool IsOpening => CurrentState == State.Opening;
    public bool IsClosing => CurrentState == State.Opening;

    public bool InAnimation { get; private set; }
   

    [TabGroup("Alpha")] public bool useAlpha = true;
    [EnableIf("useAlpha"), TabGroup("Alpha"), LabelText("Open Curve")] public AnimationCurve alphaCurveIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [EnableIf("useAlpha"), TabGroup("Alpha"), LabelText("Close Curve")] public AnimationCurve alphaCurveOut = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [TabGroup("Scale")] public bool useScale = false;
    [EnableIf("useScale"), TabGroup("Scale")] public Vector3 closeScale = new Vector3(0, 0, 0);
    [EnableIf("useScale"), TabGroup("Scale"), LabelText("Open Curve")] public AnimationCurve curveInScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [EnableIf("useScale"), TabGroup("Scale"), LabelText("Close Curve")] public AnimationCurve curveOutScale = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [TabGroup("Movement")] public bool useMovement = false;
    [EnableIf("useMovement"), TabGroup("Movement")] public bool useParentSizeForDisplacement = true;
    [EnableIf("useMovement"), TabGroup("Movement")] public Direction direction = Direction.Down;
    [EnableIf("useMovement"), TabGroup("Movement"), LabelText("Open Curve")] public AnimationCurve curveInMovement = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [EnableIf("useMovement"), TabGroup("Movement"), LabelText("Close Curve")] public AnimationCurve curveOutMovement = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Vector3 _anchoredPositionWhenClosed;
    public bool IsInitialized { get; private set; }= false;

    private IEnumerator _currentRoutine;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if(_currentRoutine != null) StartCoroutine(_currentRoutine);
        else if (openOnEnable) Open();
    }
    
    private void OnDisable()
    {
        if (hideOnDisable) Hide();
    }

    private void SetAnimationRoutine(IEnumerator nRoutine)
    {
        if(_currentRoutine != null) Debug.LogWarning($"Cuidado: Se sobreescribio una rutina previa sin terminar. {transform.GetHierarchyAsString(true)}");
        _currentRoutine = nRoutine;
    }
    
    private void SetState(State nState)
    {
        CurrentState = nState;

        switch (CurrentState)
        {
            case State.Open:
                break;
            case State.Close:
                break;
            case State.Opening:
                break;
            case State.Closing:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        OnStateChange?.Invoke(CurrentState);
    }

    private void OnValidate()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        if (useMovement) SetDirection(direction);
    }
    public void Initialize()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        if (useMovement)
        {
            SetDirection(direction);
            RectTransform.anchoredPosition = Vector3.zero;
            RectTransform.localScale = Vector3.one;
        }

        if (initializationState == InitState.Hide)
            Hide();
        else
            Show();
        
        OnInitialize?.Invoke();
    }

    public void SetDirection(Direction nDirection, bool overrideCurrentPosition = false)
    {
        UpdateAnchoredClosePosition(nDirection);
        if (overrideCurrentPosition) RectTransform.anchoredPosition = _anchoredPositionWhenClosed;
    }

    private void UpdateAnchoredClosePosition(Direction nDirection)
    {
        switch (nDirection)
        {
            case Direction.Right:
                _anchoredPositionWhenClosed = Vector3.right * (useParentSizeForDisplacement ? Parent.rect.width : RectTransform.rect.width);
                break;
            case Direction.Left:
                _anchoredPositionWhenClosed =
                    Vector3.left * (useParentSizeForDisplacement ? Parent.rect.width : RectTransform.rect.width);
                break;
            case Direction.Up:
                _anchoredPositionWhenClosed =
                    Vector3.up * (useParentSizeForDisplacement ? Parent.rect.height : RectTransform.rect.height);
                break;
            case Direction.Down:
                _anchoredPositionWhenClosed = Vector3.down *
                                 (useParentSizeForDisplacement ? Parent.rect.height : RectTransform.rect.height);
                break;
        }
    }

    [Button, ButtonGroup("Animated", GroupName = "Animated")]
    public void Close() => Close(null);

    public void Close(Action postAction)
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can't play animation outside of Play State");
            return;
        }
        
        Initialize();
        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Close");

        SetAnimationRoutine(CloseRoutine(postAction));
        SetState(State.Closing);
        if (gameObject.activeInHierarchy)
            StartCoroutine(_currentRoutine);
    }

    [Button, ButtonGroup("Animated")]
    public void Open() => Open(null);

    public void Open(Action postAction)
    {
        if (CurrentState == State.Open)
        {
            postAction?.Invoke();
            return;
        }
                
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can't play animation outside of Play State");
            return;
        }
        
        Initialize();
        if (debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Open");

        SetAnimationRoutine(OpenRoutine(postAction));
        SetState(State.Opening);
        if (gameObject.activeInHierarchy)
            StartCoroutine(_currentRoutine);
        gameObject.SetActive(true);
    }

    [Button, ButtonGroup("Snap")]
    public void Hide()
    {
        Initialize();

        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Hide");
        
        if(useMovement) RectTransform.anchoredPosition = _anchoredPositionWhenClosed;
        if(useScale) RectTransform.localScale = closeScale;
        if(useAlpha) canvasGroup.alpha = alphaCurveOut.Evaluate(0);

        if(setInteractivity) canvasGroup.interactable = interactivityWhenClosed;
        if(setBlockRaycast) canvasGroup.blocksRaycasts = blockRaycastWhenClosed;
        if (deactivateWhenClosed) gameObject.SetActive(false);

        SetState(State.Close);

        _currentRoutine = null;
    }

    [Button, ButtonGroup("Snap", GroupName = "Snap")]
    public void Show()
    {
        Initialize();
        
        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Show");

        gameObject.SetActive(true);
        
        if(useMovement) RectTransform.anchoredPosition = Vector2.zero;
        if(useScale) RectTransform.localScale = Vector3.one;
        if(useAlpha) canvasGroup.alpha = 1;
        
        if(setInteractivity) canvasGroup.interactable = interactivityWhenOpened;
        if(setBlockRaycast) canvasGroup.blocksRaycasts = blockRaycastWhenOpened;
        
        SetState(State.Open);
        
        _currentRoutine = null;
    }

    private IEnumerator CloseRoutine(Action postAction = null)
    {
        InAnimation = true;
        OnClosingStart?.Invoke();
        OnOpenOrCloseStart?.Invoke(false);
        yield return AnimationRoutine(closeDuration, _anchoredPositionWhenClosed, curveOutMovement, closeScale, curveOutScale, 0, alphaCurveOut,
            () =>
            {
                InAnimation = false;
                Hide();
                _currentRoutine = null;
                postAction?.Invoke();
                OnClosingEnd?.Invoke();
                OnOpenOrCloseEnd?.Invoke(false);
            });
    }

    private IEnumerator OpenRoutine(Action postAction = null)
    {
        InAnimation = true;
        OnOpeningStart?.Invoke();
        OnOpenOrCloseStart?.Invoke(true);
        yield return AnimationRoutine(openDuration, Vector3.zero, curveInMovement, Vector3.one, curveInScale, 1, alphaCurveIn,
            () =>
            {
                InAnimation = false;
                Show();
                _currentRoutine = null;
                postAction?.Invoke();
                OnOpeningEnd?.Invoke();
                OnOpenOrCloseEnd?.Invoke(true);
            });
    }

    private IEnumerator AnimationRoutine(float duration,
        Vector3 targetPosition, AnimationCurve movementCurve,
        Vector3 targetScale, AnimationCurve scaleCurve,
        float targetAlpha, AnimationCurve alphaCurve,
        Action postAction)
    {
        
        float t = 0;
        
        Vector3 startPosition = RectTransform.anchoredPosition;
        Vector3 startScale = RectTransform.localScale;
        float startAlpha = 1 - targetAlpha;

        Action<float> step = null;

        if (useMovement) step += x => RectTransform.anchoredPosition = Vector3.LerpUnclamped(startPosition, targetPosition, movementCurve.Evaluate(x));
        if (useScale) step += x=> RectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleCurve.Evaluate(x));
        if (useAlpha) step += x => canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, alphaCurve.Evaluate(x));

        if (setBlockRaycast) canvasGroup.blocksRaycasts = blockRaycastWhenClosed;
        if (setInteractivity) canvasGroup.interactable = interactivityWhenClosed;
        
        do
        {
            t += ((Time == TimeScale.Scaled) ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) / duration;
            step?.Invoke(t);
            yield return null;
        } while (t < 1);
        postAction?.Invoke();
    }
    
    public void Toggle(bool animated)
    {
        if (animated)
        {
            if (IsOpen || IsOpening)
                Close();
            else
                Open();
        }
        else
        {
            if (IsOpen || IsOpening)
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
    
