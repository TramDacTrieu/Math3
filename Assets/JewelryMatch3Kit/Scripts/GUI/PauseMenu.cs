using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Text playButton;
    public GameObject close;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "map")
        {
            playButton.text = "Play";
            close.SetActive(true);
        }
        else
        {
            playButton.text = "OK";
            close.SetActive(false);
        }

    }
}
