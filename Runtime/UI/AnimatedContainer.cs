using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Conteiner generico para animar canvas con 'coroutines'.
/// Si la animacion por movimiento no encaja bien, revisar que el Root del container este bien posicionados
/// </summary>
public class AnimatedContainer : MonoBehaviour
{
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

	public event Action OnClosingEnd;
	public event Action OnClosingStart;
	public event Action OnInitialize;
	public event Action OnOpeningEnd;
	public event Action OnOpeningStart;
	public event Action<bool> OnOpenOrCloseEnd;
	public event Action<bool> OnOpenOrCloseStart;
	public event Action<State> OnStateChange;

	private RectTransform rectTransform;
	private RectTransform RectTransform => rectTransform ??= (RectTransform)transform; 
	
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Optional<EventSystem> optEventSystem;

	[SerializeField] private InitState awakeBehaviour = InitState.None;
	[SerializeField] private bool deactivateGameObjectOnHide = true;
	[SerializeField] private bool setBlockRaycast = true;
	[SerializeField] private bool setInteractivity = true;
	[SerializeField] private bool debugPrint = false;
	[SerializeField] private List<AnimatedContainer> subContainers = new List<AnimatedContainer>();

	[FormerlySerializedAs("durationIn"), SerializeField] private float openDuration = .3f;
	[FormerlySerializedAs("durationOut"), SerializeField] private float closeDuration = .3f;
	[field:SerializeField] public TimeScale Time { get; private set; } = TimeScale.Unscaled;

	
	public bool useAlpha = true;
	public AnimationCurve alphaCurveIn = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public AnimationCurve alphaCurveOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public bool useScale = false;
	public Vector3 closeScale = new Vector3(0, 0, 0);
	public AnimationCurve curveInScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public AnimationCurve curveOutScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public bool useMovement = false;
	public bool useParentSizeForDisplacement = true;
	public Direction direction = Direction.Down;
	public AnimationCurve curveOutMovement = AnimationCurve.EaseInOut(0, 0, 1, 1);
	public AnimationCurve curveInMovement = AnimationCurve.EaseInOut(0, 0, 1, 1);

	private RectTransform _parent;

	private Coroutine _currentRoutine;

	public State CurrentState { get; private set; } = State.None;

	public bool InAnimation { get; private set; }

	public bool IsInitialized { get; private set; }= false;

	public bool IsOpen => CurrentState    == State.Open;
	public bool IsClosed => CurrentState  == State.Close;
	public bool IsOpening => CurrentState == State.Opening;
	public bool IsClosing => CurrentState == State.Opening;

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

	private void Awake()
    {
        Initialize();
    }

	private void OnDestroy()
    {
	    if (_currentRoutine != null && GlobalUpdate.Instance) GlobalUpdate.Instance.StopCoroutine(_currentRoutine);
    }



	public Coroutine Close() => Close(null);

    public Coroutine Close(Action postAction)
    {
	    //Debug.Log($">>>Close() {transform.GetHierarchyAsString()}");
        if (optEventSystem) optEventSystem.Value.enabled = false;

        if (CurrentState == State.Close)
        {
            postAction?.Invoke();
            return null;
        }
        
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can't play animation outside of Play State");
            return null;
        }
        
        Initialize();
        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Close");

        SetState(State.Closing);
        return gameObject.activeInHierarchy ? Play(CloseRoutine(postAction)) : null;
    }

    public Coroutine Open() => Open(null);
    
    public Coroutine Open(Action postAction)
    {
        if (CurrentState == State.Open)
        {
            postAction?.Invoke();
            return null;
        }
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Can't play animation outside of Play State");
            return null;
        }
        gameObject.SetActive(true);

        Initialize();
        if (debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Open");
        
        SetState(State.Opening);
        return gameObject.activeInHierarchy ? Play(OpenRoutine(postAction)) :  null;
    }

	public async UniTask OpenAsync(CancellationToken token = default)
	{
		if (CurrentState == State.Open)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			Debug.LogWarning("Can't play animation outside of Play State");
			return;
		}
		gameObject.SetActive(true);

		Initialize();
		if (debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Open");
        
		SetState(State.Opening);

		if (gameObject.activeInHierarchy)
		{
			
			InAnimation = true;
			OnOpeningStart?.Invoke();
			OnOpenOrCloseStart?.Invoke(true);
		
			
			await AnimationAsync(
				openDuration, 
				Vector3.zero, 
				curveInMovement, 
				Vector3.one, 
				curveInScale, 
				1, 
				alphaCurveIn, 
				token);

			Show();
			
			await UniTask.WhenAll(subContainers.ConvertAll(x => x.OpenAsync(token)));

			InAnimation = false;

			OnOpeningEnd?.Invoke();
			OnOpenOrCloseEnd?.Invoke(true);
		}
	}

	public async UniTask CloseAsync(CancellationToken token = default)
	{
		if (optEventSystem) optEventSystem.Value.enabled = false;

		if (CurrentState == State.Close)
		{
			return;
		}
        
		if (!Application.isPlaying)
		{
			Debug.LogWarning("Can't play animation outside of Play State");
			return;
		}
        
		Initialize();
		if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Close");

		SetState(State.Closing);
		
		
		InAnimation = true;
		OnClosingStart?.Invoke();
		OnOpenOrCloseStart?.Invoke(false);
		
		await UniTask.WhenAll(subContainers.ConvertAll(x => x.CloseAsync(token)));

		await AnimationAsync(
			closeDuration,
			GetCloseLocalPosition(), 
			curveOutMovement,
			closeScale,
			curveOutScale,
			0,
			alphaCurveOut,
			token);
		
		InAnimation = false;
		Hide();
		OnClosingEnd?.Invoke();
		OnOpenOrCloseEnd?.Invoke(false);
	}
	
    public void Hide()
    {
        if (optEventSystem) optEventSystem.Value.enabled = false;

        foreach (AnimatedContainer subContainer in subContainers)
            subContainer.Hide();

        if(deactivateGameObjectOnHide) gameObject.SetActive(false);

        Initialize();

        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Hide");

        if (useMovement) RectTransform.anchoredPosition = GetCloseLocalPosition();
        if(useScale) RectTransform.localScale = closeScale;
        if(useAlpha) canvasGroup.alpha = alphaCurveOut.Evaluate(0);

        if(setInteractivity) canvasGroup.interactable = false;
        if(setBlockRaycast) canvasGroup.blocksRaycasts = false;

        SetState(State.Close);

        _currentRoutine = null;
    }


    public void Show()
    {
        gameObject.SetActive(true);
        if (optEventSystem) optEventSystem.Value.enabled = true;
            
        foreach (AnimatedContainer subContainer in subContainers)
            subContainer.Show();
        
        Initialize();
        
        if(debugPrint) Debug.Log($"{transform.GetHierarchyAsString(true)} Show");
        
        if(useMovement) RectTransform.anchoredPosition = Vector3.zero;
        if(useScale) RectTransform.localScale = Vector3.one;
        if(useAlpha) canvasGroup.alpha = 1;
        
        if(setInteractivity) canvasGroup.interactable = true;
        if(setBlockRaycast) canvasGroup.blocksRaycasts = true;
        
        SetState(State.Open);
        
        _currentRoutine = null;
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

    public void CollectSubContainers()
    {
        foreach (Transform child in transform)
        {
            AnimatedContainer container = child.GetComponent<AnimatedContainer>();
            if(container && !subContainers.Contains(container))
                subContainers.Add(container);
        }
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

    /// <summary>
    /// Usamos GlobalUpdate porque al desactivar el gamobject durante la coroutina, cualquiera que este esperando nunca terminaria
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    private Coroutine Play(IEnumerator routine)
    {
	    if(_currentRoutine != null)
		    GlobalUpdate.Instance.StopCoroutine(_currentRoutine);
	    if (GlobalUpdate.Instance)
	    {
		    return _currentRoutine = GlobalUpdate.Instance.StartCoroutine(routine);
	    }
	    return null;
    }

    private Vector3 GetCloseLocalPosition() => Vector3.Scale(GetDirection(), RectTransform.rect.size);

    private Vector3 GetDirection() => DirectionVectors[(int)direction];

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
        
        Vector3 startPosition = RectTransform.anchoredPosition;
        Vector3 startScale = RectTransform.localScale;
        float startAlpha = 1 - targetAlpha;

        Action<float> step = null;

        if (useMovement) step += x => RectTransform.anchoredPosition = Vector3.LerpUnclamped(startPosition, targetPosition, movementCurve.Evaluate(x));
        if (useScale) step += x=> RectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleCurve.Evaluate(x));
        if (useAlpha) step += x => canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, alphaCurve.Evaluate(x));
        do
        {
            t += (Time == TimeScale.Scaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) / duration;
            step?.Invoke(t);
            yield return null;
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
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
    
	
	
    private async UniTask AnimationAsync(float duration,
                                         Vector3 targetPosition, AnimationCurve movementCurve,
                                         Vector3 targetScale, AnimationCurve scaleCurve,
                                         float targetAlpha, AnimationCurve alphaCurve,
                                         CancellationToken token)
    {
	    token = CancellationTokenSource.CreateLinkedTokenSource(token, destroyCancellationToken).Token;
        float t = 0;
        
        Vector3 startPosition = RectTransform.anchoredPosition;
        Vector3 startScale = RectTransform.localScale;
        float startAlpha = 1 - targetAlpha;

        Action<float> step = null;

        if (useMovement) step += x => RectTransform.anchoredPosition = Vector3.LerpUnclamped(startPosition, targetPosition, movementCurve.Evaluate(x));
        if (useScale) step += x=> RectTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, scaleCurve.Evaluate(x));
        if (useAlpha) step += x => canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, alphaCurve.Evaluate(x));
        do
        {
            t += (Time == TimeScale.Scaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime) / duration;
            step?.Invoke(t);
            await UniTask.NextFrame(token, cancelImmediately:true);
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform.parent);
        } while (t < 1);
	    await UniTask.NextFrame(token, cancelImmediately:true);

      
    }


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



    
