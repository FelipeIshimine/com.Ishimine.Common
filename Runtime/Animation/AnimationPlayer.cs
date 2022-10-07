using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[DefaultExecutionOrder(1)]
public class AnimationPlayer : MonoBehaviour
{
    private PlayableGraph _graph;
    private PlayableOutput _output;
    [field:SerializeField, FormerlySerializedAs("animator")] public Animator Animator { get; private set; }
    
    public bool useAnimationEvents = false;
    public AnimationClip fallbackAnimation;
    private AnimationMixerPlayable _mixerPlayable;
    [ShowInInspector] private readonly List<TransitionData> _transitions = new List<TransitionData>();
    private readonly Queue<Action> _transitionCallbacksQueue = new Queue<Action>();
    
    [ShowInInspector] private readonly Dictionary<AnimationClipPlayable, Action> _playableCallbacks = new Dictionary<AnimationClipPlayable, Action>();

    [Tooltip("If TRUE, an overwritten animation may not invoke their Callback")]
    public bool canSkipAnimationCallback = false;

    private bool _initialized = false;
    public enum WrapMode
    {
        None,
        Clamp,
        Loop
    }
    
    public class AnimationSettings
    {
        public WrapMode Mode;
        public AnimationClip Clip;
        [MinValue(0)] public float Speed = 1;
        public Action Callback;
    }
    
    public class TransitionSettings
    {
        public static readonly TransitionSettings Instant = new TransitionSettings()
        {
            Duration = 0f,
            Callback = null
        };
        
        [MinValue(0)] public float Duration = .25f;
        public AnimationCurve Curve = AnimationCurve.Linear(0,0,1,1);
        public Action Callback;
        
        public static readonly TransitionSettings Default = new TransitionSettings()
        {
            Duration = .25f,
            Curve = AnimationCurve.Linear(0, 0, 1, 1),
            Callback = null
        };
    }

    private struct TransitionData
    {
        public AnimationMixerPlayable Mixer;
        public TransitionSettings Settings;
        public float Progress;
    }

    
    private void Awake()
    {
        //Initialize();
    }

    public void Initialize()
    {
        if (_initialized) return;
        
        _initialized = true;
        Animator.fireEvents = useAnimationEvents;
        _graph = PlayableGraph.Create(name);
        _output = AnimationPlayableOutput.Create(_graph, "Output", Animator);
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _graph.Play();
        PlayFallbackAnimation();
    }

    public void PlayFallbackAnimation(float transitionDuration = 0)
    {
        Play(new AnimationSettings()
            {
                Clip = fallbackAnimation,
                Mode = WrapMode.Loop
            },
            new TransitionSettings()
            {
                Duration = transitionDuration
            });
    }
    
    public void Play(AnimationClip clip, WrapMode mode = WrapMode.None)
    {
        Play(new AnimationSettings()
        {
            Clip = clip,
            Mode = mode
        },
            TransitionSettings.Default);
    }

    public void Play(AnimationSettings animationSettings) => Play(animationSettings, TransitionSettings.Default);
    
    public void Play(AnimationSettings animationSettings, TransitionSettings transitionSettings)
    {
        if(!_initialized) Initialize();
        
        var prevAnimationPlayable = _mixerPlayable;
        _mixerPlayable = AnimationMixerPlayable.Create(_graph, 2);

        var nAnimationPlayable = AnimationClipPlayable.Create(_graph, animationSettings.Clip);
        nAnimationPlayable.SetSpeed(animationSettings.Speed);
        
        if(animationSettings.Mode == WrapMode.None)
            nAnimationPlayable.SetDuration(animationSettings.Clip.length);

        if(animationSettings.Callback != null) _playableCallbacks[nAnimationPlayable] = animationSettings.Callback;
        
        
        if (!prevAnimationPlayable.IsNull())
        {
            _graph.Connect(prevAnimationPlayable, 0, _mixerPlayable, 0);
            _graph.Connect(nAnimationPlayable, 0, _mixerPlayable, 1);
        }
        else
        {
            var nullPlayable = AnimationClipPlayable.Create(_graph, null);
            _graph.Connect(nullPlayable, 0, _mixerPlayable, 0);
            _graph.Connect(nAnimationPlayable, 0, _mixerPlayable, 1);
        }
        
        _mixerPlayable.SetInputWeight(0,0);
        _mixerPlayable.SetInputWeight(0,1);
        _transitions.Insert(0,new TransitionData()
        {
            Settings = transitionSettings,
            Mixer = _mixerPlayable
        });
        _output.SetSourcePlayable(_mixerPlayable);
    }

    [Button]
    public void Play() => _graph.Play();

    [Button]
    public void Stop() => _graph.Stop(); 

    private void OnDestroy()
    {
        if(_graph.IsValid()) _graph.Destroy();
    }

    private void Reset()
    {
        Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(!_initialized) return;
        
        var keys = new List<AnimationClipPlayable>(_playableCallbacks.Keys);
        for (var index = keys.Count - 1; index >= 0; index--)
        {
            var playable = keys[index];
            if (playable.IsDone())
            {
                _playableCallbacks[playable].Invoke();
                _playableCallbacks.Remove(playable);
            }
        }

        for (int i = _transitions.Count - 1; i >= 0; i--)
        {
            
            var transitionData = _transitions[i];
            float progress = transitionData.Progress =  Mathf.Clamp01(transitionData.Progress  +Time.deltaTime / _transitions[i].Settings.Duration);

            transitionData.Mixer.SetInputWeight(0, 1 - _transitions[i].Settings.Curve.Evaluate(progress));
            transitionData.Mixer.SetInputWeight(1, _transitions[i].Settings.Curve.Evaluate(progress));

            _transitions[i] = transitionData;

            var playable = (AnimationClipPlayable)transitionData.Mixer.GetInput(1);

            if (i == (_transitions.Count - 1) && progress >= 1 && (canSkipAnimationCallback || !_playableCallbacks.ContainsKey(playable)))
            {
                if(transitionData.Settings.Callback != null) _transitionCallbacksQueue.Enqueue(transitionData.Settings.Callback);
                
                if(_playableCallbacks.ContainsKey(playable))
                    _playableCallbacks.Remove(playable);

                transitionData.Mixer.GetInput(0).Destroy();
                _transitions.RemoveAt(i);
                
            }
        }

        if (fallbackAnimation && !_mixerPlayable.IsNull())
        {
            var playable = (AnimationClipPlayable)_mixerPlayable.GetInput(1);
            if(playable.IsDone()) Play(fallbackAnimation, WrapMode.Loop);
        }
    }


    private void LateUpdate()
    {
        if(_transitionCallbacksQueue.Count>0)
            _transitionCallbacksQueue.Dequeue()?.Invoke();
       
    }

    private void OnDisable()
    {
        if(_initialized)
            _graph.Stop();
    }

    private void OnEnable()
    {
        if(_initialized)
            _graph.Play();
    }


}
