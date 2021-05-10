using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSound : MonoBehaviour, IPoolable
{
    public AudioClip[] hitSounds;
    public AudioClip[] selection;
    public AudioClip[] destroySounds;
    public AudioClip[] appearSounds;
    private Rigidbody2D rb;

    bool waitSound;
    private SoundBase soundBase;

    public SoundBase SoundBase
    {
        get
        {
            if (soundBase == null)
                soundBase = SoundBase.Instance;
            return soundBase;
        }

        set
        {
            soundBase = value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void OnEnable()
    {

    }



    public void SelectionSound(int i)
    {
        // if (!waitSound)
        {
            SoundBase?.PlaySound(selection[Mathf.Clamp(i, 0, selection.Length - 1)]);
            StartCoroutine(SoundCD());
        }
    }

    IEnumerator SoundCD()
    {
        waitSound = true;
        yield return new WaitForSeconds(2);
        waitSound = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (rb != null && rb?.velocity.magnitude > 4)
        {
            SoundBase?.PlaySoundsRandom(hitSounds);
        }
    }

    public void DestroySound()
    {
        SoundBase?.PlaySoundsRandom(destroySounds);
    }

    public void OnGetFromPool(Vector2 pos)
    {
        if (Application.isPlaying)
            SoundBase?.PlaySoundsRandom(appearSounds);
    }
}
