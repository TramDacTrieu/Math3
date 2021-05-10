using UnityEngine;
using System.Collections;

public class MusicBase : MonoBehaviour
{

    public static MusicBase Instance;
    public AudioClip[] music;

    ///MusicBase.Instance.audio.PlayOneShot(MusicBase.Instance.music[0]);


    // Use this for initialization
    void Awake()
    {
        if (transform.parent == null)
        {
            // transform.parent = Camera.main.transform;
            transform.localPosition = Vector3.zero;
        }

        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = MusicBase.Instance.music[1];
        audioSource.Play(0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
