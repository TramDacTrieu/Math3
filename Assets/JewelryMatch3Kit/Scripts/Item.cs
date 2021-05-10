using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using JewelryMatch3Kit.Scripts.Diamond;
using UnityEngine.UI;
using UnityEditor;


public enum ItemsTypes
{
    NONE = 0,
    VERTICAL_STRIPPED,
    HORIZONTAL_STRIPPED,
    PACKAGE,
    CHOCOBOMB,
    INGREDIENT,
    BOMB,
    FLOWER
}

public class Item : MonoBehaviour, IHittable, IPoolable
{
    public Sprite[] items;
    public Sprite[] itemsAnimation;
    public List<StripedItem> stripedItems = new List<StripedItem>();
    public Sprite[] packageItems;
    public Sprite[] ChocoBombItems;
    public Sprite[] bombItems;
    Sprite[] ingredientItems;
    public SpriteRenderer sprRenderer;
    public bool dragThis;
    public Vector3 mousePos;
    public Vector3 deltaPos;
    public Vector3 switchDirection;
    private Item switchItem;
    public bool falling;
    public ItemsTypes NextType = ItemsTypes.NONE;

    public ItemsTypes nextType
    {
        get
        {
            return NextType;
        }
        set
        {
            NextType = value;
        }
    }

    public ItemsTypes CurrentType = ItemsTypes.NONE;

    public ItemsTypes currentType
    {
        get
        {
            return CurrentType;
        }
        set
        {

            CurrentType = value;
        }
    }

    public ItemsTypes debugType = ItemsTypes.NONE;
    public int COLORView;
    //	private int COLOR;

    public int color
    {
        get
        {
            return GetComponent<IColorableComponent>().color;
        }
        set
        {
            GetComponentsInChildren<IColorableComponent>().ForEachY(i => i.SetColor(value));
        }
    }

    public Animator anim;
    public bool destroying;
    public bool appeared;
    public bool animationFinished;
    public bool justCreatedItem;
    private float xScale;
    private float yScale;
    public GameObject timerTextPrefab;
    Text timerText;
    public int bombTimer;
    public GameObject Selection;
    public GameObject bombSelection1;
    public GameObject light;
    public GameObject wickBurningPrefab;
    public bool awaken;
    public GameObject appearingEffect;
    private bool extraChecked;
    public Item item;

    private void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        saveSpritePosition = transform.GetChild(0).localPosition;
    }

    // Use this for initialization
    void OnEnable()
    {
        item = this;
        falling = true;
        id = GetInstanceID();
        name = "Item " + id;
        if (nextType != ItemsTypes.NONE)
        {
            debugType = nextType;
            currentType = nextType;
            nextType = ItemsTypes.NONE;
            falling = false;
        }

        xScale = transform.localScale.x;
        yScale = transform.localScale.y;
        if (currentType == ItemsTypes.INGREDIENT)
        {
            GameObject obj = Instantiate(Resources.Load("Prefabs/arrow_ingredients")) as GameObject;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(0.66f, -0.53f, 0);
            obj.transform.localScale = Vector3.one * 0.6f;

        }
    }

    private bool highlight;
    private bool stayLight;
    public int id;
    private Vector3 saveSpritePosition;

    public void SetHightlighted()
    {
        stayLight = true;
        if (!highlight)
            StartCoroutine(SetHightlightedCor());
    }

    IEnumerator SetHightlightedCor()
    {
        highlight = true;
        GetComponent<IHighlightableComponent>().SetSelected(true);
        while (stayLight)
        {
            yield return new WaitForFixedUpdate();
            stayLight = false;
            yield return new WaitForSeconds(0.1f);
        }

        highlight = false;
        light.SetActive(false);
    }

    public void AwakeItem()
    {
        awaken = true;
        GetComponent<IHighlightableComponent>().SetSelected(true);
    }

    public void SleepItem()
    {
        awaken = false;
        if (!anim)
            return;//1.3
        GetComponent<IHighlightableComponent>().SetSelected(false);
    }

    public void SetType(string tagType)
    {
        ChangeType(tagType);
    }

    public void ChangeType(string tagType, int stripedSubtype = -1)
    {
        if (stripedSubtype == -1)
            stripedSubtype = UnityEngine.Random.Range(0, 2);
        Physics2D.autoSimulation = false;
        GetComponent<CircleCollider2D>().enabled = false;
        var obj = ObjectPooler.Instance.GetPooledObject(tagType);
        obj.GetComponent<IPoolableComponent>().SetupFromPool(transform.position);
        if (tagType == "striped")
            obj.GetComponent<Striped>().SetType(stripedSubtype);
        obj.GetComponentsInChildren<IColorableComponent>().ForEachY(i => i.SetColor(color));
        Physics2D.autoSimulation = true;
        FinishedChangeType();
        // if (this != null)
        //   StartCoroutine(ChangeTypeCor());
    }

    public void FinishedChangeType()
    {
        if (nextType == ItemsTypes.PACKAGE)
            sprRenderer.sprite = packageItems[color];
        else if (nextType == ItemsTypes.CHOCOBOMB)
            sprRenderer.sprite = ChocoBombItems[0];
        else if (nextType == ItemsTypes.BOMB)
        {
            sprRenderer.sprite = bombItems[color];
            SetupBomb();
        }
        else if (nextType == ItemsTypes.FLOWER)
        {
            sprRenderer.sprite = itemsAnimation[color];
        }

        //     square.DestroyBlock();

        debugType = nextType;
        currentType = nextType;
        nextType = ItemsTypes.NONE;
        GetComponent<IDestroyableComponent>().DestroyAround();
        gameObject.SetActive(false);
    }

    void SetupBomb()
    {

        // anim.SetBool("package_idle", true);

        GameObject t = Instantiate(timerTextPrefab) as GameObject;
        t.transform.SetParent(transform);
        t.transform.localPosition = new Vector3(1.5f, -0.8f, 0);
        t.transform.localScale = Vector3.one;
        timerText = t.transform.GetChild(0).GetComponent<Text>();
        if (bombTimer <= 0)//1.3
            bombTimer = LevelManager.Instance.bombTimer;

        GameObject wickBurning = Instantiate(wickBurningPrefab) as GameObject;
        wickBurning.transform.SetParent(transform);
        wickBurning.transform.localPosition = new Vector3(0.69f, 0.85f, 0);
        wickBurning.transform.localScale = Vector3.one * 0.2f;
    }

    public void SetAnimationDestroyingFinished()
    {
        LevelManager.THIS.itemsHided = true;
        animationFinished = true;
    }

    #region Destroying

    public virtual void DestroyItem(bool showScore = false, string anim_name = "", bool explEffect = false, bool directly = false, Action callback = null)
    {
        if (destroying)
            return;
        // if (nextType != ItemsTypes.NONE) { ChangeType(); return; }
        if (this == null)
            return;
        destroying = true;
        //		square.item = null;

        if (this == null)
            return;

        if (gameObject.activeSelf)
            StartCoroutine(DestroyCor(showScore, anim_name, explEffect, directly, callback));


    }

    IEnumerator DestroyCor(bool showScore = false, string anim_name = "", bool explEffect = false, bool directly = false, Action callback = null)
    {
        yield return new WaitForSeconds(.1f);
        if (currentType == ItemsTypes.FLOWER)
        {
            var obj = ObjectPooler.Instance.GetPooledObject("flower");
            obj.SetActive(true);
            obj.GetComponent<FlowerFly>().Launch(transform.position);
        }

        if (LevelManager.THIS.limitType == LIMIT.TIME && transform.Find("5sec") != null)
        {
            GameObject FiveSec = transform.Find("5sec").gameObject;
            FiveSec.transform.SetParent(null);
#if UNITY_5
			FiveSec.GetComponent<Animation> ().clip.legacy = true;
#endif

            FiveSec.GetComponent<Animation>().Play("5secfly");
            Destroy(FiveSec, 1);
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                LevelManager.THIS.Limit += 5;
        }

        if (showScore)
            LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForItem, transform.position);

        if (destroying)
        {
            destroying = false;
            callback();
        }
    }
    #endregion

    public void OnGetFromPool(Vector2 pos)
    {
        transform.GetChild(0).localPosition = saveSpritePosition;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void Hit(Vector2 pos, Action callback)
    {
        DestroyItem(false, "", false, false, callback);
    }
}

[System.Serializable]
public class StripedItem
{
    public Sprite horizontal;
    public Sprite vertical;
}
