using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Line : MonoBehaviour
{
    public Material material;
    private Mesh mesh;
    public Vector3[] vertices;
    public Vector3 start;
    public Vector3 end;
    public int lineWidth;

    public List<LineRenderer> lines = new List<LineRenderer>();
    public Vector3[] points = new Vector3[200];

    // Use this for initialization
    void Start()
    {
        // for (int i = 0; i < 30; i++)
        // {
        //     lines.Add(ObjectPooler.Instance.GetPooledObject("line", true).GetComponent<LineRenderer>());
        // }

    }


    public void SetVertexCount(int count)
    {
        int i = 0;
        if (lines.Count < count)
            AddLine();
        foreach (LineRenderer item in lines)
        {
            if (i < count)
            {
                item.enabled = true;
                item.transform.GetChild(0).gameObject.SetActive(true);
                item.transform.GetChild(1).gameObject.SetActive(true);
                // SetSorting(item);
            }
            else
            {
                item.enabled = false;
                item.transform.GetChild(0).position = Vector3.left * 1000;
                item.transform.GetChild(1).position = Vector3.left * 1000;
                item.transform.GetChild(0).gameObject.SetActive(false);
                item.transform.GetChild(1).gameObject.SetActive(false);
                item.SetPosition(0, Vector2.zero);
                item.SetPosition(1, Vector2.zero);
            }
            i++;
        }
    }

    public void DrawLines(IHighlightableComponent[] list)
    {
        // list = list?.Select(i => i.GetComponent<Transform>())?.OrderByDistance().Select(i => i.GetComponent<IHighlightableComponent>()).ToArray();
        for (int i = 1; i < list.Length; i++)
        {
            if (list.Length == 1) break;
            var item = list[i];
            var item2 = list[i - 1];
            if (item != null && item2 != null)
            {
                if (DistanceHandle.GetDistance(item.transform, item2.transform) < 0.5f)
                    AddPoint(item.transform.position, item2.transform.position, i);
            }
        }
    }

    public void AddPoint(Vector3 position, Vector3 position2, int index)
    {
        points[index] = position;
        points[index - 1] = position2;
        if (index > 0 && index < lines.Count)
        {
            LineRenderer lineRenderer = lines[index];
            Vector3 position1 = points[index - 1];
            SetPointPosition(lineRenderer.transform.GetChild(0), position);
            SetPointPosition(lineRenderer.transform.GetChild(1), position2);
            lineRenderer.SetPosition(0, position1);
            lineRenderer.SetPosition(1, points[index]);
        }
    }

    public void HidePoints()
    {
        SetVertexCount(0);
        // points.ForEachY(i => i = Vector2.zero);
        // var lines = GameObject.FindGameObjectsWithTag("line");
        // foreach (var item in lines)
        //     item.gameObject.SetActive(false);
    }

    private static void SetPointPosition(Transform transform1, Vector3 position1)
    {
        transform1.gameObject.SetActive(true);
        transform1.position = position1;
    }

    void AddLine()
    {
        // GameObject newLine = Instantiate(transform.GetChild(0).gameObject) as GameObject;
        // newLine.transform.SetParent(transform);
        lines.Add(ObjectPooler.Instance.GetPooledObject("line", true).GetComponent<LineRenderer>());

    }

    void SetSorting(LineRenderer lr)
    {
        lr.sortingLayerID = 0;
        lr.sortingOrder = 10;

    }
}
