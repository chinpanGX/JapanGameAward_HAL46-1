using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//移動速度
    private PillerManager piller;//柱情報

    //フィールド取得
    Field field;

    private bool InputState;//移動状態

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    private int BlockUpDownFlag;//ブロック上り下りフラグ

    
    private int MoveFlag;
    
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();


        field = this.GetComponent<Field>();

        InputState = true;
    }

    // Update is called once per frame
    void Update()
    {
        //入力処理
        ProcesInput();
    }

    private void FixedUpdate()
    {
        //上移動
        UpDownMove();

        //左右移動制限処理
        MoveRest();

        //左右移動
        field.ProcesMove();

        //柱変更処理
        ProcesPiller();
    }

    //入力処理
    private void ProcesInput()
    {
        if (!piller.StateReverce() && InputState)
        {
            field.SetNoMove();

            if (Input.GetButton("Right"))//右
            {
                field.SetMove(-speed);
            }
            else if (Input.GetButton("Left"))//左
            {
                field.SetMove(speed);
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
        if (Mathf.Abs(field.nowmoveaxis) > pillerposi)//小さい
        {

            //変数の中身変更
            if (field.nowmoveaxis <= 0)//右にいる
            {
                field.nowmoveaxis = pillerposi + (field.nowmoveaxis + pillerposi);//移動幅リセット
                field.nowPiller = MovePillerID(true);//現在の柱変更
            }
            else if (field.nowmoveaxis > 0)//左にいる
            {
                field.nowmoveaxis = -pillerposi + (field.nowmoveaxis - pillerposi);//移動幅リセット
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
        float nextmove = field.nowmoveaxis + field.moveaxis;//次の移動角度を計算
        if (Mathf.Abs(nextmove) + 0.8f > ProcessPillerPosi())
        {
            if ((field.nowmoveaxis <= 0 && piller.GetPillerBlock(MovePillerID(true), field.nowHeight)) || //右にいる
                (field.nowmoveaxis > 0 && piller.GetPillerBlock(MovePillerID(false), field.nowHeight)))   //左にいる
            {
                //移動しない
                field.SetNoMove();
            }
        }
    }

    //ブロック一個上る
    private void BlockUp()
    {
        BlockUpDownFlag = BLOCK_UP;
    }

    private void UpDownMove()
    {
        if (BlockUpDownFlag == BLOCK_UP)
        {
            //field.SelectChangeHeight(field.nowHeight + 1);
        }
        else if (BlockUpDownFlag == BLOCK_DOWN)
        {
            //field.SelectChangeHeight(field.nowHeight - 1);
        }
        
    }

    //ブロック降りる
    private void BlockDown()
    {

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
}
