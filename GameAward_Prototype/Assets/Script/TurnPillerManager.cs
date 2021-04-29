using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPillerManager : MonoBehaviour
{
    public GameObject PillerPrefub;
    public int ReturnFlame;//柱の回転時間１フレーム1/60

    [HideInInspector] public GameObject[] Piller;//加点柱オブジェクト
    [HideInInspector] public int[] PillerWidht;//回転柱幅

    private PillerManager FieldPiller;

    private int ReturnPillerID;//フラグ
    private Quaternion ReturnMove;//１フレームの回転角度
    private Vector3 Axis;

    private float flamecount;

    // Start is called before the first frame update
    void Start()
    {
        FieldPiller = this.GetComponent<PillerManager>();

        ReturnPillerID = -1;
        ReturnMove = Quaternion.identity;
        flamecount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //フレーム固定の更新
    private void FixedUpdate()
    {
        Reverse();
    }


    //表裏反転
    //Pillerid 柱ID
    //rotedirection　回転方向　true 後ろ　false 手前
    public void ReverseStart(int Pillerid, bool rotedirection)
    {
        //回転軸
        //中心(自分と高さ同じ)までの方向ベクトル
        Axis = new Vector3(0.0f, Piller[Pillerid].transform.position.y, 0.0f) - Piller[Pillerid].transform.position;

        //回転軸を求める
        Axis = Vector3.Cross(Axis, Vector3.up);
        Axis.Normalize();//なんとなく正規化

        //フラグ関係
        foreach (Transform child in Piller[Pillerid].transform)
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
        ReturnMove = Quaternion.AngleAxis(angle, Axis);//一回の回転量

        ReturnPillerID = Pillerid;
    }


    //回転処理
    private void Reverse()
    {
        if (ReturnPillerID != -1)//回転している場合
        {

            //回転
            Piller[ReturnPillerID].transform.localRotation *= ReturnMove;

            //フレームカウント
            flamecount++;

            if (flamecount >= ReturnFlame)//フレームカウントが指定の数値を超えたら
            {
                //柱が逆さまになったのを回転前に戻す
                Piller[ReturnPillerID].transform.Rotate(Axis, 180.0f);

                //ブロックフラグ関係
                foreach (Transform child in Piller[ReturnPillerID].transform)
                {
                    Field block = child.GetComponent<Field>();
                    block.FallFlag = true;
                    block.ChangeHeight();
                }

                flamecount = 0;//フレームカウントリセット
                ReturnPillerID = -1;//回転する柱をリセット
            }
        }
    }


    //回転柱配列作成
    //sizenum 要素数
    public void PrePiller(int sizenum)
    {
        Piller = new GameObject[sizenum];
    }


    public bool StateReverce()
    {
        if (ReturnPillerID != -1)//回転中の場合
        {
            return true;
        }
        return false;
    }
}
