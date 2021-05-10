using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

public class PlayerAvatar : MonoBehaviour, IAvatarLoader
{
    public Image image;

    void Start()
    {
        image.enabled = false;
        var lastReachedLevel = LevelsMap._instance.GetMapLevels().Where(l => !l.IsLocked).Last();
        if (lastReachedLevel) lastReachedLevel.transform.Find("Idle").gameObject.SetActive(true);
    }

#if PLAYFAB || GAMESPARKS
	void OnEnable () {
		NetworkManager.OnPlayerPictureLoaded += ShowPicture;
	}

	void OnDisable () {
		NetworkManager.OnPlayerPictureLoaded -= ShowPicture;
	}


#endif
    public void ShowPicture()
    {
        image.sprite = InitScript.profilePic;
        image.enabled = true;
    }

}
