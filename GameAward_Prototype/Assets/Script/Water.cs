using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private int m_Frame;
    [SerializeField] float m_speed;
    [SerializeField] private int m_DelayTime;
    [SerializeField] GameObject m_Player;

    // Start is called before the first frame update
    void Start()
    {
        m_Frame = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        m_Frame++;
        if (m_Frame > m_DelayTime)
        {
            Move();
        }
        // y座標比較
        if(this.transform.position.y >= m_Player.transform.position.y + 0.4f)
        {
            Scene.ChangeScene("Test");
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
