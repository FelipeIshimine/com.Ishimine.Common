using UnityEngine;

public class DebugOnly : MonoBehaviour
{
    public void Awake()
    {
        if(!Debug.isDebugBuild)
            Destroy(gameObject);
    }
}