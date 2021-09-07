using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ConstantColorChange))]
public class ConstantColorChange : MonoBehaviour
{
    public float flickerSpeed = 0.5f;

    public Image image;
    
    public Color endColor;
    public Color startColor;
    
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float _time = 0;

    public bool resetOnDisable = true;

    private void Awake()
    {
        if(!image) image = GetComponent<Image>();
        curve.postWrapMode = WrapMode.Loop;
    }

    private void Update()
    {
        _time += Time.deltaTime * flickerSpeed; 
        _time %= 1;
        image.color = Color.Lerp(startColor, endColor, curve.Evaluate(_time));
    }

    public void Set(bool value)
    {
        enabled = value;
        if (resetOnDisable && !value)
        {
            _time = 0;
            image.color = Color.Lerp(startColor, endColor, curve.Evaluate(_time));
        }
    }
}