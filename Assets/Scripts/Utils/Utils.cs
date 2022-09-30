using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utils : MonoBehaviour
{
    public static Vector2 ScreenToWorld(Camera camera, Vector3 position) {
        position.z = camera.nearClipPlane;
        return camera.ScreenToWorldPoint(position);
    }

    
}
