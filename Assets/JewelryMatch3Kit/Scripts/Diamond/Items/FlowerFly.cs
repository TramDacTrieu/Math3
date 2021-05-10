using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets._2D;

namespace JewelryMatch3Kit.Scripts.Diamond
{
    public class FlowerFly : MonoBehaviour
    {
        public static List<Transform> targets = new List<Transform>();
        public string[] priorityTags;
        public Transform targetTransform;
        public void Launch(Vector2 startPos, Action action = null)
        {
            transform.position = startPos;
            StartCoroutine(StartFly(action));
        }

        IEnumerator StartFly(Action action = null)
        {
            var target = targetTransform == null ? GetTarget() : targetTransform;
            if (target == null) gameObject.SetActive(false);
            var startTime = Time.time;
            float distCovered;
            float speed = 2;

            float journeyLength = Vector2.Distance(transform.position, target.position);
            float fracJourney;
            while (true)
            {
                distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / journeyLength;
                transform.position = Vector2.Lerp(transform.position, target.position, fracJourney);
                transform.Rotate(Vector3.back, Time.realtimeSinceStartup);
                yield return new WaitForFixedUpdate();
                if (Vector3.Distance(transform.position, target.position) < 0.5f)
                {
                    action?.Invoke();
                    DestroyTarget(target);
                    break;
                }
            }
            gameObject.SetActive(false);
        }

        void DestroyTarget(Transform target)
        {
            target?.GetComponent<IDestroyableComponent>().Hit(target.transform.position);
        }

        Transform GetTarget()
        {
            var hit = Camera.main.GetComponent<Camera2DFollow>().GetVisibleObjects().Where(i => i.GetComponent<IDestroyableComponent>());// Physics2D.OverlapCircleAll(Camera.main.transform.position, 10);
            if (hit == null) return null;
            foreach (var priorityTag in priorityTags)
            {
                var findSpecialObject = hit?.Where(i => i.CompareTag(priorityTag) && !targets.Contains(i.transform) && !i.GetComponent<IDestroyableComponent>().destroyed).NextRandom();
                if (findSpecialObject != null)
                {
                    Transform transform1 = findSpecialObject.transform;
                    targets.Add(transform1);
                    return transform1;
                }
            }
            return hit?.Where(i => i != null && i.layer == LayerMask.NameToLayer("Item")).NextRandom()?.transform;
        }

    }
}
