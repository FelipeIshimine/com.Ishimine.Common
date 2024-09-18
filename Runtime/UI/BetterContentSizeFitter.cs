using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class BetterContentSizeFitter : MonoBehaviour
{
    private HorizontalOrVerticalLayoutGroup _target;

    private HorizontalOrVerticalLayoutGroup Target
    {
        get
        {
            if (!_target) _target = GetComponent<HorizontalOrVerticalLayoutGroup>();
            return _target;
        }
    }

    public enum Orientation
    {
        Horizontal, Vertical
    }

    public void Refresh() => Refresh(Target is HorizontalLayoutGroup?Orientation.Horizontal:Orientation.Vertical);

    public float Refresh(Orientation orientation)
    {
        RectTransform rTransform = transform as RectTransform; 
        float t = 0;

        foreach (RectTransform children in transform)
            t += orientation == Orientation.Horizontal ? children.sizeDelta.x : children.sizeDelta.y;

        rTransform.sizeDelta = orientation == Orientation.Horizontal ? new Vector2(t, rTransform.sizeDelta.y) : new Vector2( rTransform.sizeDelta.x, t);
        return t;
    }
    
}