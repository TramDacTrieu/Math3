using UnityEngine;

public class VisibilityListener : MonoBehaviour
{
    public static int visibleItems;
    public bool visible;
    private SpriteRenderer spriteRenderer;

    public delegate void BecameVisible(bool isVisible);
    public event BecameVisible OnBecameVisib;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnBecameVisible()
    {
        visibleItems++;
        OnBecameVisib?.Invoke(true);
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visibleItems = Mathf.Clamp(visibleItems - 1, 0, visibleItems - 1);
        OnBecameVisib?.Invoke(false);
        visible = false;
    }

    // private void OnGUI()
    // {
    //     GUILayout.Label("" + visibleItems);
    // }
}