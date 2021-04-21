using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerManager : MonoBehaviour
{

    public int Aroundnum;//一周の柱の数
    public int ReturnFlame;//柱の回転時間１フレーム1/60

    [HideInInspector]
    public GameObject[] Piller;//柱オブジェクト


    private int ReturnPillerID;//フラグ
    private Quaternion ReturnMove;//１フレームの回転角度
    private Vector3 Axis;

    private float flamecount;

    private void Awake()
    {
        ReturnPillerID = -1;
        ReturnMove = Quaternion.identity;
        flamecount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
    public void ReverseStart(int Pillerid)
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
                //ブロックフラグ関係
                foreach (Transform child in Piller[ReturnPillerID].transform)
                {
                    Field block = child.GetComponent<Field>();
                    block.FallFlag = true;
                    block.ChangeHeight();
                }

                //柱が逆さまになったのを回転前に戻す
                Piller[ReturnPillerID].transform.Rotate(Axis, 180.0f);

                flamecount = 0;//フレームカウントリセット
                ReturnPillerID = -1;//回転する柱をリセット
            }

        }
    }

    //柱の配列を準備する
    public void PrePiller()
    {
        if (Piller.Length <= 0)//配列が設定されていない場合
        {
            Piller = new GameObject[Aroundnum];
        }
    }

    public bool StateReverce()
    {
        if (ReturnPillerID != -1)//回転中の場合
        {
            return true;
        }
        return false;
    }

    public bool GetPillerBlock(int piller, int height)
    {
        
        foreach (Transform child in Piller[piller].transform)//柱に置いてあるブロックを見る
        {
            if (child.name == ("Block" + height))//同じブロックが存在した場合
            {
                return true;
            }
        }
        return false;
    }

}
