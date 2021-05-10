using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoostAnimation : MonoBehaviour
{
    public GameObject appearingEffect;

    public void ShowEffect()
    {
        GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
        Destroy(partcl, 1f);

    }


    public void OnFinished(BoostType boostType)
    {
        GameObject effect = Instantiate(appearingEffect) as GameObject;
        effect.transform.position = transform.position;
        Destroy(effect, 2);

        if (LevelManager.THIS.waitingBoost.type == BoostType.Hammer)
        {
            LevelManager.THIS.waitingBoost = null;
        }

        if (!name.Contains("shovel"))
            SoundBase.Instance.PlaySound(SoundBase.Instance.explosion);
        else
            SoundBase.Instance.PlaySound(SoundBase.Instance.shovel);

        LevelManager.THIS.ClearHighlight(true);
        Destroy(gameObject);
    }
}
