using UnityEngine;
using Random = UnityEngine.Random;

        [ExecuteAlways]
public class ConstantOrbit : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 targetPosition;

    private Vector3 TargetPosition => targetTransform ? targetTransform.position : targetPosition;

    public Vector3 axisMultiplier;
    public float radius = 1;
    public float speedMultiplier = 1;
    public bool useRandomOffset = true;
	public Vector3 offset;
    
    private float t;

    [SerializeField] private bool showOnScene;

    private bool HasTargetTransform() => targetTransform;
    private void Awake()
    {
        if (useRandomOffset)
            offset = new Vector3(Random.Range(0, Mathf.PI), Random.Range(0, Mathf.PI), Random.Range(0, Mathf.PI));
        Step();
    }

    private void OnEnable()
    {
        t = 0;
    }

    
    public void Update()
    {
	    if (!showOnScene && !Application.isPlaying)
	    {
		    return;
	    }
        Step();
    }

    private void Step()
    {
        t += Time.deltaTime * speedMultiplier;
        var direction = new Vector3(Mathf.Sin(t + offset.x) * axisMultiplier.x, Mathf.Sin(t + offset.y) * axisMultiplier.y,
            Mathf.Sin(t + offset.z) * axisMultiplier.z) * radius;
        if (direction != Vector3.zero)
            this.transform.LookAt(new Vector3(direction.x, 0, direction.z));
        transform.position = TargetPosition + direction;
    }

    private void OnDrawGizmos()
    {
    }
}
