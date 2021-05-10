using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class IHighlightableComponent : MonoBehaviour
{
    public bool selectable = true;
    GameObject lightObject;
    public GameObject lightPrefab;
    public GameObject lightWheelPrefab;
    public GameObject circleLightPrefab;
    public GameObject circleLightObject;
    GameObject lightWheel;
    public AnimationItem animation;
    public Sprite selectionOutline;
    public GameObject[] selectionOutlineObjects;
    Transform[] transforms;
    public SpriteRenderer[] spriteRenderers;
    int[] layers;
    private IExplodable IExplodable;
    public IColorableComponent IColorableComponent;
    public IDestroyableComponent IDestroyableComponent;
    public Rigidbody2D Rigidbody2D;
    public ItemSound ItemSound;
    public bool selected;
    public bool highlighted;

    public bool preSelected;
    public OrderKeeper layerKeeper;

    public MoveWave moveWave;

    private void Awake()
    {
        if (selectable)
        {
            animation = gameObject.AddComponent<AnimationItem>();
            animation.time = 0.2f;
            animation.scale = 1.2f;
        }
        ItemSound = GetComponent<ItemSound>();
        moveWave = gameObject.AddComponent<MoveWave>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        IExplodable = GetComponent<IExplodable>();
        IColorableComponent = GetComponent<IColorableComponent>();
        IDestroyableComponent = GetComponent<IDestroyableComponent>();
        if (selectable)
        {
            transform.GetChild(0).localScale = Vector2.one;
            CreateSelection();
        }
        CreateOutline();

        transforms = transform.FindAllChildrenIncludingInactive().Distinct().ToArray();
        spriteRenderers = transforms.Select(i => i.GetComponent<SpriteRenderer>()).WhereNotNull().ToArray();
        if (Rigidbody2D)
            spriteRenderers[0].gameObject.AddComponent<VisibilityListener>();
        layers = new int[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            layers[i] = spriteRenderers[i].sortingLayerID;
        }
        layerKeeper = new OrderKeeper(transform.gameObject, spriteRenderers);

    }

    private void OnEnable()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingLayerID = layers[i];
        }
        if (selectable)
        {
            transform.GetChild(0).localScale = Vector2.one;
            lightWheel?.SetActive(false);
            selectionOutlineObjects?.SetActiveAll(false);
        }
        SelectionManager.OnSelectionEnded += OnClearSelection;
        SelectionManager.OnSelectionStarted += OnBlurObject;
    }

    private void OnDisable()
    {
        SelectionManager.OnSelectionEnded -= OnClearSelection;
        SelectionManager.OnSelectionStarted -= OnBlurObject;
        SelectionManager.OnSelectionStarted -= ResetTip;

    }

    public void CreateSelection()
    {
        if (lightWheel == null && lightWheelPrefab != null)
        {
            lightWheel = Instantiate(lightWheelPrefab) as GameObject;
            lightWheel.transform.SetParent(transform.GetChild(0));
            lightWheel.name = "Wheel";
            lightWheel.transform.localPosition = Vector3.zero;
            lightWheel.transform.localScale = Vector3.one;
        }
    }

    private void CreateOutline()
    {
        if (selectionOutlineObjects.Length == 0 && selectable)
        {
            selectionOutlineObjects = new GameObject[2];
            for (int i = 0; i < 2; i++)
                selectionOutlineObjects[i] = CreateOutline(i);
        }

        if (circleLightObject == null && circleLightPrefab != null)
        {
            circleLightObject = Instantiate(circleLightPrefab);
            circleLightObject.transform.parent = transform.GetChild(0);
            circleLightObject.transform.localPosition = Vector2.zero;
            if (gameObject.tag == "nested")
            {
                circleLightObject.transform.localScale = Vector2.one * 0.65f;
            }
            circleLightObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            if (!tag.Contains("nested"))
                circleLightObject.SetActive(false);
        }

    }



    private GameObject CreateOutline(int index)
    {
        GameObject selectionOutlineObject = selectionOutlineObjects[index];
        if (selectionOutlineObject == null)
        {
            selectionOutlineObject = new GameObject();
            selectionOutlineObject.name = "selectionOutline";
            selectionOutlineObject.transform.parent = transform.GetChild(0);
            selectionOutlineObject.transform.localPosition = Vector2.zero;
            selectionOutlineObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            if (GetComponent<Potion>() != null) selectionOutlineObject.transform.localPosition = Vector2.up * 0.18f;
            selectionOutlineObject.transform.localScale = Vector2.one * 1.1f;
            if (gameObject.tag == "nested")
                selectionOutlineObject.transform.localScale = Vector2.one * 0.65f;
            var spr = selectionOutlineObject.AddComponent<SpriteRenderer>();
            spr.sprite = selectionOutline;
            spr.sortingOrder = 3;
        }
        return selectionOutlineObject;
    }

    private void OnBlurObject()
    {
        foreach (var item in spriteRenderers)
        {
            item.color = new Color(.5f, .5f, .5f);
        }
        // SetCircleOutline(false);
    }

    private void OnClearSelection()
    {
        foreach (var item in spriteRenderers)
        {
            item.color = Color.white;
        }
        SetLight(false);
        SetOutline(false);
        preSelected = false;
    }

    public void SetSelected(bool set)
    {
        if (set)
        {
            spriteRenderers.ForEachY(i => i.sortingLayerName = "Item1");
            animation.Selected(OnSelectionFinished);
            if (lightWheel.GetComponent<PlayableDirector>())
                lightWheel.GetComponent<PlayableDirector>().enabled = true;

        }
        else if (selected)
        {
            spriteRenderers.ForEachY(i => i.sortingLayerName = "Item2");
            animation.DeSelected(OnDeselectionFinished);
        }
        if (selectionOutlineObjects.Length > 1)
            selectionOutlineObjects[1]?.SetActive(set);
        lightWheel?.SetActive(set);
        selected = set;
    }

    public void SetOutline(bool set)
    {
        // lightObject.SetActive(set);
        if (selectionOutlineObjects.Length > 0)
            selectionOutlineObjects[0]?.SetActive(set);
        if (!set)
            highlighted = set;
    }
    bool tip;

    public void SetTip()
    {
        SelectionManager.OnSelectionStarted += ResetTip;
        if (!tip)
            StartCoroutine(TipAnim());
    }

    IEnumerator TipAnim()
    {
        tip = true;
        selectionOutlineObjects[0]?.SetActive(true);
        var sr = selectionOutlineObjects[0].GetComponent<SpriteRenderer>();
        // while (true)
        {
            float t = 0;
            while (t < 1)
            {
                float a = Mathf.Lerp(0f, 1, t += Time.deltaTime);
                sr.color = new Color(1, 1, 1, a);
                yield return new WaitForEndOfFrame();
            }
            t = 0;
            while (t < 1)
            {
                float a = Mathf.Lerp(1, 0f, t += Time.deltaTime);
                sr.color = new Color(1, 1, 1, a);
                yield return new WaitForEndOfFrame();
            }

        }
        ResetTip();
    }

    void ResetTip()
    {
        StopCoroutine(TipAnim());
        selectionOutlineObjects[0].GetComponent<SpriteRenderer>().color = Color.white;
        selectionOutlineObjects[0]?.SetActive(false);
        SelectionManager.OnSelectionStarted -= ResetTip;
        tip = false;
    }

    public void SetLight(bool set)
    {
        if (circleLightObject != null && !tag.Contains("nested"))
            circleLightObject.SetActive(set);

    }

    #region Animations

    public void OnSelectionFinished()
    {
        spriteRenderers.ForEachY(i => i.sortingLayerName = "Item3");

    }
    public void OnDeselectionFinished()
    {
        spriteRenderers.ForEachY(i => i.sortingLayerName = "Default");

    }
    #endregion

}
