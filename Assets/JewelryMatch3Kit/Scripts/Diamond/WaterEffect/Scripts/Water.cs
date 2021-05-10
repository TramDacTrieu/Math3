using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityStandardAssets._2D;
// [ExecuteInEditMode]
public class Water : MonoBehaviour
{
    float[] xpositions;
    float[] ypositions;
    float[] ypositionsLine;

    public Material overlay;
    // List<GameObject> spriteObjects = new List<GameObject>();
    public Color color = new Color(0f, 0.5f, 1, 0.5f);
    public Color colorLine = new Color(1f, 1f, 1, 0.5f);
    List<SpriteVerticles> spriteLines = new List<SpriteVerticles>();

    public ParticleSystem bubbleParticles;
    float bottom;
    int edgecount = 10;
    private float height;
    public float scaleX = 0.5f;


    float vol = 1;
    public Texture2D tex;
    List<SpriteVerticles> spriteVerticles = new List<SpriteVerticles>();
    public RectTransform rectTransfrorm;
    private Vector3[] rectV;
    BoxCollider2D boxcollider;
    BuoyancyEffector2D buoyancyEffector2D;
    void Start()
    {
        if (Application.isPlaying)
        {
            // LevelSettings.THIS.CreateBorderMask("Water");
            LevelSettings.THIS.CreateBorderMask("Level", false, true);
        }
        gameObject.layer = LayerMask.NameToLayer("Water");
        if (rectTransfrorm.rect.height <= 0)
            rectTransfrorm.sizeDelta = new Vector2(rectTransfrorm.rect.width, 0.01f);
        SpawnWaterSprite(tex.width);
        if (boxcollider == null)
            boxcollider = gameObject.AddComponent<BoxCollider2D>();
        if (buoyancyEffector2D == null) buoyancyEffector2D = gameObject.GetComponent<BuoyancyEffector2D>();
        buoyancyEffector2D.surfaceLevel = height - 0.5f;
        boxcollider.offset = rectTransfrorm.rect.center;
        boxcollider.size = rectTransfrorm.rect.size;
        boxcollider.isTrigger = true;
        boxcollider.usedByEffector = true;
    }

    public void FillLevel(float volume)
    {
        StartCoroutine(Filling(volume));
    }
    IEnumerator Filling(float volume)
    {
        rectV = new Vector3[4];
        rectTransfrorm.GetWorldCorners(rectV);
        float Top = rectV[1].y - 5;
        float y = 0;
        if (Camera2DFollow.direction > 0)
            y = Mathf.Min(ItemPhysicsManger.LowerDinamicTreshold.y, Top);
        else
            y = Mathf.Max(ItemPhysicsManger.LowerDinamicTreshold.y, Top);
        ItemPhysicsManger.LowerDinamicTreshold = new Vector2(0, y);
        float filledVolume = 0;
        while (filledVolume < volume)
        {
            filledVolume += Time.deltaTime;
            rectTransfrorm.sizeDelta = new Vector2(rectTransfrorm.rect.width, rectTransfrorm.rect.height + Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnRectTransformDimensionsChange()
    {
        rectV = new Vector3[4];
        //        Rect rect = rectTransfrorm.GetWorldRect();
        rectTransfrorm.GetWorldCorners(rectV);
        height = (rectV[1].y - rectV[0].y);
        float Left = rectV[0].x;
        float Top = rectV[1].y;
        float width = (int)((rectV[3].x - rectV[0].x));
        Rect rect = new Rect(Left, Top, width, height);

        for (int i = 0; i < spriteVerticles.Count; i++)
        {
            var spriteObject = spriteVerticles[i].GameObject;
            Vector2 offsetX = Vector2.right * (spriteVerticles[i].globalvertices[0].x / tex.width) * scaleX;
            UpdatePosScale((Vector2)rectV[0] + offsetX, rect.height, spriteObject);
            spriteObject = spriteLines[i].GameObject;
            offsetX = Vector2.right * (spriteVerticles[i].globalvertices[0].x / tex.width) * scaleX;
            UpdatePosScale((Vector2)rectV[0] + offsetX, rect.height, spriteObject);
        }
        if (buoyancyEffector2D != null && boxcollider != null)
        {
            buoyancyEffector2D.surfaceLevel = rect.height - 0.5f;
            boxcollider.offset = rectTransfrorm.rect.center;
            boxcollider.size = rectTransfrorm.rect.size;
        }
        var scaleParticles = bubbleParticles.shape;
        scaleParticles.scale = rectTransfrorm.rect.size;
    }

    private void UpdatePosScale(Vector2 leftPos, float height, GameObject spriteObject)
    {
        spriteObject.transform.localScale = new Vector2(scaleX, height);
        spriteObject.transform.position = leftPos + Vector2.up * (spriteObject.transform.localScale.y / 2);
    }

    void UpdateSprite(List<SpriteVerticles> spriteVerticles, bool line)
    {
        for (int i = 0; i < spriteVerticles.Count; i++)
        {
            UpdateVertices(i, spriteVerticles[i].sprite, spriteVerticles[i].sprite.vertices, line);
        }
    }

    private void UpdateVertices(int i, Sprite sprite, Vector2[] vertices, bool line)
    {
        Vector2[] Vertices = new Vector2[4];
        var yPos = line == false ? ypositions : ypositionsLine;
        Vertices[3] = new Vector2(xpositions[i], yPos[i] * 1000 / vol);
        Vertices[1] = new Vector2(xpositions[i + 1], yPos[i + 1] * 1000 / vol);
        Vertices[0] = new Vector2(xpositions[i], bottom);
        Vertices[2] = new Vector2(xpositions[i + 1], bottom);
        Vector2[] sv = vertices;
        if (!line)
        {
            sv[0] = new Vector2(0, Mathf.Clamp(95 + Vertices[3].y, 0, 100));
            sv[1] = new Vector2(100, 0);
            sv[2] = new Vector2(100, Mathf.Clamp(95 + Vertices[1].y, 0, 100));
            sv[3] = new Vector2(0, 0);
        }
        else
        {
            sv[0] = new Vector2(0, Mathf.Clamp(95 + Vertices[3].y, 0, 100));
            sv[1] = new Vector2(100, Mathf.Clamp(95 + Vertices[1].y, 0, 100) - 15 / height);
            sv[2] = new Vector2(100, Mathf.Clamp(95 + Vertices[1].y, 0, 100));
            sv[3] = new Vector2(0, Mathf.Clamp(95 + Vertices[3].y, 0, 100) - 15 / height);
        }
        vertices = sv;
        sprite.OverrideGeometry(vertices, sprite.triangles);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < spriteVerticles.Count; i++)
        {
            ypositions[i] = Mathf.Sin(i + Time.time * 1) / (height * 100);
        }
        for (int i = 0; i < spriteVerticles.Count; i++)
        {
            ypositionsLine[i] = Mathf.Sin(i + Time.time * 2) / (height * 150);
        }
        UpdateSprite(spriteLines, true);
        UpdateSprite(spriteVerticles, false);
    }

    public void SpawnWaterSprite(float WidthSprite)
    {
        rectV = new Vector3[4];
        rectTransfrorm.GetWorldCorners(rectV);

        edgecount = Mathf.RoundToInt((rectV[3].x - rectV[0].x) / scaleX) + 1;
        height = (rectV[1].y - rectV[0].y);
        float Left = rectV[0].x;
        float Top = rectV[1].y;
        int nodecount = edgecount + 1;

        xpositions = new float[nodecount];
        ypositions = new float[nodecount];
        ypositionsLine = new float[nodecount];

        for (int i = 0; i < nodecount; i++)
        {
            ypositions[i] = Top;
            ypositionsLine[i] = Top;
            xpositions[i] = Left + WidthSprite * i;// / edgecount;
        }
        for (int i = 0; i < edgecount; i++)
        {
            var spriteObject = spriteLines.Addd(CreateSprite(height, i, colorLine, "water_line", "Level"));
            spriteObject.GameObject.GetComponent<SpriteRenderer>().material = overlay;
            spriteObject.GameObject.transform.parent = transform;
        }
        for (int i = 0; i < edgecount; i++)
        {
            var spriteObject = spriteVerticles.Addd(CreateSprite(height, i, color, "wave", "Water"));
            spriteObject.GameObject.transform.parent = transform;
            // CreateCollider(spriteObject.GameObject);
            xpositions[i] = spriteObject.GameObject.transform.position.x;
        }
        OnRectTransformDimensionsChange();
    }

    private SpriteVerticles CreateSprite(float height, int i, Color color, string name, string layer)
    {
        Vector2[] Vertices = new Vector2[4];
        Vertices[3] = new Vector2(xpositions[i], ypositions[i]);
        Vertices[1] = new Vector2(xpositions[i + 1], ypositions[i + 1]);
        Vertices[0] = new Vector2(xpositions[i], bottom);
        Vertices[2] = new Vector2(xpositions[i + 1], bottom);
        var spriteObject = new GameObject(name);
        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        var sr = spriteObject.AddComponent<SpriteRenderer>();
        sr.color = color;
        sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        sr.sortingLayerName = layer;

        Sprite o = sprite;
        Vector2[] sv = o.vertices;
        ushort[] triangles = new ushort[6];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 1;
        triangles[4] = 0;
        triangles[5] = 3;
        sv[0] = new Vector2(0, 100 - Vertices[1].y);
        sv[1] = new Vector2(100, 0);
        sv[2] = new Vector2(100, 100 - Vertices[3].y);
        sv[3] = new Vector2(0, 0);
        var sprVerticle = new SpriteVerticles { sprite = sprite, vertices = sv, globalvertices = Vertices, GameObject = spriteObject };
        sprite.OverrideGeometry(sv, triangles);
        spriteObject.GetComponent<SpriteRenderer>().sprite = sprite;
        Vector2 offsetX = /* i > 0 ? Vector2.right * (spriteVerticles[spriteVerticles.Count - 1].globalvertices[1].x / tex.width) * scaleX : */ Vector2.right * (Vertices[0].x / tex.width) * scaleX;
        // UpdatePosScale((Vector2)rectV[0] + offsetX, height, spriteObject);

        return sprVerticle;
    }

    private static void CreateCollider(GameObject spriteObject)
    {
        var coll = spriteObject.AddComponent<BoxCollider2D>();
        var bounce = spriteObject.AddComponent<BuoyancyEffector2D>();
        bounce.surfaceLevel = 0.4f;
        coll.usedByEffector = true;
        coll.isTrigger = true;
    }


}

class SpriteVerticles
{
    public Sprite sprite;
    public GameObject GameObject;
    public Vector2[] vertices = new Vector2[4];
    public Vector2[] globalvertices = new Vector2[4];
}
