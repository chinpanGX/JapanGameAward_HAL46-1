using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text Action;
    [SerializeField] Text Time;

    //ŽžŠÔ
    private int time { get; set; }
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
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_GAME)
        {
            return;
        }

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
