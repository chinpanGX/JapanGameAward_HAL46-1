using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float Length;//距離


    private GameObject player;//プレイヤー
    private PillerManager piller;//柱情報

    // Start is called before the first frame update
    void Start()
    {
        //プレイヤーオブジェクト貰う
        player = GameObject.Find("Player").gameObject;

        //柱情報
        piller = GameObject.Find("Manager").gameObject.GetComponent<PillerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!piller.StateReverce())
        {
            //プレイヤーの情報をカメラ用に加工したやつ
            Vector3 zikuposi = new Vector3(player.transform.position.x, 2.0f, player.transform.position.z);

            //基準
            Vector3 kijun = new Vector3(0.0f, 2.0f, 0.0f);

            //ベクトル
            Vector3 vec = zikuposi - kijun;
            Vector3.Normalize(vec);//正規化

            //座標設定
            this.transform.position = kijun + (vec * Length);

            //軸ポジを見る
            this.transform.LookAt(zikuposi);
        }

        
    }
}
