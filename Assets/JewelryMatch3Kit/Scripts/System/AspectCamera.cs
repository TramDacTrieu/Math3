using UnityEngine;
[ExecuteInEditMode]
public class AspectCamera : MonoBehaviour
{
    public Sprite map;
    void Start()
    {
        Camera.main.orthographicSize = map.bounds.size.x / Screen.width * Screen.height / 2;
    }
}