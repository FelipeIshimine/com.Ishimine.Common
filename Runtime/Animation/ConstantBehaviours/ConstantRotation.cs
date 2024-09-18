using UnityEngine;

public class ConstantRotation : ConstantBehaviour
{
    public Vector3 offset = new Vector3();
    public Vector3 axisSpeed = new Vector3(0,0,1);
    public AnimationCurve xCurve = AnimationCurve.Linear(0,0,1,1);
    public AnimationCurve yCurve = AnimationCurve.Linear(0,0,1,1);
    public AnimationCurve zCurve = AnimationCurve.Linear(0,0,1,1);

    public bool useLocalRotation;
    
    protected override void Process(float nTime)
    {
	    if (useLocalRotation)
	    {
		    transform.localRotation = Quaternion.Euler(
			    offset.x + xCurve.Evaluate(nTime * axisSpeed.x) * 360,
			    offset.y + yCurve.Evaluate(nTime * axisSpeed.y) * 360,
			    offset.z + zCurve.Evaluate(nTime * axisSpeed.z) * 360);
	    }
	    else
	    {
		    transform.rotation = Quaternion.Euler(
			    offset.x + xCurve.Evaluate(nTime * axisSpeed.x)*360,
			    offset.y + yCurve.Evaluate(nTime * axisSpeed.y)*360,
			    offset.z + zCurve.Evaluate(nTime * axisSpeed.z)*360);
	    }
        
    }
}
