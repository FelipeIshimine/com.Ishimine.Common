using UnityEngine;

public class ConstantScaling : ConstantBehaviour
{
    public Vector3 offset = new Vector3(1, 1, 1);
    public Vector3 multiplier = new Vector3(1, 1, 1);

    public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
    public WrapMode wrapMode = WrapMode.Loop;

    protected override void Process(float nTime)
    {
        transform.localScale = offset + multiplier * curve.Evaluate(nTime);
        curve.postWrapMode = wrapMode;
    }
}
