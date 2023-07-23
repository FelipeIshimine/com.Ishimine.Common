using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redcode.Awaiting;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Conteiner generico para animar canvas con 'coroutines'.
/// Si la animacion por movimiento no encaja bien, revisar que el Root del container este bien posicionados
/// </summary>
public class AnimatedContainer : MonoBehaviour, ISelfValidator, ISerializationCallbackReceiver
{
    #region Events
    public event Action OnInitialize;
    public event Action<State> OnStateChange;
    public event Action<bool> OnOpenOrCloseStart;
    public event Action<bool> OnOpenOrCloseEnd;
    public event Action OnOpeningStart;
    public event Action OnOpeningEnd;
    public event Action OnClosingStart;
    public event Action OnClosingEnd;
    #endregion

    private RectTransform _parent;
    
    [FoldoutGroup("Dependencies"),SerializeField, FormerlySerializedAs("_rectTransform")] private RectTransform rectTransform;
    [FoldoutGroup("Dependencies"),SerializeField] private CanvasGroup canvasGroup;
    [FoldoutGroup("Dependencies"),SerializeField] private Optional<EventSystem> optEventSystem;

    public InitState awakeBehaviour = InitState.None;
    [field:SerializeField, FormerlySerializedAs("timeType")] public TimeScale Time { get; private set; } = TimeScale.Unscaled;
    public bool deactivateGameObjectOnHide = true;
    public bool debugPrint = false;

    [HorizontalGroup("Horizontal")]
    [VerticalGroup("Horizontal/BlockRaycast"), SerializeField] private bool setBlockRaycast = true;
    [VerticalGroup("Horizontal/Interactivity"),SerializeField] private bool setInteractivity = true;


    [SerializeField, FoldoutGroup("SubContainers")] private List<AnimatedContainer> subContainers = new List<AnimatedContainer>();
    public enum InitState
    {
        None,
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
   
    [FormerlySerializedAs("durationIn"), SerializeField,  BoxGroup("Animation"),HorizontalGroup("Animation/Duration")] private float openDuration = .3f;
    [FormerlySerializedAs("durationOut"), SerializeField, BoxGroup("Animation"),HorizontalGroup("Animation/Duration")] private float closeDuration = .3f;
    [TabGroup("Alpha")] public bool useAlpha = true;
    [TabGroup("Alpha"),EnableIf("useAlpha"),  LabelText("Open Curve")] public AnimationCurve alphaCurveIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Alpha"),EnableIf("useAlpha"),  LabelText("Close Curve")] public AnimationCurve alphaCurveOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Scale")] public bool useScale = false;
    [TabGroup("Scale"), EnableIf("useScale") ] public Vector3 closeScale = new Vector3(0, 0, 0);
    [TabGroup("Scale"), EnableIf("useScale") , LabelText("Open Curve")] public AnimationCurve curveInScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Scale"), EnableIf("useScale") , LabelText("Close Curve")] public AnimationCurve curveOutScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Movement")] public bool useMovement = false;
    [TabGroup("Movement"), EnableIf("useMovement")] public bool useParentSizeForDisplacement = true;
    [TabGroup("Movement"), EnableIf("useMovement")] public Direction direction = Direction.Down;
    [TabGroup("Movement"), EnableIf("useMovement"), LabelText("Close Curve")] public AnimationCurve curveOutMovement = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Movement"), EnableIf("useMovement"), LabelText("Open Curve")] public AnimationCurve curveInMovement = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static readonly Vector3[] DirectionVectors = new Vector3[]
    {
        new Vector3(0,1,0),
        new Vector3(1,1,0),
        new Vector3(-1,1,0),
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,-1,0),
        new Vector3(1,-1,0),
        new Vector3(-1,-1,0),
    };

    public bool IsInitialized { get; private set; }= false;

    private IEnumerator _currentRoutine;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if(_currentRoutine != null) StartCoroutine(_currentRoutine);
    }
    
    private void OnDisable()
    {
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
        
        //if (useMovement) SetDirection(direction);
    }
    public void Initialize()
    {
        if (IsInitialized) return;
        IsInitialized = true;

        switch (awakeBehaviour)
        {
            case InitState.Hide:
                Hide();
                break;
            case InitState.Show:
                Show();
                break;
            case InitState.None:
                break;
        }
        
        OnInitialize?.Invoke();
    }

    public async Task CloseAsync()
    {
	    bool deactivateGo = deactivateGameObjectOnHide;
	    deactivateGameObjectOnHide = false;
	    await Close(null);
	    deactivateGameObjectOnHide = deactivateGo;
	    if(deactivateGameObjectOnHide) gameObject.SetActive(false);
    }

    public YieldInstruction Close() => Close(null);
    
    [Button, ButtonGroup("Animated", GroupName = "Animated")]
    public YieldInstruction Close(Action postAction)
    {
        if (optEventSystem) optEventSystem.Value.enabled = false;

        if (CurrentState == State.Close)
        {
            postAction?.Invoke();
            return new YieldInstruction();
        }
        
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can't play animation outside of Play State");
            return new YieldInstruction();
        }
        
        Initialize();
        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Close");

        SetState(State.Closing);
        return gameObject.activeInHierarchy ? Play(CloseRoutine(postAction)) : new YieldInstruction();
    }

    private YieldInstruction Play(IEnumerator routine)
    {
	    if(_currentRoutine != null)
		    StopCoroutine(_currentRoutine);
	    return StartCoroutine(routine);
    }

    [Button, ButtonGroup("Animated")]
    public YieldInstruction Open() => Open(null);

    public YieldInstruction Open(Action postAction)
    {
        if (CurrentState == State.Open)
        {
            postAction?.Invoke();
            return new YieldInstruction();
        }
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can't play animation outside of Play State");
            return new YieldInstruction();
        }
        gameObject.SetActive(true);

        Initialize();
        if (debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Open");
        
        SetState(State.Opening);
        return gameObject.activeInHierarchy ? Play(OpenRoutine(postAction)) :  new YieldInstruction();
    }

    [Button, ButtonGroup("Snap")]
    public void Hide()
    {
        if (optEventSystem) optEventSystem.Value.enabled = false;

        foreach (AnimatedContainer subContainer in subContainers)
            subContainer.Hide();

        if(deactivateGameObjectOnHide) gameObject.SetActive(false);

        Initialize();

        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Hide");

        if (useMovement) rectTransform.transform.localPosition = GetCloseLocalPosition();
        if(useScale) rectTransform.localScale = closeScale;
        if(useAlpha) canvasGroup.alpha = alphaCurveOut.Evaluate(0);

        if(setInteractivity) canvasGroup.interactable = false;
        if(setBlockRaycast) canvasGroup.blocksRaycasts = false;

        SetState(State.Close);

        _currentRoutine = null;
    }

    private Vector3 GetCloseLocalPosition() => Vector3.Scale(GetDirection(), rectTransform.rect.size);

    private Vector3 GetDirection() => DirectionVectors[(int)direction];
     

    [Button, ButtonGroup("Snap", GroupName = "Snap")]
    public void Show()
    {
        gameObject.SetActive(true);
        if (optEventSystem) optEventSystem.Value.enabled = true;
            
        foreach (AnimatedContainer subContainer in subContainers)
            subContainer.Show();
        
        Initialize();
        
        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Show");
        
        if(useMovement) rectTransform.localPosition = Vector3.zero;
        if(useScale) rectTransform.localScale = Vector3.one;
        if(useAlpha) canvasGroup.alpha = 1;
        
        if(setInteractivity) canvasGroup.interactable = true;
        if(setBlockRaycast) canvasGroup.blocksRaycasts = true;
        
        SetState(State.Open);
        
        _currentRoutine = null;
    }

    private IEnumerator CloseRoutine(Action postAction = null)
    {
        InAnimation = true;
        OnClosingStart?.Invoke();
        OnOpenOrCloseStart?.Invoke(false);
        yield return AnimationRoutine(closeDuration, GetCloseLocalPosition(), curveOutMovement, closeScale, curveOutScale, 0, alphaCurveOut, false,
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
        yield return AnimationRoutine(openDuration, Vector3.zero, curveInMovement, Vector3.one, curveInScale, 1, alphaCurveIn, true,
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
        bool isOpen,
        Action postAction)
    {
        
        float t = 0;
        
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 startScale = rectTransform.localScale;
        float startAlpha = 1 - targetAlpha;

        Action<float> step = null;

        if (useMovement) step += x => rectTransform.localPosition = Vector3.LerpUnclamped(startPosition, targetPosition, movementCurve.Evaluate(x));
        if (useScale) step += x=> rectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleCurve.Evaluate(x));
        if (useAlpha) step += x => canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, alphaCurve.Evaluate(x));

        //if (setBlockRaycast) canvasGroup.blocksRaycasts = false;
        //if (setInteractivity) canvasGroup.interactable = false;
  
        do
        {
            t += (Time == TimeScale.Scaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) / duration;
            step?.Invoke(t);
            
            yield return null;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
        } while (t < 1);
        yield return null;

        List<YieldInstruction> coroutines = new List<YieldInstruction>();

        foreach (AnimatedContainer subContainer in subContainers)
            coroutines.Add((isOpen ? subContainer.Open() : subContainer.Close()));

        for (var index = 0; index < coroutines.Count; index++)
        {
            var yieldInstruction = coroutines[index];
            yield return yieldInstruction;
        }

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

    [Button, FoldoutGroup("SubContainers")]
    public void CollectSubContainers()
    {
        foreach (Transform child in transform)
        {
            AnimatedContainer container = child.GetComponent<AnimatedContainer>();
            if(container && !subContainers.Contains(container))
                subContainers.Add(container);
        }
    }

    public void Validate(SelfValidationResult result)
    {
        if (rectTransform == null)
            result.AddError("RectTransform not set").WithFix("Fix",()=> SetRectTransform(transform as RectTransform));
    }

    private void SetRectTransform(RectTransform nRectTransform) => this.rectTransform = nRectTransform;
    public void OnBeforeSerialize()
    {
        if(rectTransform == null) SetRectTransform(transform as RectTransform);
    }

    public void OnAfterDeserialize()
    {
    }
    
    
    public enum Direction
    {
        Up = 0,
        UpRight = 1,
        UpLeft = 2,
        Right = 3,
        Left = 4,
        Down = 5, 
        DownRight = 6,
        DownLef = 7
    }
    public enum TimeScale { Scaled, Unscaled }
    public enum Order { Open, Close, Show, Hide }
}
    
