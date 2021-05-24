using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TurnPillerManager : MonoBehaviour
{
    public GameObject PillerPrefub;
    public int ReturnFlame;//柱の回転時間１フレーム1/60

    //[HideInInspector] public GameObject[] Piller;//加点柱オブジェクト
    public List<GameObject> Piller { get; set; }

    private PillerManager FieldPiller;

    private void Awake()
    {
        Piller = new List<GameObject>();
    }

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

    //回転柱情報を配列にセット
    public void SetPiller(GameObject piller, int side, int height)
    {
        for (int i = 0; i < Piller.Count; i++)
        {
            Field field = Piller[i].GetComponent<Field>();
            int inside = field.nowPiller;
            int inheight = field.nowHeight;
            if (inside == side)
            {
                Piller.Insert(i, piller);
                return;
            }
        }

        Piller.Add(piller);
    }

    //回転できるやつがあったら回転開始
    public GameObject StartReverse(int pillerid, int height)
    {
        GameObject obj = null;
        bool flag = false;
        for (int i = 0; i < Piller.Count; i++)
        {

            //データ受け取る
            Field field = Piller[i].GetComponent<Field>();
            TurnPiller turnPiller = Piller[i].GetComponent<TurnPiller>();

            //同じ柱ない場合
            if (field.nowPiller != pillerid)
            {
                if (flag)
                {   
                    break;
                }
                continue;
            }
            else if(!flag)
            {
                flag = true;
            }

            
            //同じ柱でなおかつ柱の範囲にいるか
            int ereamax = field.nowHeight + turnPiller.size - 1;
            int ereamin = field.nowHeight - turnPiller.size;
            if (ereamax >= height && ereamin <= height)
            {
                //すでに見つけてる高さより小さい場合は入れ替える
                if (obj == null)
                {
                    obj = Piller[i];
                }
                else if (obj.GetComponent<Field>().nowHeight > field.nowHeight)
                {
                    obj = Piller[i];
                }
            }
        }

        if (obj != null)
        {
            obj.GetComponent<TurnPiller>().ReverseStart(true);
        }
        return obj;
    }
}
