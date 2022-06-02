using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ConstantBehaviourSync : MonoBehaviour
{
    public bool syncDurationAndSpeed = true;
    [ShowIf("syncDurationAndSpeed"), OnValueChanged("OnDurationChange"), MinValue(.001f)] public float duration = 1;
    [ShowIf("syncDurationAndSpeed"), OnValueChanged("OnSpeedChange"), MinValue(.001f)] public float speed = 1;

    public bool syncTimeOffset = true;
    [ShowIf("syncTimeOffset")] public float timeOffset = 0;

    [SerializeField] private List<ConstantBehaviour> constantBehaviours;

    public bool resetOnEnable = true;
    
#if UNITY_EDITOR
    [OnValueChanged(nameof(Collect))]public GameObject collect;

    [Button]
    private void Collect(GameObject go)
    {
        CollectFrom(go);
        collect = null;
    }
    
#endif

    private void OnDurationChange() => speed = 1 / duration;
    private void OnSpeedChange() => duration = 1 / speed;

    private void OnValidate()
    {
        CollectFrom(gameObject);
        
        foreach (var item in constantBehaviours)
        {
            if (syncDurationAndSpeed)
                item.SetDuration(duration);
            if(syncTimeOffset)
                item.SetTimeOffset(timeOffset);
            item.resetOnEnable = resetOnEnable;
        }
    }
    
    private void CollectFrom(GameObject target)
    {
        var components = target.GetComponents<ConstantBehaviour>();
        foreach (ConstantBehaviour constantBehaviour in components)
        {
            if(!constantBehaviours.Contains(constantBehaviour))
                constantBehaviours.Add(constantBehaviour);
        }
    }
}