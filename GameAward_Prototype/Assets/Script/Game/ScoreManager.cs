using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text Action;
    [SerializeField] Text Time;

    //時間
    public int time { get; set; }
    private int flamecount;

    //アクション
    public int action { get; set; }

    //ミス
    public int miss { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Action.text = "Action  " + 0;
        Time.text = "Time  " + 0;
    }

    private void FixedUpdate()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_GAME)
        {
            if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_PLAY)
            {
                if (flamecount % 60 == 0)
                {
                    time++;

                    if (time <= 9999)
                    {
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

        
    }

    public void CountAction()
    {

        action++;
        if (action > 9999)
        {
            return;
        }
        Action.text = "Action  " + action;
    }
}
