using UnityEngine;

public class ConstantFloatingAdvanced : MonoBehaviour
{
    public Vector3 magnitude = new Vector3(1,1,1);
    public Vector3 speed = new Vector3(1, 1, 1);
    public Vector3 timeOffset = new Vector3(0, .33f, .66f);

    public Vector3 positionOffset = new Vector3(0, 0, 0);

    private Vector3 t = new Vector3(0,0,0);

    [ContextMenu(nameof(SetLocalPositionAsOffset))]
    public void SetLocalPositionAsOffset() => positionOffset = transform.localPosition;

    public AnimationCurve xCurve = AnimationCurve.Linear(0,0,1,1);
    public AnimationCurve yCurve = AnimationCurve.Linear(0,0,1,1);
    public AnimationCurve zCurve = AnimationCurve.Linear(0,0,1,1);
    
    public void Update()
    {
        t += speed * Time.deltaTime;
        transform.localPosition = positionOffset + new Vector3(
	        timeOffset.x + xCurve.Evaluate(t.x) * magnitude.x,
	        timeOffset.y + yCurve.Evaluate(t.y) * magnitude.y,
	        timeOffset.z + zCurve.Evaluate(t.z) * magnitude.z);
    }
}
