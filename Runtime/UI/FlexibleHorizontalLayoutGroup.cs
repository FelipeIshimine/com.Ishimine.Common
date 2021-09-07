using UnityEngine;
using UnityEngine.UI;

public class FlexibleHorizontalLayoutGroup : HorizontalLayoutGroup
{
    public override void SetLayoutHorizontal()
    {
        base.SetLayoutHorizontal();
        RectTransform rTransform = transform as RectTransform; 
        float t = 0;

        int count = -1;
        foreach (RectTransform children in transform)
        {
            t += children.sizeDelta.x;
            count++;
        }

        rTransform.sizeDelta = new Vector2( t+ count * spacing,rTransform.sizeDelta.y);
    }

}