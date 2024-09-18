using System.Collections.Generic;
using UnityEngine;

public class ConstantBehaviourSync : MonoBehaviour
{
    public bool syncDurationAndSpeed = true;
    public float duration = 1;
    public float speed = 1;

    public bool syncTimeOffset = true;
    public float timeOffset = 0;

    [SerializeField] private List<ConstantBehaviour> constantBehaviours;

    public bool resetOnEnable = true;
    
#if UNITY_EDITOR
    public GameObject collect;

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