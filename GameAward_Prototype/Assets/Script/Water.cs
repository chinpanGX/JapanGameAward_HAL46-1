using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] int m_StartHeight;
    [SerializeField] GameObject m_Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (m_Player.transform.position.y >= m_StartHeight && m_Player.transform.position.y < 20)//指定された高さに達したら移動が始まる
        {
            Move();
        }

        // y座標比較
        if(this.transform.position.y >= m_Player.transform.position.y + 0.2f)
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
