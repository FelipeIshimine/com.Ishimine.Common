using UnityEngine;

public class ConstantShaking : MonoBehaviour
{
    [SerializeField] private Vector3 speed = new Vector2(1,0);
    [SerializeField] private Vector3 startOffset = new Vector3();
    [SerializeField] private Vector3 magnitude = new Vector3(1,1,1);
    [SerializeField] private AnimationCurve xCurve;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private AnimationCurve zCurve;

    public bool resetOnEnable = true;
    public bool resetOnDisable = true;

    private float _t = 0;

    private void Awake()
    {
        xCurve.postWrapMode = WrapMode.Loop;
        yCurve.postWrapMode = WrapMode.Loop;
        zCurve.postWrapMode = WrapMode.Loop;
    }

    private void Initialize()
    {
        _t = 0;
        Step();
    }

    private void Step()
    {
        transform.localPosition = 
            new Vector3(
                xCurve.Evaluate(startOffset.x + _t * speed.x)*magnitude.x,
                yCurve.Evaluate(startOffset.y + _t * speed.y)*magnitude.y,
                zCurve.Evaluate(startOffset.z + _t * speed.z)*magnitude.z);
        
        _t += Time.deltaTime;
    }

    private void Update()
    {
        Step();
    }

    private void OnEnable()
    {
        if(resetOnEnable) Initialize();
    }

    private void OnDisable()
    {
        if(resetOnDisable) Initialize();
    }

    public void Set(bool value)
    {
        if(value == enabled)
            return;
        if((!value && resetOnDisable) || (value && resetOnEnable)) Initialize();
        enabled = value;
    }
}
