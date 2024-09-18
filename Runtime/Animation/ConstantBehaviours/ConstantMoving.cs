using UnityEngine;

public class ConstantMoving : MonoBehaviour
{
    public float movingSpeed = 10;
    public bool vertical;

    private void Update()
    {
        if (vertical)
            transform.position += Vector3.up * movingSpeed;
        else
            transform.position += Vector3.right * movingSpeed;
    }
}
