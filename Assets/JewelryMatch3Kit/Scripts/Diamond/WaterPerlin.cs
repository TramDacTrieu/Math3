using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class WaterPerlin : MonoBehaviour
{
    public Vector2[] posList = new Vector2[100];
    public LineRenderer line;
    public float amplitude = 5;
    public bool perlin = true;

    void Update()
    {
        // WaterPerl();
    }



    private void WaterPerl()
    {
        Vector2 pos1 = Vector2.left * 10;
        Vector2 pos2 = Vector2.right * 10;
        posList = new Vector2[10];
        line.positionCount = posList.Length;
        var distance = Vector2.Distance(pos1, pos2);

        for (var i = 0; i < posList.Length; i++)
        {
            posList[i] = Vector2.Lerp(pos1, pos2, (float)i / line.positionCount);
            line.SetPosition(i, posList[i]);
        }
        if (perlin)
        {
            for (var i = 0; i < posList.Length; i++)
            {
                posList[i] = new Vector2(posList[i].x, posList[i].y + Mathf.PerlinNoise(i, Time.time));
                line.SetPosition(i, posList[i]);
            }
        }
        posList[0] = pos1;
        posList[posList.Length - 1] = pos2;
    }
}
