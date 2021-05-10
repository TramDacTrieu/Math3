//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.U2D;
//using UnityEngine.Tilemaps;
//using System;

//namespace UnityEditor
//{
//    [CreateAssetMenu]
//    [CustomGridBrush(false, false, true, "Item Prefab Brush")]
//    public class PrefabBrush : GridBrushBase
//    {
//        private const float k_PerlinOffset = 100000f;
//        public GameObject[] m_Prefabs;
//        public GameObject m_Prefab;
//        public float m_PerlinScale = 0.5f;
//        public int m_Z;
//        public int color;
//        private GameObject levelObject;
//        private PolygonCollider2D collider;

//        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
//        {
//            // Do not allow editing palettes
//            if (brushTarget.layer == 31)
//                return;
////            int index = Mathf.Clamp(Mathf.FloorToInt(GetPerlinValue(position, m_PerlinScale, k_PerlinOffset) * m_Prefabs.Length), 0, m_Prefabs.Length - 1);
//            var pos = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z)));
//            GameObject prefab = m_Prefab;
//            Erase(grid, brushTarget, position);

//            bool nonItemPrefab = prefab.GetComponent<PrefabIcon>() != null;
//            if (!prefab.tag.Contains("wire") && !nonItemPrefab)
//            {
//                if (!PointInPolygon(pos, prefab.GetComponent<Collider2D>())) return;
//            }
//            GameObject instance = null;
//            if (!nonItemPrefab)
//                instance = ObjectPooler.Instance.GetPooledObject(prefab.tag); // (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//            if (nonItemPrefab)
//                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//            if (instance != null)
//            {
//                Transform parentSave = instance.transform.parent;
//                instance.transform.parent = null;
//                IColorableComponent[] colorableComponent = instance.GetComponentsInChildren<IColorableComponent>();
//                if (color > -1 && colorableComponent != null)
//                {
//                    foreach (var item in colorableComponent)
//                    {
//                        item.RandomColorOnAwake = false;
//                        item.SetColor(color);
//                    }
//                }
//                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
//                Undo.RegisterCreatedObjectUndo((UnityEngine.Object)instance, "Paint Prefabs");
//                if (nonItemPrefab)
//                {
//                    instance.transform.SetParent(GameObject.Find("LevelObjects").transform);
//                    instance.transform.position = pos;
//                    Tools.current = UnityEditor.Tool.Rect;
//                    if (!Application.isPlaying)
//                    {
//                        instance = PrefabUtility.ConnectGameObjectToPrefab(instance, prefab);
//                    }
//                    Selection.activeObject = instance;

//                }
//                else
//                {
//                    instance.transform.SetParent(parentSave);
//                    instance.GetComponent<IPoolableComponent>().SetupFromPool(pos);
//                    if (instance.GetComponent<Diamond>() != null || instance.GetComponent<Nested>() != null || instance.GetComponent<NestedBig>() != null) LevelSettings.THIS.ComputeCollectables();
//                }
//            }
//        }

//        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
//        {
//            // Do not allow editing palettes
//            if (brushTarget.layer == 31)
//                return;

//            Transform erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
//            if (erased != null)
//            {
//                if (erased.GetComponent<Diamond>() != null || erased.GetComponent<Nested>() != null || erased.GetComponent<NestedBig>() != null) LevelSettings.THIS.ComputeCollectables();
//                Undo.DestroyObjectImmediate(erased.gameObject);
//            }
//        }

//        private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
//        {

//            int childCount = parent.childCount;
//            Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
//            var hit = Physics2D.OverlapPoint(min,
//             (1 << LayerMask.NameToLayer("Item") | 1 << LayerMask.NameToLayer("Objects")));
//            if (hit) return hit.gameObject.transform;
//            Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
//            Bounds bounds = new Bounds((max + min) * .5f, max - min);

//            for (int i = 0; i < childCount; i++)
//            {
//                Transform child = parent.GetChild(i);
//                if (bounds.Contains(child.position))
//                    return child;
//            }
//            return null;
//        }

//        private static float GetPerlinValue(Vector3Int position, float scale, float offset)
//        {
//            return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
//        }

//        public bool PointInPolygon(Vector2 point, Collider2D prefabCollider)
//        {
//            bool inside = false;
//            if (levelObject == null)
//                levelObject = GameObject.FindObjectOfType<LevelSettings>().gameObject;
//            if (collider == null)
//                collider = levelObject.GetComponent<PolygonCollider2D>();
//            if (prefabCollider.GetType() == typeof(CircleCollider2D))
//            {
//                var newPoint = point + Vector2.left * ((CircleCollider2D)prefabCollider).radius / 1.5f;
//                inside = collider.OverlapPoint(newPoint);
//                if (!inside) return false;
//                newPoint = point + Vector2.right * ((CircleCollider2D)prefabCollider).radius / 1.5f;
//                inside = collider.OverlapPoint(newPoint);
//                if (!inside) return false;
//                newPoint = point + Vector2.up * ((CircleCollider2D)prefabCollider).radius / 1.5f;
//                inside = collider.OverlapPoint(newPoint);
//                if (!inside) return false;
//                newPoint = point + Vector2.down * ((CircleCollider2D)prefabCollider).radius / 1.5f;
//                inside = collider.OverlapPoint(newPoint);
//                if (!inside) return false;
//            }
//            else if (prefabCollider.GetType() == typeof(PolygonCollider2D))
//            {
//                var points = ((PolygonCollider2D)prefabCollider).points;
//                for (int i = 0; i < points.Length; i++)
//                {
//                    var newPoint = point + points[i];
//                    inside = collider.OverlapPoint(newPoint);
//                    if (!inside) return false;
//                }
//            }
//            return inside;
//        }
//    }

//    [CustomEditor(typeof(PrefabBrush))]
//    public class PrefabBrushEditor : GridBrushEditorBase
//    {
//        private PrefabBrush prefabBrush { get { return target as PrefabBrush; } }

//        private SerializedProperty m_Prefab;
//        private Texture2D texturePressed;
//        private SerializedObject m_SerializedObject;
//        private List<ItemEditorIcon> prefabs;
//        private List<PrefabIcon> otherPrefabs;

//        colors color;

//        Texture2D randomBall;

//        protected void OnEnable()
//        {
//            m_SerializedObject = new SerializedObject(target);
//            m_Prefab = m_SerializedObject.FindProperty("m_Prefab");

//            texturePressed = GetPressedTexture();
//            LoadPrefabs();
//        }

//        public override void OnToolActivated(GridBrushBase.Tool tool)
//        {
//            Selection.activeObject = GameObject.FindObjectOfType<Grid>();
//        }

//        private Texture2D GetPressedTexture()
//        {
//            var texturePressed = new Texture2D(50, 50);
//            var fillColor = new Color(0, 0, 0, 0.5f);
//            var fillColorArray = texturePressed.GetPixels();

//            for (var c = 0; c < fillColorArray.Length; ++c)
//            {
//                fillColorArray[c] = fillColor;
//            }

//            texturePressed.SetPixels(fillColorArray);

//            texturePressed.Apply();
//            return texturePressed;
//        }

//        private void LoadPrefabs()
//        {
//            var list = Resources.LoadAll("Prefabs/GameItems/").Select(i => (GameObject)i).ToArray();
//            prefabs = new List<ItemEditorIcon>();
//            foreach (var item in list)
//            {
//                if (item.layer != LayerMask.NameToLayer("Item")) continue;
//                ItemEditorIcon item1 = new ItemEditorIcon();
//                item1.GameObject = item;
//                item1.SpriteRenderers = item.transform.GetComponentsInChildren<SpriteRenderer>().OrderBy(i => i.sortingOrder).ToArray();
//                prefabs.Add(item1);
//            }
//            otherPrefabs = new List<PrefabIcon>();
//            list = Resources.LoadAll("Prefabs/DesignItems/").Select(i => (GameObject)i).ToArray();
//            foreach (var item in list)
//            {
//                otherPrefabs.Add(item.GetComponent<PrefabIcon>());
//            }
//        }

//        public override void OnPaintInspectorGUI()
//        {
//            OnGUI();
//        }

//        private void OnGUI()
//        {
//            if (texturePressed == null) texturePressed = GetPressedTexture();
//            if (Application.isPlaying) return;
//            if (prefabs == null) return;
//            GUILayout.BeginArea(new Rect(0, 0, 1000, 1000));
//            GUILayout.Space(30);
//            color = (colors)EditorGUILayout.EnumPopup(color, GUILayout.Width(100));
//            prefabBrush.color = (int)color;
//            GUILayout.BeginVertical();
//            {
//                ShowColorItems();
//                GUILayout.Space(10);
//                // GUILayout.FlexibleSpace();
//                ShowNoColorItems();
//                GUILayout.Space(10);
//                ShowOtherPrefabs();
//            }
//            GUILayout.EndVertical();
//            GUILayout.EndArea();
//        }

//        private void ShowOtherPrefabs()
//        {
//            int length = otherPrefabs.Count();

//            GUILayout.BeginHorizontal();
//            {
//                for (int i = 0; i < length; i++)
//                {
//                    PrefabIcon itemEditorIcon = otherPrefabs[i];
//                    if (GUILayout.Button(new GUIContent("", itemEditorIcon.gameObject.name), GUILayout.Width(50), GUILayout.Height(50)))
//                    {
//                        Selection.activeObject = GameObject.FindObjectOfType<Grid>();
//                        prefabBrush.m_Prefab = itemEditorIcon.gameObject;
//                    }
//                    var lastRect = GUILayoutUtility.GetLastRect();
//                    GUI.DrawTexture(lastRect, itemEditorIcon.icon);
//                    if (prefabBrush.m_Prefab == itemEditorIcon.gameObject)
//                    {
//                        GUI.DrawTexture(lastRect, texturePressed);
//                    }
//                }
//            }
//            GUILayout.EndHorizontal();
//        }

//        private void ShowNoColorItems()
//        {
//            int length = prefabs.Count();
//            GUILayout.BeginHorizontal();
//            {
//                for (int i = 0; i < length; i++)
//                {
//                    ItemEditorIcon itemEditorIcon = prefabs[i];
//                    if (itemEditorIcon.GameObject.GetComponent<IColorEditor>() != null) continue;
//                    if (GUILayout.Button(new GUIContent("", itemEditorIcon.GameObject.name), GUILayout.Width(50), GUILayout.Height(50)))
//                    {
//                        var spriteShaperController = GameObject.FindObjectOfType<LevelSettings>().GetComponent<SpriteShapeController>();
//                        if (!spriteShaperController.autoUpdateCollider)
//                            spriteShaperController.BakeCollider();
//                        Selection.activeObject = GameObject.FindObjectOfType<Grid>();
//                        // GameObject.FindObjectOfType<LevelSettings>().UpdateCollider();
//                        prefabBrush.m_Prefab = itemEditorIcon.GameObject;
//                    }
//                    var lastRect = GUILayoutUtility.GetLastRect();
//                    DrawLayeredTextures(itemEditorIcon, itemEditorIcon.texPosList.ToArray(), lastRect, itemEditorIcon.GameObject.transform);
//                    if (prefabBrush.m_Prefab == itemEditorIcon.GameObject)
//                    {
//                        GUI.DrawTexture(lastRect, texturePressed);
//                    }
//                }
//            }
//            GUILayout.EndHorizontal();
//        }
//        private void ShowColorItems()
//        {
//            int columns = 5;
//            var list = prefabs.Where(i => i.GameObject.GetComponent<IColorEditor>() != null).OrderBy(i => i.texPos.order).ToArray();
//            GUILayout.BeginVertical();
//            {
//                for (int i = 0; i < list.Length; i++)
//                {
//                    GUILayout.BeginHorizontal();
//                    {
//                        for (int j = 0; j < columns; j++)
//                        {
//                            if (i + j >= list.Length) break;
//                            ItemEditorIcon itemEditorIcon = list[i + j];
//                            if (itemEditorIcon.GameObject.GetComponent<IColorEditor>() == null) continue;
//                            if (GUILayout.Button(new GUIContent("", itemEditorIcon.GameObject.name), GUILayout.Width(50), GUILayout.Height(50)))
//                            {
//                                var spriteShaperController = GameObject.FindObjectOfType<LevelSettings>().GetComponent<SpriteShapeController>();
//                                if (!spriteShaperController.autoUpdateCollider)
//                                    spriteShaperController.BakeCollider();
//                                // GameObject.FindObjectOfType<LevelSettings>().UpdateCollider();
//                                Selection.activeObject = GameObject.FindObjectOfType<Grid>();
//                                prefabBrush.m_Prefab = itemEditorIcon.GameObject;
//                                prefabBrush.color = (int)color;
//                            }
//                            var lastRect = GUILayoutUtility.GetLastRect();
//                            DrawLayeredTextures(itemEditorIcon, itemEditorIcon.texPosList.ToArray(), lastRect, itemEditorIcon.GameObject.transform);
//                            if (prefabBrush.m_Prefab == itemEditorIcon.GameObject)
//                            {
//                                GUI.DrawTexture(lastRect, texturePressed);
//                            }
//                        }
//                        i += columns - 1;
//                    }
//                    GUILayout.EndHorizontal();
//                }
//            }
//            GUILayout.EndVertical();
//        }

//        private void DrawLayeredTextures(ItemEditorIcon itemEditorIcon, TexPos[] texPosList, Rect lastRect, Transform parentObj)
//        {
//            Rect newRect = lastRect;
//            // if (texPos.Texture2D != null)
//            foreach (var item in itemEditorIcon.SpriteRenderers)
//            {

//                var texPos = item;
//                var centerButton = lastRect.center;
//                float x = lastRect.position.x + texPos.transform.localPosition.x;
//                float y = lastRect.position.y + texPos.transform.localPosition.y;
//                float width = (texPos.sprite.texture.width * texPos.transform.lossyScale.x);
//                width = Mathf.Clamp(width * 60 / 300, 0, 60);
//                float height = (texPos.sprite.texture.height * texPos.transform.lossyScale.y);
//                height = Mathf.Clamp(height * 60 / 300, 0, 60);
//                Rect rect = new Rect(x, y, width, height);

//                var texCenter = rect.center;
//                var offset = centerButton - texCenter;

//                float localX = texPos.transform.localPosition.x * 20;
//                float localY = texPos.transform.localPosition.y * 20;
//                if (item.transform == parentObj)
//                {
//                    localX = 0;
//                    localY = 0;
//                }
//                x += offset.x - localX;
//                y += offset.y - localY;


//                newRect = new Rect(x, y, width, height);
//                Texture2D texture1 = texPos.sprite.texture;
//                if (texPos.GetComponent<IColorEditor>() != null)
//                {
//                    if ((int)color < texPos.GetComponent<IColorEditor>()?.Sprites.Length && (int)color > -1)
//                        texture1 = texPos.GetComponent<IColorEditor>()?.Sprites[(int)color].texture;
//                    if (color == colors.Random)
//                        texture1 = texPos.GetComponent<IColorEditor>()?.randomEditorSprite.texture;
//                }

//                GUI.DrawTexture(newRect, texture1);
//            }
//            // for (int i = 0; i < texPosList.Length; i++)
//            // {
//            //     DrawTexRecursevly(newRect, parentObj, texPosList[i]);
//            // }
//        }

//        struct TexPos
//        {
//            public IColorEditor colorComponent;
//            public int order;
//            public Texture2D Texture2D;
//            public Texture2D Texture2DRandom;
//            public Vector2 pos;
//            public Vector2 scale;
//            public List<TexPos> chidlren;

//        }
//        class ItemEditorIcon
//        {
//            public TexPos texPos;
//            public SpriteRenderer[] SpriteRenderers;
//            public List<TexPos> texPosList = new List<TexPos>();
//            public GameObject GameObject;
//        }

//        enum colors
//        {
//            Random = -1,
//            Color1 = 0,
//            Color2 = 1,
//            Color3 = 2,
//            Color4 = 3,
//            Color5 = 4,

//        }
//    }
//}
