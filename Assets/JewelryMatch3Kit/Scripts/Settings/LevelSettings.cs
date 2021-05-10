using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

[ExecuteInEditMode]
public class LevelSettings : MonoBehaviour
{
    public static LevelSettings THIS;
    public LevelScriptable LevelScriptable;
    public BoundsInt bounds;
    private GameSettingsScriptable gameSetting;

    private void OnEnable()
    {
        // if (!Application.isPlaying) gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        // else gameObject.GetComponent<PolygonCollider2D>().enabled = false;

        if (THIS == null)
        {
            THIS = this;
            LoadLevel();
        }


        // else Destroy(gameObject);
        // CreateBorderMask();
    }
    private void Start()
    {
        if (Application.isPlaying)
        {
            gameSetting = Resources.Load("Settings/GameSettingsScriptable") as GameSettingsScriptable;

            CreateBorderMask("Water", true, FindObjectOfType<Water>() != null);
        }
    }
    public void LoadLevel(int num = 0)
    {
        if (num == 0)
            LevelScriptable = Resources.Load("Levels/" + SceneManager.GetActiveScene().name) as LevelScriptable;
        else
            LevelScriptable = Resources.Load("Levels/level" + num) as LevelScriptable;
    }

    private void Update()
    {
        // CreateBorderMask();
    }


    public void UpdateCollider()
    {
        var path = GetPathCollider();
        for (int i = 1; i < path.Length; i++)
        {
            Debug.DrawLine(path[i - 1], path[i]);
        }

    }
    public void CreateBorderMask(string layer, bool enableSprite, bool enableMaks)
    {
        GameObject obj = GameObject.Find("WaterMask " + layer);
        if (obj == null)
        {
            obj = new GameObject();
            obj.name = "WaterMask " + layer;
            obj.AddComponent<SpriteRenderer>();
            SpriteMask spriteMask = obj.GetComponent<SpriteMask>();
            if (spriteMask == null)
            {
                spriteMask = obj.gameObject.AddComponent<SpriteMask>();
            }
            spriteMask.isCustomRangeActive = true;
            spriteMask.frontSortingLayerID = SortingLayer.NameToID(layer);
            spriteMask.frontSortingOrder = 6;
            spriteMask.backSortingLayerID = SortingLayer.NameToID(layer);
            spriteMask.backSortingOrder = -1;
        }
        obj.transform.localScale = Vector2.one * 100;
        // obj.transform.position = transform.position;
        // var tilemap = GameObject.Find("GridLevel/LevelGrid").GetComponent<Tilemap>();
        PolygonCollider2D edgeCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        Vector2[] vertices = /*GetVerticesAround(); */ /*GetSplinePoints();*/    GetComponent<EdgeCollider2D>().points;   /* GetSplinePointsBezier(); */;
        vertices = AddMorePoints(vertices);
        // Draw(vertices);
        var rect = GetRectVertices(vertices, transform);
        var tex = new Texture2D(256, 256);
        // tex = RenderToTexture(rect.width, rect.height, transform.position);
        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        UpdateSprite(vertices, rect.xMin, rect.yMin, sprite);
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = enableSprite;
        obj.GetComponent<SpriteMask>().enabled = enableMaks;
        obj.GetComponent<SpriteMask>().sprite = sprite;
        SpriteShapeRenderer spriteShapeController = GetComponent<SpriteShapeRenderer>();
        spriteRenderer.sortingLayerName = spriteShapeController.sortingLayerName;
        spriteRenderer.sortingOrder = spriteShapeController.sortingOrder;
        spriteRenderer.color = gameSetting.levelColor;
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        spriteShapeController.enabled = false;
        GetComponent<SpriteShapeController>().enabled = false;
        SetSpritePosition(obj, transform.position);
    }

    Vector2[] AddMorePoints(Vector2[] vertices)
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 1; i < vertices.Length; i++)
        {
            float distance = Vector2.Distance(vertices[i], vertices[i - 1]) / 1000f;
            for (int j = 0; j < distance; j++)
            {
                Vector2 newPoint = vertices[i - 1] + (vertices[i] - vertices[i - 1]) / distance * j;
                list.Add(transform.TransformPoint(newPoint));
            }
        }
        return list.ToArray();
    }

    private static void Draw(Vector2[] vertices2)
    {
        float l = .01f;

        foreach (var item in vertices2)
        {
            Debug.DrawLine(item + new Vector2(-l, l), item + new Vector2(l, -l));
            Debug.DrawLine(item + new Vector2(-l, -l), item + new Vector2(l, l));
        }

        for (int i = 0; i < vertices2.Length - 1; i++)
        {
            Vector2 item = vertices2[i];
            Debug.DrawLine(vertices2[i], vertices2[i + 1], Color.red);
            Debug.DrawLine(item + new Vector2(-l, l), item + new Vector2(l, -l));
            Debug.DrawLine(item + new Vector2(-l, -l), item + new Vector2(l, l));
        }
    }

    public Vector2 GetSpritePivot(Rect rect)
    {
        var pivotX = -rect.center.x / (rect.center.x - rect.xMin) / 2 + 0.5f;
        var pivotY = -rect.center.y / (rect.center.y - rect.yMin) / 2 + 0.5f;

        return new Vector2(pivotX, pivotY);
    }

    Rect GetRectVertices(Vector2[] vertices, Transform relativeTransform)
    {
        float minX = vertices.Min(i => i.x) * relativeTransform.localScale.x + relativeTransform.position.x;
        float minY = vertices.Min(i => i.y) * relativeTransform.localScale.y + relativeTransform.position.y;
        float maxX = vertices.Max(i => i.x) * relativeTransform.localScale.x + relativeTransform.position.x;
        float maxY = vertices.Max(i => i.y) * relativeTransform.localScale.y + relativeTransform.position.y;
        float width = (maxX - minX);
        float height = (maxY - minY);
        Rect rect = new Rect(minX, minY, width, height);
        return rect;
    }

    public Vector2[] GetVerticesAround()
    {
        List<Vector2> listRight = null;
        List<Vector2> listLeft = GetBorders(out listRight);
        listRight.Reverse();
        return listLeft.Concat(listRight).ToArray();
    }

    public Vector2[] GetPathCollider()
    {
        List<Vector2> list = new List<Vector2>();
        List<Vector2> listRight = null;
        List<Vector2> listLeft = GetBorders(out listRight);
        float tempX = 0;
        for (int i = 0; i < listLeft.Count; i += 1)
        {
            var hit = listLeft[i];
            var hit1 = listRight[i];
            float x = (hit1.x + hit.x) / 2;
            float y = hit.y;
            Vector2 point = new Vector2(Mathf.Lerp(tempX, x, y), y);
            list.Add(point);
            tempX = x;
        }
        list = list.Where((x, i) => i % 50 == 0).ToList();
        return list.ToArray();
    }

    private List<Vector2> GetBorders(out List<Vector2> listRight)
    {
        Vector2[] vertices = GetComponent<EdgeCollider2D>().points;
        // vertices = OffsetVertices(vertices, transform);
        var rect = GetRectVertices(vertices, transform);
        List<Vector2> list = new List<Vector2>();
        listRight = new List<Vector2>();
        for (float i = 0; i < rect.height; i += 0.1f)
        {
            var vMin = new Vector2(rect.xMin, rect.yMin + i);
            var vMax = new Vector2(rect.xMax, rect.yMin + i);
            var hit = Physics2D.Raycast(vMin + Vector2.left, Vector2.right, 100, 1 << LayerMask.NameToLayer("Border"));
            var hit1 = Physics2D.Raycast(vMax + Vector2.right, Vector2.left, 100, 1 << LayerMask.NameToLayer("Border"));
            if (hit.collider != null && hit1.collider != null)
            {
                list.Add(hit.point);
                listRight.Add(hit1.point);
            }
        }

        return list;
    }

    public Vector2[] GetPath()
    {
        Vector2[] vertices = GetComponent<EdgeCollider2D>().points;
        vertices = OffsetVertices(vertices, transform);
        var rect = GetRectVertices(vertices, transform);
        var center = rect.center;
        float scale = 0.01f;
        var listnew = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            listnew[i].x = vertices[i].x + (center.x - vertices[i].x) * scale;
            listnew[i].y = vertices[i].y + (center.y - vertices[i].y) * scale;
        }

        const int V = 10;
        IEnumerable<IGrouping<float, Vector2>> enumerable = vertices.GroupBy(i => (float)Math.Round(i.y, 2));
        IEnumerable<Vector2> enumerable1 = enumerable.Select(i => new Vector2(i.Average(xxx => xxx.x), i.Average(xxx => xxx.y)));
        var path = enumerable1
        .OrderBy(i => i.y)
        .Select((x, i) => new { Index = i, Value = x })
        .GroupBy(x => x.Index / V)
        .Select(x => x.Select(v => v.Value).ToList())
        .ToList();
        List<Vector2> list = new List<Vector2>();
        foreach (var item in path)
        {
            float x = (((item.Max(cc => cc.x) + item.Min(cc => cc.x)) / 2));
            float y = (item.Sum(cc => cc.y) / V);
            list.Add(new Vector2(x, y));
        }
        return list.ToArray();
    }

    private Vector2[] OffsetVertices(Vector2[] vertices, Transform relativeTransform)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = vertices[i].x * relativeTransform.localScale.x + relativeTransform.position.x;
            vertices[i].y = vertices[i].y * relativeTransform.localScale.y + relativeTransform.position.y;
        }
        return vertices;
    }

    void SetSpritePosition(GameObject obj, Vector3 pos)
    {
        var vertices = obj.GetComponent<SpriteRenderer>().sprite.vertices;
        var rect = GetRectVertices(vertices, transform);
        Vector2 center = rect.center;
        Vector3 offset = pos - obj.transform.position;
        obj.transform.position = transform.position - offset;
    }
    private void UpdateSprite(Vector2[] vertices, float minX, float minY, Sprite sprite)
    {
        Vector2[] vertices2 = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices2[i] = new Vector3((vertices[i].x + 128), (vertices[i].y + 128), 0);
        }
        Triangulator tr = new Triangulator(vertices2);
        int[] indices = tr.Triangulate();
        // indices = TriangulateConvexPolygon(vertices2).ToArray();
        ushort[] triangles = indices.Select(i => (ushort)i).ToArray();
        sprite.OverrideGeometry(vertices2, triangles);
    }

    Texture2D RenderToTexture(float height, float width, Vector2 pos)
    {
        int w = 1024;
        int h = 1024;
        RenderTexture rt = new RenderTexture(w, h, 8, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Point;
#if UNITY_5_6_OR_NEWER
        rt.autoGenerateMips = false;
#else
                    rt.generateMips = false;
#endif
        rt.Create();
        Camera cam = new GameObject().AddComponent<Camera>();
        cam.backgroundColor = new Color(1, 1, 1, 0);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = pos;
        cam.transform.position -= new Vector3(0, 0, 10);
        cam.orthographic = true;
        cam.orthographicSize = width * (float)Screen.height / Screen.width * 0.5f;
        cam.targetTexture = rt;
        cam.cullingMask = 1 << 31;
        RenderTexture oldRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        int oldLayer = gameObject.layer;
        gameObject.layer = 31;
        cam.Render();
        gameObject.layer = oldLayer;
        Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        DestroyImmediate(cam.gameObject);
        DestroyImmediate(rt);
        return tex;
    }



    void DrawDebugTriangles(Vector2[] vertices, int[] triangles)
    {
        for (int i = 0; i < triangles.Length - 1; i++)
        {
            Debug.DrawLine(vertices[triangles[i]], vertices[triangles[i + 1]], Color.red);
        }
    }

    Vector2[] GetSplinePoints()
    {
        SpriteShapeController spriteShapeController = GetComponent<SpriteShapeController>();
        int splinePointCount = spriteShapeController.spline.GetPointCount();
        Vector2[] vertices = new Vector2[splinePointCount];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = spriteShapeController.spline.GetPosition(i);
        }
        return vertices;

    }

    private Vector2[] GetSplinePointsBezier()
    {
        SpriteShapeController spriteShapeController = GetComponent<SpriteShapeController>();
        Vector2[] vertices = GetSplinePoints();
        for (int i = 0; i < vertices.Length; i++)
        {
            int nextIndex = SplineUtility.NextIndex(i, vertices.Length);
            var startPoint = spriteShapeController.spline.GetPosition(i);
            var endPoint = spriteShapeController.spline.GetPosition(nextIndex);
            var startTangent = spriteShapeController.spline.GetRightTangent(i);
            var endTangent = spriteShapeController.spline.GetLeftTangent(nextIndex);
            vertices[i] = BezierUtility.BezierPoint(startPoint, startTangent + startPoint, endTangent + endPoint, endPoint, i);
        }

        return vertices;
    }

    public static List<int> TriangulateConvexPolygon(Vector2[] convexHullpoints)
    {
        List<int> triangles = new List<int>();

        for (int i = 2; i < convexHullpoints.Length; i++)
        {
            int a = 0;
            int b = i - 1;
            int c = i;

            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        return triangles;
    }

    public void ComputeCollectables()
    {
        LevelScriptable.Collect = FindObjectsOfType<Diamond>().Length + FindObjectsOfType<Nested>().Length + FindObjectsOfType<NestedBig>().Length * 2 + FindObjectsOfType<Lock>().Length;

    }
}

public class Triangle
{
    public int v1;
    public int v2;
    public int v3;

    public Triangle(int a, int b, int c)
    {
        v1 = a;
        v2 = b;
        v3 = c;
    }
}

public class Triangulator
{
    private List<Vector2> m_points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}