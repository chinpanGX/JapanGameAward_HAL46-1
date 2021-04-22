using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//移動速度
    public PillerManager piller;//柱情報

    //フィールド取得
    Field field;

    public Quaternion move { get; set; }//移動
    private float moveaxis;

    private float nowmove;//今の角度

    private bool MoveState;//移動状態
    
    // Start is called before the first frame update
    void Start()
    {
        move = Quaternion.identity;
        nowmove = 0.0f;
        field = this.GetComponent<Field>();

        MoveState = true;
    }

    // Update is called once per frame
    void Update()
    {
        //入力処理
        ProcesInput();
    }

    private void FixedUpdate()
    {
        //左右移動制限処理
        MoveRest();

        //左右移動
        ProcesMove();

        //柱変更処理
        ProcesPiller();
    }

    //入力処理
    private void ProcesInput()
    {
        if (!piller.StateReverce() && MoveState)
        {
            SetNoMove();

            if (Input.GetButton("Right"))//右
            {
                SetMove(-speed);
            }
            else if (Input.GetButton("Left"))//左
            {
                SetMove(speed);
            }
            else if (Input.GetButtonDown("Reverce"))//回転
            {
                piller.ReverseStart(this.GetComponent<Field>().nowPiller);
            }
            else if (Input.GetButtonDown("Jump"))//ブロック上る
            {
                BlockUp();
            }
        }
    }

    //柱処理
    private void ProcesPiller()
    {
        //柱処理
        float pillerposi = ProcessPillerPosi();
        if (Mathf.Abs(nowmove) > pillerposi)//小さい
        {

            //変数の中身変更
            if (nowmove <= 0)//右にいる
            {
                nowmove = pillerposi + (nowmove + pillerposi);//移動幅リセット
                field.nowPiller = MovePillerID(true);//現在の柱変更
            }
            else if (nowmove > 0)//左にいる
            {
                nowmove = -pillerposi + (nowmove - pillerposi);//移動幅リセット
                field.nowPiller = MovePillerID(false);//現在の柱
            }

            //柱変更
            this.transform.parent = piller.Piller[field.nowPiller].transform;
        }
    }


    private float ProcessPillerPosi()
    {
        return ((360.0f / piller.Aroundnum) / 2.0f);
    }

    //移動停止処理
    private void MoveRest()
    {
        float nextmove = nowmove + moveaxis;//次の移動角度を計算
        if (Mathf.Abs(nextmove) + 0.8f > ProcessPillerPosi())
        {
            if ((nowmove <= 0 && piller.GetPillerBlock(MovePillerID(true), field.nowHeight)) || //右にいる
                (nowmove > 0 && piller.GetPillerBlock(MovePillerID(false), field.nowHeight)))   //左にいる
            {
                SetNoMove();//移動しない
            }
        }
    }

    //ブロック一個上る
    private void BlockUp()
    {
        field.SelectChangeHeight(field.nowHeight + 1);
    }

    //柱の移動ID
    //true 右　false 左
    private int MovePillerID(bool way)
    {
        if (way)//右
        {
            return (field.nowPiller + (piller.Aroundnum + 1)) % piller.Aroundnum;
        }

        //左
        return (field.nowPiller + (piller.Aroundnum - 1)) % piller.Aroundnum;
    }

    //移動
    private void ProcesMove()
    {
        this.transform.position = move * this.transform.position;
        nowmove += moveaxis;
    }

    //移動予約
    private void SetMove(float angle)
    {
        move = Quaternion.AngleAxis(angle, Vector3.up);
        moveaxis += angle;
    }

    //移動しない
    private void SetNoMove()
    {
        move = Quaternion.identity;
        moveaxis = 0.0f;
    }
}
