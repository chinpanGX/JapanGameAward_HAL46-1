using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] int m_StartHeight;
    [SerializeField] GameObject m_Player;

    private bool moveflag;
    //警告
    [SerializeField] GameObject m_warning;
    private int m_flamecount;

    private int m_Time; // 
    [SerializeField] private int m_MaxTime;

    // Start is called before the first frame update
    void Start()
    {
        m_warning.SetActive(false);
        m_flamecount = 0;
        m_Time = 0;
        moveflag = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        m_Time++;
        if (m_Player.transform.position.y >= m_StartHeight || m_Time > m_MaxTime)//指定された高さに達したら移動が始まるまたはフレームが超えたとき
        {
            moveflag = true;
        }

        // ヒット判定
        if (this.transform.position.y >= m_Player.transform.position.y + 0.2f)
        {
            m_warning.SetActive(false);
            m_flamecount = 0;
            moveflag = false;
            Fade.FadeOut("Title");
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
