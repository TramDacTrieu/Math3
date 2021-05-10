using UnityEngine;
using UnityEngine.UI;

public class Moves : MonoBehaviour
{
    private Text txt;
    private bool alert;

    private void Start()
    {
        txt = GetComponent<Text>();
    }
    private void Update()
    {
        if (name == "Limit")
        {
            if (LevelManager.Instance.limitType == LIMIT.MOVES)
            {
                txt.text = "" + LevelManager.THIS.Limit;
                // txt.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-460, -42, 0);
                //txt.transform.localScale = Vector3.one;
                if (LevelManager.THIS.Limit <= 5)
                {
                    // txt.color = Color.red;
                    //txt.GetComponent<Outline>().effectColor = new Color(214 / 255, 0, 196 / 255);
                    if (!alert)
                    {
                        alert = true;
                        // if (LevelManager.THIS.gameStatus == GameState.Playing)
                        // SoundBase.Instance.PlaySound(SoundBase.Instance.alert);
                    }

                }
                else
                {
                    alert = false;
                    // txt.color = Color.white;
                    //txt.GetComponent<Outline>().effectColor = new Color(148f / 255f, 61f / 255f, 95f / 255f);
                }

            }
        }
    }

}