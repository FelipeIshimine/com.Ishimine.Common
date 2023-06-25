using UnityEngine;

public static class CameraExtensions
{
    public static Vector3 GetWorldPositionOnPlane(this Camera camera, Vector3 screenPosition, float z) 
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        new Plane(Vector3.forward, new Vector3(0, 0, z)).Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }
}