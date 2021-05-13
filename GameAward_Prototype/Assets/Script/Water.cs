using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private int m_Frame;
    [SerializeField] private int m_DelayTime;
    private bool m_IsGameOver = false; // ゲームオーバーしたかどうかの検知

    // Start is called before the first frame update
    void Start()
    {
        m_Frame = 0;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public bool GetGameOverFlag()
    {
        return m_IsGameOver;
    }

    private void FixedUpdate()
    {
        m_Frame++;
        if (m_Frame > m_DelayTime)
        {
            Move();
        }
    }

    // �㏸
    private void Move()
    {
        Transform myTransform = this.transform;
        Vector3 pos = myTransform.position;
        pos.y += 0.01f;    
        myTransform.position = pos; 
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_IsGameOver = true;
        }
    }
}
