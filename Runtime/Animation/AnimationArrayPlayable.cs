using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimationArrayPlayable : PlayableBehaviour
{
    private int _currentClipIndex = 0;
    private AnimationMixerPlayable _mixer;
    private float _blendDuration = .25f;
    private float _blendSpeed = 4;

    private float[] rawWeight;

    private Playable[] _playables;

    private AnimationClip[] _clips;
    public event Action<float> OnClipProgress;
    
    
    public void Initialize(AnimationClip[] clipsToPlay, Playable owner, PlayableGraph graph)
    {
        _clips = clipsToPlay;
        owner.SetInputCount(1);
        _mixer = AnimationMixerPlayable.Create(graph, clipsToPlay.Length);
        graph.Connect(_mixer, 0, owner, 0);
        owner.SetInputWeight(0, 1);
        _playables = new Playable[clipsToPlay.Length];

        rawWeight = new float[clipsToPlay.Length];
        for (int clipIndex = 0 ; clipIndex < _mixer.GetInputCount() ; ++clipIndex)
        {
            _playables[clipIndex] = AnimationClipPlayable.Create(graph, clipsToPlay[clipIndex]);
            graph.Connect(_playables[clipIndex], 0, _mixer, clipIndex);
            _mixer.SetInputWeight(clipIndex, 0f);
            rawWeight[clipIndex] = 0;
        }
    }

    public override void PrepareFrame(Playable owner, FrameData info)
    {
        if (_mixer.GetInputCount() == 0)
            return;

        float totalRawWeight = 0;
        for (int i = 0; i < rawWeight.Length; i++)
        {
            if (i == _currentClipIndex)
                rawWeight[i] = rawWeight[i]>=1?1:Mathf.Clamp01(rawWeight[i] + info.deltaTime * _blendSpeed);
            else
                rawWeight[i] = rawWeight[i]<=0?0:Mathf.Clamp01(rawWeight[i] - info.deltaTime * _blendSpeed);

            totalRawWeight += rawWeight[i];
        }

        for (int i = 0; i < rawWeight.Length; i++)
            _mixer.SetInputWeight(i, rawWeight[i] / totalRawWeight);   
        
        OnClipProgress?.Invoke((float)(_playables[_currentClipIndex].GetTime() / (_clips[_currentClipIndex].averageDuration * _playables[_currentClipIndex].GetSpeed())));
    }

    public int GetIndex() => _currentClipIndex;
    public void SetIndex(int index)
    {
        _currentClipIndex = index;
        
        if(!_clips[index].isLooping)
            _mixer.GetInput(_currentClipIndex).SetTime(0);
    }

    public void SetBlendDuration(float blendDuration)
    {
        _blendDuration = blendDuration;
        _blendSpeed = 1/blendDuration;
    }

    public void SetSpeed(float nSpeed) => _mixer.SetSpeed(nSpeed);

    public void PrintWeights()
    {
        for (int i = 0; i < _playables.Length; i++)
            Debug.Log($"{i}:{_mixer.GetInputWeight(i)}");
    }

    public void SetAnimationTime(int index, float time) => _playables[index].SetTime(time);
}