using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator)), DefaultExecutionOrder(-100)]
public class AnimationArrayPlayer : MonoBehaviour
{
    public event Action<float> OnClipProgress;
    public AnimationClip[] clips;
    private PlayableGraph _playableGraph;
    private AnimationArrayPlayable _animationArrayPlayable;
    public float blendDuration = .25f;

    public bool autoPlayOnStart = true;

    //todo: public bool advanceOnAnimationEnd = false;

    public bool Initialized { get; private set; } = false;
    
    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (Initialized)
            return;
        _playableGraph = PlayableGraph.Create();
        var playQueuePlayable = ScriptPlayable<AnimationArrayPlayable>.Create(_playableGraph);
        _animationArrayPlayable = playQueuePlayable.GetBehaviour();
        _animationArrayPlayable.Initialize(clips, playQueuePlayable, _playableGraph);
        _animationArrayPlayable.OnClipProgress += OnProgress;

        var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", GetComponent<Animator>());
        playableOutput.SetSourcePlayable(playQueuePlayable, 0);
        Initialized = true;
    }


    private void OnProgress(float t) => OnClipProgress?.Invoke(t);
    
    private void Start()
    {
        if(autoPlayOnStart) _playableGraph.Play();
    }

    private void OnDestroy()
    {
        _playableGraph.Destroy();
    }

    public void Play() => _playableGraph.Play();

    public void Pause() => _playableGraph.Stop();

    public void SetSpeed(float nSpeed)
    {
        if (!Application.isPlaying) return;
        _animationArrayPlayable.SetSpeed(nSpeed);
    }

    public float GetAnimationDuration(int index) => clips[index].length;
    
    public void SetAnimation(int index)
    {
        if(!Application.isPlaying || _animationArrayPlayable.GetIndex() == index)
            return;
        
        if(!Initialized)
            Initialize();
            
        _animationArrayPlayable.SetIndex(index);
        _animationArrayPlayable.SetBlendDuration(blendDuration);
    }


    public void PrintWeights()
    {
        _animationArrayPlayable.PrintWeights();
    }

    public void SetAnimationTime(int index, float time) => _animationArrayPlayable.SetAnimationTime(index, time);
}