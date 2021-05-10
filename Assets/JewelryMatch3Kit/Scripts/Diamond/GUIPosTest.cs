using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class GUIPosTest : MonoBehaviour
{
    private void Update()
    {
        Vector3 localPosition = GetComponent<RectTransform>().position;
        Vector2 message = RectTransformUtility.WorldToScreenPoint(Camera.allCameras.First(j => j.name == "CameraUI"), localPosition);
        Debug.Log(Camera.main.ScreenToWorldPoint(message));
    }
}