using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] int m_StartHeight;
    [SerializeField] GameObject m_Player;

    private bool moveflag;
    private bool hitflag;

    //警告
    [SerializeField] GameObject m_warning;
    private int m_flamecount;

    private int m_Time;
    [SerializeField] private int m_MaxTime;

    private AudioController gameaudio;

    // Start is called before the first frame update
    void Start()
    {
        m_warning.SetActive(false);
        m_flamecount = 0;
        m_Time = 0;
        moveflag = false;

        this.transform.position = new Vector3(0.0f, -2.0f, 0.0f);

        gameaudio = AudioManager.PlayAudio("Game01", true, true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_GAME || StatusFlagManager.GameStatusFlag != StatusFlagManager.GAME_PLAY)
        {
            return;
        }

        // ヒット判定
        if (moveflag == true && this.transform.position.y >= m_Player.transform.position.y + 0.2f && !Fade.m_isFadeOut)
        {
            m_warning.SetActive(false);
            m_flamecount = 0;
            moveflag = false;
            hitflag = true;
            m_Player.transform.Find("PlayerModel").GetComponent<Animator>().SetBool("GameOver", true);
            gameaudio.FadeOutStart(20);
            StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
            StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_START;
            Fade.FadeOut("SampleScene");

            return;
        }
        else if (hitflag)
        {
            return;
        }

        m_Time++;
        if (!moveflag && !hitflag && m_Player.transform.position.y >= m_StartHeight || m_Time > m_MaxTime)//指定された高さに達したら移動が始まる
        {
            moveflag = true;
            gameaudio.FadeOutStart(20);
            gameaudio = AudioManager.PlayAudio("Game02", true, true);
        }

        if (moveflag == true && m_Player.transform.position.y < 20)
        {
            Move();

            //警告表示
            if (m_flamecount % 40 == 0 && m_flamecount <= 280)
            {
                bool active = m_warning.activeSelf;
                m_warning.SetActive(!active);
            }
            else if (m_flamecount >= 280)
            {
                m_warning.SetActive(false);
            }

            m_flamecount++;
        }
        else if (m_Player.transform.position.y >= 20)
        {
            gameaudio.FadeOutStart(120);
        }
        else
        {
            m_warning.SetActive(false);
            m_flamecount = 0;
            moveflag = false;
        }
    }

    // �㏸
    private void Move()
    {
        Transform myTransform = this.transform;
        Vector3 pos = myTransform.position;
        pos.y += m_speed;
        myTransform.position = pos;
    }
}
