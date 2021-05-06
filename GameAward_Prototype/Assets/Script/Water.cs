using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private int m_Frame; // フレーム数
    [SerializeField]
    private int m_DelayTime; // 待ち時間

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
        // ディレイタイムを超えたら動き出す
        if (m_Frame > m_DelayTime)
        {
            Move();
        }
    }

    // 上昇
    private void Move()
    {
        Transform myTransform = this.transform;
        // 座標を取得
        Vector3 pos = myTransform.position;
        pos.y += 0.01f;    // x座標へ0.01加算
        myTransform.position = pos;  // 座標を設定
    }
}
