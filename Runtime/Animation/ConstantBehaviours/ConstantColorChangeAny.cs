using UnityEngine;

public class ConstantColorChangeAny : ConstantBehaviour
{
    [SerializeReference] public IUseColorAndSprite image;

    public Color endColor;
    public Color startColor;

    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public WrapMode wrapMode = WrapMode.PingPong;

    protected override void Process(float nTime)
    {
        image.Color = Color.Lerp(startColor, endColor, curve.Evaluate(nTime));
    }

    public void Set(bool value)
    {
        enabled = value;
        if (!value)
            ResetTime();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        curve.preWrapMode = curve.postWrapMode = wrapMode;
    }
}
