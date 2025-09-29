using UnityEngine;

namespace Redcode.Awaiting.Demo
{
    public class AwaitRotateObjectExample : MonoBehaviour
    {
        private async void Start()
        {
            while (true)
            {
                await new WaitForUpdate();
                transform.Rotate(Vector3.up, 90f * Time.deltaTime);
            }
        }

    }
}