using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelNumber : MonoBehaviour
{
    private GameObject canvasMap;
    MapLevel mapLevel;
    Transform pin;
    // Use this for initialization
    void Start()
    {
        mapLevel = transform.parent.parent.GetComponent<MapLevel>();
        int num = int.Parse(transform.parent.parent.name.Replace("Level", ""));
        GetComponent<Text>().text = "" + num;
        pin = transform.parent;
        canvasMap = GameObject.Find("WorldCanvas");
        transform.SetParent(canvasMap.transform, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (mapLevel.IsLocked && gameObject.activeSelf) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        // transform.position = pin.position;

    }
}
