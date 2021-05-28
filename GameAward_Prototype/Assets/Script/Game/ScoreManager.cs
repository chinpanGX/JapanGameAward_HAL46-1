using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text Action;
    [SerializeField] Text Time;

    //ŽžŠÔ
    public int time { get; set; }
    private int flamecount;

    //ƒAƒNƒVƒ‡ƒ“
    public int action { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Action.text = "Action  " + 0;
        Time.text = "Time  " + 0;
    }

    private void FixedUpdate()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_GAME && !Fade.m_isFadeOut)
        {
            Action.gameObject.SetActive(true);
            Time.gameObject.SetActive(true);

            if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_PLAY)
            {
                if (flamecount % 60 == 0)
                {
                    if (time <= 9999)
                    {
                        time++;
                        Time.text = "Time  " + time;
                    }
                }
                flamecount++;
            }
            else if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_CLEAR)
            {
                Action.gameObject.SetActive(false);
                Time.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            Action.gameObject.SetActive(false);
            Time.gameObject.SetActive(false);
        }

        
    }

    public void CountAction()
    {
        if (action > 9999)
        {
            return;
        }
        action++;
        Action.text = "Action  " + action;
    }
}
