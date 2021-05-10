using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JewelryMatch3Kit.Scripts.Diamond
{
    public class Wire : MonoBehaviour, IHittable
    {
        public Sprite[] Sprites;
        public int HP = 1;
        public SpriteRenderer[] SpriteRenderers;
        public GameObject lightObject;
        public GameObject lightPrefab;
        private Transform smallWire;

        private void Awake()
        {
            // for (int i = 0; i < HP; i++)
            // {
            //     SpriteRenderers[i].sprite = Sprites[i];
            //     SpriteRenderers[i].enabled = true;
            // }

        }

        private void OnEnable()
        {
            SpriteRenderers.ForEachY(i => i.sortingLayerName = "Default");
            if (SpriteRenderers.Length > 1)
            {
                smallWire = transform.GetChild(0).GetComponentInChildren<Wire>().transform;
                smallWire.localPosition = Vector2.zero;
                smallWire.GetComponent<Collider2D>().enabled = false;
            }
            // ChangeState();
        }

        private void ChangeState()
        {

            if (HP >= 2)
                SpriteRenderers.ForEachY(i => i.enabled = true);
            else if (HP <= 1 && SpriteRenderers.Length > 1) SpriteRenderers[1].enabled = false;
            // for (int i = 0; i < SpriteRenderers.Length; i++)
            // {
            //     SpriteRenderers[i].enabled = false;
            // }

        }

        public void Hit(Vector2 pos, Action callback = null)
        {
            // HP--;
            // ChangeState();
            if (smallWire?.gameObject.activeSelf ?? false)
            {
                if (!smallWire.GetComponent<IDestroyableComponent>().destroyed)
                    smallWire.GetComponent<IDestroyableComponent>().Hit(pos, false, false);
                Invoke("Wait", 1);
            }
            else
            {
                SpriteRenderers.ForEachY(i => i.sortingLayerName = "Item3");
                var falling = gameObject.AddComponent<FallingAction>();
                falling.callback = callback;
            }

        }

        void Wait()
        {
            GetComponent<IDestroyableComponent>().destroyed = false;
        }



    }
}