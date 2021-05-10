using UnityEngine;
[ExecuteInEditMode]
public class BackCameraFix : MonoBehaviour
{
    private void Awake()
    {
        Canvas canv = GetComponent<Canvas>();
        if (canv.worldCamera == null) canv.worldCamera = Camera.main;
    }
}