using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TurnPillerManager : MonoBehaviour
{
    public GameObject PillerPrefub;
    public int ReturnFlame;//柱の回転時間１フレーム1/60

    [HideInInspector] public GameObject[] Piller;//加点柱オブジェクト

    private PillerManager FieldPiller;

    // Start is called before the first frame update
    void Start()
    {
        FieldPiller = this.GetComponent<PillerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //フレーム固定の更新
    private void FixedUpdate()
    {

    }

    //回転柱配列作成
    //sizenum 要素数
    public void PrePiller(int sizenum)
    {
        Piller = new GameObject[sizenum];
        for (int i = 0; i < Piller.Length; i++)
        {
            Piller[i] = null;
        }
    }

    //回転柱情報を配列にセット
    public void SetPiller(GameObject piller)
    {
        int i = 0;
        for (i = 0; i < Piller.Length; i++)
        {
            if (Piller[i] == null)
            {
                Piller[i] = piller;
                return;
            }
        }

        Array.Resize(ref Piller, Piller.Length + 5);
        Piller[i] = piller;
    }

    //回転できるやつがあったら回転開始
    public bool StartReverse(int pillerid, int height)
    {
        for (int i = 0; i < Piller.Length; i++)
        {
            

            //中身がないところまで来たら回転しない
            if (Piller[i] == null)
            {
                return false;
            }

            //必要なやつ取得
            Field field = Piller[i].GetComponent<Field>();
            TurnPiller turnPiller = Piller[i].GetComponent<TurnPiller>();

            //同じ柱でなおかつ柱の範囲にいるか
            int ereamax = field.nowHeight + turnPiller.size - 1;
            if (field.nowPiller == pillerid &&
                (ereamax >= height && ereamax - (turnPiller.size * 2) - 1 <= height))
            {
                turnPiller.ReverseStart(true);
                return true;
            }
        }

        return false;
    }
}
