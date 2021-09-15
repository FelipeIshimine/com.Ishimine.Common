using UnityEngine;
using UnityEngine.UI;

public class ConstantColorChange : ConstantBehaviour
{
    public Image image;
    public Color endColor;
    public Color startColor;
    
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    protected override void Awake()
    {
        base.Awake();
        if (!image) image = GetComponent<Image>();
        curve.postWrapMode = WrapMode.Loop;
    }

    protected override void Process(float nTime)
    {
        image.color = Color.Lerp(startColor, endColor, curve.Evaluate(nTime));
    }

    public void Set(bool value)
    {
        enabled = value;
        if (!value)
            ResetTime();
    }
}