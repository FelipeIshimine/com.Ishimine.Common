using UnityEngine;
using UnityEngine.UI;

public class FlexibleVerticalLayoutGroup : VerticalLayoutGroup
{
   public override void SetLayoutVertical()
   {
      base.SetLayoutVertical();
      
      RectTransform rTransform = transform as RectTransform; 
      float t = 0;
      int count = -1;

      foreach (RectTransform children in transform)
      {
         t += children.sizeDelta.y;
         count++;
      }

      rTransform.sizeDelta = new Vector2( rTransform.sizeDelta.x, t + count * spacing);
   }
}