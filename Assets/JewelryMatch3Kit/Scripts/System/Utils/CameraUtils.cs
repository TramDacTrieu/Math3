using UnityEngine;

public static class CameraUtils
{

    public static Rect GetCameraWorldRect(this Camera cam)
    {
        Vector3 vector3 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 vector33 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Rect rect = new Rect(vector3.x, vector3.y, vector33.x - vector3.x, vector33.y - vector3.y);
        return rect;
    }

    public static Vector2 GetCameraWorldBottomBorder(this Camera cam)
    {
        Vector3 vector3 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 vector33 = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        // return new Rect(vector3.x, vector3.x, vector3.x - vector33.x, vector3.y - vector33.y);
        return vector3;
    }

    public static Texture2D RenderToTexture(this Camera cam, GameObject gameObject, int height, int width)
    {
        int w = width;
        int h = height;
        RenderTexture rt = new RenderTexture(w, h, 8, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Point;
#if UNITY_5_6_OR_NEWER
        rt.autoGenerateMips = false;
#else
                    rt.generateMips = false;
#endif
        rt.Create();
        // Camera cam = new GameObject().AddComponent<Camera>();
        // cam.backgroundColor = new Color(1, 1, 1, 0);
        // cam.clearFlags = CameraClearFlags.SolidColor;
        // cam.transform.position = gameObject.transform.position;
        // cam.transform.position -= new Vector3(0, 0, 10);
        // cam.orthographic = true;
        // cam.orthographicSize = width * (float)Screen.height / Screen.width * 0.5f;
        cam.targetTexture = rt;
        // cam.cullingMask = 1 << 31;
        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        int oldLayer = gameObject.layer;
        gameObject.layer = 31;
        cam.Render();
        cam.targetTexture = null;

        gameObject.layer = oldLayer;
        Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        // Destroy(cam.gameObject);
        // DestroyImmediate(rt);
        return tex;
    }
}