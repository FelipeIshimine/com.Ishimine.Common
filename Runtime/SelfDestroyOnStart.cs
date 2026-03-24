using UnityEngine;

public class SelfDestroyOnStart : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject);
    }
}