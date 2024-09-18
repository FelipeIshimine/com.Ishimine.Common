using UnityEngine;

public abstract class ConstantBehaviour : MonoBehaviour
{
    public float duration = 1;
    public float speed = 1;
    [SerializeField] private bool modifyInEditMode = true;

    public bool resetOnEnable = false;

    public float timeOffset = 0;

    private float _time;

    private void OnDurationChange() => speed = 1 / duration;

    private void OnSpeedChange() => duration = 1 / speed;

    protected virtual void Awake()
    {
        ResetTime();
    }

    protected void ResetTime()
    {
        _time = timeOffset;
        Process(timeOffset);
    }
    protected virtual void OnEnable()
    {
        if(resetOnEnable)
            ResetTime();
    }

    protected virtual void Update()
    {
        _time += Time.deltaTime / duration;
        Process(_time);
    }

    public void SetDuration(float nDuration)
    {
        duration = nDuration;
        OnDurationChange();
    }

    internal void SetTimeOffset(float nTimeOffset)
    {
        timeOffset = nTimeOffset;
        if (!Application.isPlaying)
            Process(timeOffset);
    }

    public void SetSpeed(float nSpeed)
    {
        speed= nSpeed;
        OnSpeedChange();
    }

    protected virtual void OnValidate()
    {
        if(modifyInEditMode)
            Process(timeOffset);
    }
    protected abstract void Process(float nTime);

}
