using UnityEngine;
using Sirenix.OdinInspector;

public class ConstantBehaviourSync : MonoBehaviour
{
    public bool syncDurationAndSpeed = true;
    [ShowIf("syncDurationAndSpeed"), OnValueChanged("OnDurationChange"), MinValue(.001f)] public float duration = 1;
    [ShowIf("syncDurationAndSpeed"), OnValueChanged("OnSpeedChange"), MinValue(.001f)] public float speed = 1;

    public bool syncTimeOffset = true;
    [ShowIf("syncTimeOffset")] public float timeOffset = 0;

    [SerializeField] private ConstantBehaviour[] constantBehaviours;

    public bool resetOnEnable = true;

    private void OnDurationChange() => speed = 1 / duration;
    private void OnSpeedChange() => duration = 1 / speed;

    private void OnValidate()
    {
        constantBehaviours = GetComponents<ConstantBehaviour>();

        foreach (var item in constantBehaviours)
        {
            if (syncDurationAndSpeed)
                item.SetDuration(duration);
            if(syncTimeOffset)
                item.SetTimeOffset(timeOffset);
            item.resetOnEnable = resetOnEnable;
        }
    }
}