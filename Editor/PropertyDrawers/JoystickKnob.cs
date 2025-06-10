// JoystickKnob.cs
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickKnob : VisualElement
{
    public event Action<Vector3> OnValueChanged;
    Vector3 currentValue;
    readonly float maxMag, step;
    bool dragging;

    const float Size = 60f;
    const float HandleFrac = 0.4f;

    public JoystickKnob(float maxMagnitude, float step)
    {
        maxMag = Mathf.Max(0.0001f, maxMagnitude);
        this.step = step;

        style.width     = Size;
        style.height    = Size;
        style.position  = Position.Relative;

        // background
        var bg = new VisualElement();
        bg.style.width  = Size;
        bg.style.height = Size;
        bg.style.backgroundColor = new Color(0.2f,0.2f,0.2f);
        bg.style.borderTopLeftRadius     = Size/2;
        bg.style.borderTopRightRadius    = Size/2;
        bg.style.borderBottomLeftRadius  = Size/2;
        bg.style.borderBottomRightRadius = Size/2;
        bg.style.position = Position.Absolute;
        Add(bg);

        // handle
        var handle = new VisualElement();
        float hs = Size * HandleFrac;
        handle.style.width  = hs;
        handle.style.height = hs;
        handle.style.backgroundColor = Color.white;
        handle.style.borderTopLeftRadius     = hs/2;
        handle.style.borderTopRightRadius    = hs/2;
        handle.style.borderBottomLeftRadius  = hs/2;
        handle.style.borderBottomRightRadius = hs/2;
        handle.style.position = Position.Absolute;
        Add(handle);

        pickingMode = PickingMode.Position;
        RegisterCallback<PointerDownEvent>(e => StartDrag(e, handle));
        RegisterCallback<PointerMoveEvent>(e => Drag(e, handle));
        RegisterCallback<PointerUpEvent>(EndDrag);
        RegisterCallback<PointerCancelEvent>(EndDrag);

        this.style.marginBottom =
            this.style.marginLeft =
                this.style.marginRight =
                    this.style.marginTop = 10;
    }

    void StartDrag(PointerDownEvent e, VisualElement handle)
    {
        dragging = true;
        e.StopPropagation();
        UpdateByPointer(e.localPosition, handle);
    }

    void Drag(PointerMoveEvent e, VisualElement handle)
    {
        if (!dragging) return;
        e.StopPropagation();
        UpdateByPointer(e.localPosition, handle);
    }

    void EndDrag(EventBase e)
    {
        dragging = false;
        e.StopPropagation();
    }

    void UpdateByPointer(Vector2 lp, VisualElement handle)
    {
        Vector2 center = new Vector2(Size/2, Size/2);
        Vector2 delta  = lp - center;
        delta.y = -delta.y;

        float radius = Size/2;
        if (delta.magnitude > radius) delta = delta.normalized * radius;
        Vector2 norm = delta / radius;

        // world-space value before snapping
        Vector3 raw = new Vector3(norm.x * maxMag,
                                  norm.y * maxMag,
                                  currentValue.z);

        // apply step in world units
        if (step > 0f)
        {
            raw.x = Mathf.Round(raw.x / step) * step;
            raw.y = Mathf.Round(raw.y / step) * step;
        }

        // recompute normalized after snapping
        norm.x = raw.x / maxMag;
        norm.y = raw.y / maxMag;

        currentValue = raw;

        // reposition handle
        float offset = handle.resolvedStyle.width / 2f;
        Vector2 pos = center + new Vector2(norm.x, -norm.y) * radius
                      - new Vector2(offset, offset);
        handle.style.left = pos.x;
        handle.style.top  = pos.y;

        OnValueChanged?.Invoke(currentValue);
    }

    public void SetValue(Vector3 v)
    {
        // apply step
        if (step > 0f)
        {
            v.x = Mathf.Round(v.x / step) * step;
            v.y = Mathf.Round(v.y / step) * step;
        }

        // normalized
        Vector2 norm = new Vector2(v.x / maxMag, v.y / maxMag);
        norm = Vector2.ClampMagnitude(norm, 1f);

        currentValue = new Vector3(norm.x * maxMag, norm.y * maxMag, v.z);

        // move handle
        if (childCount < 2) return;
        var handle = this.ElementAt(1);
        Vector2 center = new Vector2(Size/2, Size/2);
        float radius = Size/2;
        float offset = handle.resolvedStyle.width / 2f;
        Vector2 pos = center + new Vector2(norm.x, -norm.y) * radius
                      - new Vector2(offset, offset);
        
        handle.style.left = pos.x;
        handle.style.top  = pos.y;
    }
}
