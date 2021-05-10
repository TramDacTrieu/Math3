using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets._2D;

public class SelectionManager : MonoBehaviour
{
    private Vector2 startPosTouch;
    private bool touchBegin;
    private Vector2 directionTouch;
    private Vector2 currentPos;
    private bool touchMouseBegin;
    private Vector3 startPosMouse;
    private Camera _camera;
    private Camera2DFollow camera2dFollow;
    private List<IHighlightableComponent> highlightedList;
    // private IEnumerable<IHighlightableComponent> itemList;
    private List<IHighlightableComponent> preSelectedList;
    private LineManager lineManager;
    List<IExplodable> specialItems = new List<IExplodable>();
    List<IDestroyableComponent> gatheredTypes;
    List<IHighlightableComponent> specialHighlighed = new List<IHighlightableComponent>();
    private TouchCounter touch_counter;
    public int selectedColor;
    private Vector2 inputPos;
    private Vector2 directPos;
    public TouchBlocker touchBlocker;

    public delegate void TouchDetect();
    public static event TouchDetect OnSelectionStarted;
    public static event TouchDetect OnSelectionEnded;

    private void Start()
    {
        _camera = Camera.main;
        camera2dFollow = _camera.GetComponent<Camera2DFollow>();
        lineManager = new LineManager();
        gatheredTypes = LevelManager.THIS.gatheredTypes;
        touchBlocker = gameObject.AddComponent<TouchBlocker>();

    }

    private void Update()
    {
        if (LevelManager.THIS?.gameStatus == GameState.Playing && !touchBlocker.blocked)
        {
            if (GetTouchPosition(ref currentPos, ref directionTouch, ref startPosTouch))
            {
                Vector2 newPos = _camera.ScreenToWorldPoint(currentPos);
                if (newPos == inputPos) return;
                inputPos = newPos;
                if (LevelManager.THIS.lastDraggedItem != null && Vector2.Distance(LevelManager.THIS.lastDraggedItem.transform.position, inputPos) > 10 || LevelManager.THIS.gameStatus != GameState.Playing)
                {
                    LevelManager.THIS.lastDraggedItem = null;
                    ClearSelection(); return;
                }
                var startPos = _camera.ScreenToWorldPoint(startPosTouch);
                directPos = inputPos + directionTouch.normalized * .7f;
                var list = Physics2D.LinecastAll(startPos, directPos, 1 << LayerMask.NameToLayer("Item"));
                for (int i = 0; i < list.Length; i++)
                {
                    var item = list[i].collider;
                    IHighlightableComponent highlightableComponent = item.GetComponent<IHighlightableComponent>();
                    if (!highlightableComponent) continue;
                    if (!highlightableComponent?.selectable ?? false) continue;
                    if (LevelManager.THIS.ActivatedBoost.type == BoostType.Hammer)
                    {
                        LevelManager.THIS.ActivatedBoost.Activate(highlightableComponent.GetComponent<IDestroyableComponent>());
                        LevelManager.THIS.ActivatedBoost = null;
                        ItemPhysicsManger.LowerDinamicTreshold = highlightableComponent.transform.position + Vector3.down * 3 * Camera2DFollow.direction;
                        return;
                    }
                    var iHighlightable = highlightableComponent;
                    if (!iHighlightable.preSelected && selectedColor > -1) continue;
                    if (iHighlightable.highlighted) OnAnyItemSelected(iHighlightable);
                    else
                        OnNewSelected(iHighlightable);
                }
            }
        }
        if (LevelManager.THIS.gameStatus != GameState.Playing && touch_counter != null)
        {
            ClearSelection(); return;
        }
    }

    private void OnStartSelection(Vector2 position)
    {
        position = _camera.ScreenToWorldPoint(position);
        highlightedList = new List<IHighlightableComponent>();
        preSelectedList = new List<IHighlightableComponent>();
        var itemCollider = Physics2D.OverlapPoint(position, 1 << LayerMask.NameToLayer("Item"));
        if (itemCollider != null && itemCollider.GetComponent<IHighlightableComponent>() && (bool)itemCollider.GetComponent<IHighlightableComponent>()?.selectable)
        {
            // HighlightManager.BlurOtherObjectsAll(FindObjectsOfType<IHighlightableComponent>() /* camera2dFollow.GetVisibleItems().Select(i => i.IHighlightableComponent).ToArray() */, new Color(0.5f, 0.5f, 0.5f));
            OnSelectionStarted?.Invoke();
            IColorableComponent item = itemCollider.GetComponent<IColorableComponent>();
            selectedColor = item.color;
            touch_counter = ObjectPooler.Instance.GetPooledObject("touch_counter", true, true).GetComponent<TouchCounter>();
            touch_counter.transform.position = position + Vector2.up * 3.5f;
            FillPreSelectedList(item);
        }

    }

    private void OnAnyItemSelected(IHighlightableComponent iHighlightable)
    {
        if (iHighlightable.GetComponent<Collider2D>().OverlapPoint(inputPos) && LevelManager.THIS.lastDraggedItem != iHighlightable.IColorableComponent)
        {
            var onlySpecialItems = specialHighlighed.Except(preSelectedList).ToList();
            var onlySpecialItemsExceptSelected = specialHighlighed.Except(highlightedList).ToList();
            HighlightManager.WhiteOtherObjects(onlySpecialItems.ToArray(), new Color(0.5f, 0.5f, 0.5f));
            onlySpecialItemsExceptSelected.ForEach(i => i?.SetOutline(false));
            specialHighlighed.Clear();
            foreach (var specialItem in specialItems)
            {
                specialHighlighed.AddRange(specialItem.GetDestroyItems(iHighlightable.transform.position).Distinct());
            }

            specialHighlighed.ForEach(i => i?.SetOutline(true));
            HighlightManager.WhiteOtherObjects(specialHighlighed.ToArray(), Color.white);
            touch_counter.transform.position = (Vector3)iHighlightable.transform.position + Vector3.up * 3.5f;

            if (LevelManager.THIS.lastDraggedItem != null)
                LevelManager.THIS.lastDraggedItem.IHighlightableComponent.SetSelected(false);
            LevelManager.THIS.lastDraggedItem = iHighlightable.IColorableComponent;
            LevelManager.THIS.lastDraggedItem.IHighlightableComponent.SetSelected(true);
        }
    }

    private void OnNewSelected(IHighlightableComponent iHighlightable)
    {
        iHighlightable.ItemSound.SelectionSound(highlightedList.Count);
        if (selectedColor == -1 && iHighlightable.IColorableComponent.color > -1)
        {
            FillPreSelectedList(iHighlightable.IColorableComponent);
            selectedColor = iHighlightable.IColorableComponent.color;
        }
        bool reachedBonusCount = highlightedList.Count % LevelManager.THIS.extraItemEvery == 0;
        if (iHighlightable.IColorableComponent.color > -1)
            touch_counter.SetColor(iHighlightable.IColorableComponent.color);
        lineManager.AddPoint(iHighlightable, highlightedList.LastOrDefault()?.transform.position, highlightedList);
        touch_counter.NextType(highlightedList.Count, reachedBonusCount);
        touch_counter.UpdateCount(highlightedList.Count);
        IExplodable explodable = iHighlightable.GetComponent<IExplodable>();
        if (explodable != null && !specialItems.Contains(explodable))
            specialItems.Add(explodable);

        IDestroyableComponent bombItem = iHighlightable.GetComponent<IDestroyableComponent>();
        if (iHighlightable.GetComponent<ISpecial>() != null && !gatheredTypes.Contains(bombItem))
            gatheredTypes.Add(bombItem);
    }

    private void OnEndSelection()
    {
        var list = highlightedList/* .Concat(specialHighlighed) */.WhereNotNull();
        if (list.Count() > 2)
        {
            LevelManager.THIS.destroyAnyway = list.Select(i => i.GetComponent<IDestroyableComponent>()).ToList();
            LevelManager.THIS.FindMatches();
            ItemPhysicsManger.LowerDinamicTreshold = highlightedList
            .Concat(specialHighlighed)
            .WhereNotNull()
            .OrderBy(i => i.transform.position.y)
            .First()
            .transform.position + Vector3.down * 3 * Camera2DFollow.direction;

            LevelManager.THIS.Limit--;
            LevelManager.THIS.moveID++;
        }
        ClearSelection();
    }

    private void ClearSelection()
    {
        highlightedList.WhereNotNull().ForEachY(i =>
        {
            i.SetOutline(false);
            i.SetSelected(false);
        });
        // HighlightManager.BlurOtherObjectsAll(highlightedList.ToArray(), Color.white);
        OnSelectionEnded?.Invoke();
        // specialHighlighed.ForEach(i => i?.SetHighlighted(false));
        // Camera.main.GetComponent<Camera2DFollow>().GetVisibleItems().ForEachY(i => i?.IHighlightableComponent.SetHighlighted(false));
        specialHighlighed.Clear();
        highlightedList.Clear();
        lineManager.Clear();
        specialItems.Clear();
        gatheredTypes.Clear();
        touch_counter?.gameObject.SetActive(false);

    }

    private void FillPreSelectedList(IColorableComponent iHighlightable)
    {
        var colorableComponent = HighlightManager.GetNearMatches(iHighlightable).ForEachY(i => i.IHighlightableComponent.preSelected = true);
        preSelectedList.AddRange(colorableComponent.Select(i => i.GetComponent<IHighlightableComponent>()));
        if (preSelectedList.Count > 0)
            HighlightManager.WhiteOtherObjects(preSelectedList.ToArray(), Color.white);
    }
    bool GetTouchPosition(ref Vector2 currentPos, ref Vector2 directionTouch, ref Vector2 startPosTouch)
    {
        touchBegin = false;
        // currentPos = Vector2.zero;
        {
            Touch touch = new Touch();

            // if (Input.GetMouseButton(0))
            if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return false;
                    touch.tapCount = 1;
                    touch.fingerId = 1;
                    touch.phase = TouchPhase.Began;
                    touchMouseBegin = true;
                    startPosMouse = Input.mousePosition;
                    touch.position = startPosMouse;
                }
                else if (!Input.GetMouseButtonUp(0) && touchMouseBegin && (Vector2)Input.mousePosition != startPosTouch)
                {
                    touch.phase = TouchPhase.Moved;
                    touch.deltaPosition = Input.mousePosition - startPosMouse;
                    touch.position = Input.mousePosition;
                    touch.tapCount = 1;
                    touch.fingerId = 1;
                }
                else if (Input.GetMouseButtonUp(0) && touchMouseBegin)
                {
                    touch.phase = TouchPhase.Ended;
                    touchMouseBegin = false;
                    touch.tapCount = 1;
                }
            }
            touchBegin = HandleTouch(ref currentPos, ref directionTouch, ref startPosTouch, touch);
        }
        return touchBegin;
    }


    private bool HandleTouch(ref Vector2 currentPos, ref Vector2 directionTouch, ref Vector2 startPosTouch, Touch touch)
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
        { if (touch.tapCount <= 0) return false; }

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return false;
                startPosTouch = touch.position;
                touchBegin = true;
                currentPos = touch.position;
                startPosTouch = touch.position;
                OnStartSelection(currentPos);
                break;

            case TouchPhase.Moved:
                directionTouch = touch.deltaPosition;
                touchBegin = true;

                currentPos = touch.position;
                break;
            case TouchPhase.Stationary:
                currentPos = touch.position;
                touchBegin = true;

                break;
            case TouchPhase.Ended:
                touchBegin = false;
                directionTouch = Vector2.zero;
                OnEndSelection();

                break;
            case TouchPhase.Canceled:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return touchBegin;
    }
}

class LineManager
{
    private int highlightedListCount;
    List<IHighlightableComponent> onwList = new List<IHighlightableComponent>();
    List<SpriteRenderer> lines = new List<SpriteRenderer>();


    public LineManager()
    {
    }

    public void AddPoint(IHighlightableComponent item, Vector3? lastPos, List<IHighlightableComponent> selectedList, bool fromAddRange = false)
    {
        if (selectedList.Contains(item)) return;
        if (lastPos != null && Vector2.Distance(item.transform.position, lastPos.Value) > 2 && !fromAddRange)
        {
            AddRange(item, selectedList);
            return;
        }
        if (item != null)
        {
            item.SetOutline(true);
            item.highlighted = true;
        }
        // item?.SetSelected(true);
        if (lastPos != null)
        {
            GameObject lineObj = ObjectPooler.Instance.GetPooledObject("line");
            SpriteRenderer line = lineObj.GetComponent<SpriteRenderer>();
            Vector3 dir = lastPos.Value - item.transform.position;
            Vector3 initVector = (item.transform.position + Vector3.left) - item.transform.position;
            line.transform.position = item.transform.position + dir * 0.5f;
            float angle = Vector2.Angle(dir, initVector);
            if (item.transform.position.y < lastPos.Value.y) angle = -angle;
            line.transform.rotation = Quaternion.Euler(0, 0, angle);
            lines.Add(line);
        }
        selectedList.Add(item);
    }

    void AddRange(IHighlightableComponent item, List<IHighlightableComponent> selectedList)
    {
        Vector2? lastPos = selectedList.LastOrDefault().transform.position;
        if (lastPos == null) return;
        IHighlightableComponent closetItem = null;
        var missedRange = HighlightManager.GetLinePath(item, ref closetItem, selectedList.ToArray());
        foreach (var element in missedRange)
        {
            if (lastPos != null /* && Vector2.Distance(element.transform.position, lastPos.Value) < 2f */)
                AddPoint(element, closetItem.transform.position, selectedList, true);
        }
    }

    internal void Clear()
    {
        lines.ForEach(i => i.gameObject.SetActive(false));
        lines.Clear();
        onwList.Clear();
    }
}
