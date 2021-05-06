using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPiller : MonoBehaviour
{
    //回転速度
    public int ReturnFlame;

    public int size { get; set; }

    private Field field;//フィールド

    //柱マネージャー
    private PillerManager piller;

    private bool ReturnFlag;
    private Quaternion Move;
    private Vector3 Axis;

    private int flamecount;


    // Start is called before the first frame update
    void Start()
    {
        ReturnFlag = false;
        Move = Quaternion.identity;
        Axis = Vector3.zero;
        flamecount = 0;

        field = this.GetComponent<Field>();

        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ReverseStart(true);
        }
    }

    private void FixedUpdate()
    {
        Reverse();
    }

    //表裏反転
    //Pillerid 柱ID
    //rotedirection　回転方向　true 後ろ　false 手前
    public void ReverseStart(bool rotedirection)
    {
        //回転軸
        //中心(自分と高さ同じ)までの方向ベクトル
        Axis = new Vector3(0.0f, this.transform.position.y, 0.0f) - this.transform.position;

        //回転軸を求める
        Axis = Vector3.Cross(Axis, Vector3.up);
        Axis.Normalize();//なんとなく正規化

        //ブロック情報等を受け取る
        for (int i = 0; i < size * 2; i++)
        {
            GameObject obj = piller.GetBlockObject(field.nowPiller, field.nowHeight + size - 1 - i);
            if (obj != null)
            {
                obj.transform.parent = this.transform;
            }
        }

        //フラグ関係
        foreach (Transform child in this.transform)
        {
            Field block = child.GetComponent<Field>();
            block.FallFlag = false;
        }

        //回転処理
        float angle = 180.0f / (float)ReturnFlame;
        if (rotedirection == false)
        {
            angle *= -1;
        }
        Move = Quaternion.AngleAxis(angle, Axis);//一回の回転量

        ReturnFlag = true;
    }

    //回転処理
    private void Reverse()
    {
        if (ReturnFlag == true)//回転している場合
        {

            //回転
            this.transform.rotation *= Move;

            //フレームカウント
            flamecount++;

            if (flamecount >= ReturnFlame)//フレームカウントが指定の数値を超えたら
            {

                //柱が逆さまになったのを回転前に戻す
                this.transform.Rotate(Axis, 180.0f);

                //ブロックフラグ関係
                foreach (Transform child in this.transform)
                {
                    Field block = child.GetComponent<Field>();
                    block.FallFlag = true;
                    block.ChangeHeight(field.nowHeight);
                    child.transform.parent = this.transform.parent;
                    child.transform.position = new Vector3(child.transform.position.x, block.nowHeight, child.transform.position.z);
                }

                flamecount = 0;//フレームカウントリセット
                ReturnFlag = false;//回転する柱をリセット
            }
        }
    }
}
