using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelColliderDisable : MonoBehaviour
{

    private void OnEnable()
    {
        // if (!Application.isPlaying) gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        // else gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }
}
