using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//移動速度
    public int jumpflame;
    private PillerManager piller;//柱情報

    //フィールド取得
    Field field;

    public bool InputState;//移動状態

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    private int BlockUpDownFlag;//ブロック上り下りフラグ
    private bool Way;

    private const int MOVE_NONE = 0;
    private const int MOVE_UP = 1;
    private const int MOVE_X = 2;

    private const int MOVE_CENTER = 3;
    private const int MOVE_DOWN = 4;
    public int MoveFlag;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();


        field = this.GetComponent<Field>();
        field.DefoMoveFlag = true;

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
                piller.ReverseStart(this.GetComponent<Field>().nowPiller, false);
            }
            else if (Input.GetButtonDown("Jump"))//ブロック上る
            {
                BlockUp();
            }
        }
    }


    //ブロック一個上る
    private void BlockUp()
    {
        //上にブロックがあるか
        if (piller.GetPillerBlock(field.nowPiller, field.nowHeight + 1))
        {
            return;
        }

        if (field.NowWay())//右にいる場合
        {
            //右側にブロックがあるか //右一個上にブロックがあるか
            if (!piller.GetPillerBlock(field.MovePillerID(true), field.nowHeight) ||
                piller.GetPillerBlock(field.MovePillerID(true), field.nowHeight + 1))
            {
                return;
            }

            Way = true;
        }
        else//左にいる場合
        {
            //左側にブロックがあるか  //左一個上にブロックがあるか
            if (!piller.GetPillerBlock(field.MovePillerID(false), field.nowHeight)||
                piller.GetPillerBlock(field.MovePillerID(false), field.nowHeight + 1))
            {
                return;
            }

            Way = false;
        }


        BlockUpDownFlag = BLOCK_UP;
        InputState = false;
    }

    private void UpDownMove()
    {
        if (piller.StateReverce())
        {
            return;
        }

        if (BlockUpDownFlag == BLOCK_UP)
        {
            if (MoveFlag == MOVE_NONE)//登る開始
            {
                field.SetYMove(field.nowHeight + 1, jumpflame);
                MoveFlag = MOVE_UP;
            }
            else if (MoveFlag == MOVE_UP && !field.YMoveFlag)//登り中
            {
                field.SetXMove(Way, jumpflame);//右移動開始
                MoveFlag = MOVE_X;
            }
            else if (MoveFlag == MOVE_X && !field.XMoveFlag)//右移動中
            {
                //ブロック登り終わり
                InputState = true;
                MoveFlag = MOVE_NONE;
                BlockUpDownFlag = BLOCK_NONE;
            }
        }
        else if (BlockUpDownFlag == BLOCK_DOWN)
        {

            if (MoveFlag == MOVE_NONE && 
                !piller.GetPillerBlock(field.nowPiller, field.nowHeight - 1))//中央に移動開始
            {
                field.SetCenterMove(jumpflame);
                //field.SetXMove(true, jumpflame);
                MoveFlag = MOVE_CENTER;
                InputState = false;
            }
            else if (MoveFlag == MOVE_CENTER && !field.MoveCenter)//中央に移動中
            {
                if (field.nowHeight - 1 < 0)//0より小さい場合
                {
                    MoveFlag = MOVE_NONE;
                    BlockUpDownFlag = BLOCK_NONE;
                    InputState = true;

                }
                else
                {
                    int downnum = 1;
                    for (int i = 2; i < 4; i++)
                    {
                        if (field.nowHeight - i < 0)
                        {
                            break;
                        }
                        else if (!piller.GetPillerBlock(field.nowPiller, field.nowHeight - i))
                        {
                            downnum++;
                        }
                    }
                    field.SelectChangeHeight(field.nowHeight - downnum);
                    MoveFlag = MOVE_DOWN;
                }
            }
            else if (MoveFlag == MOVE_DOWN && this.GetComponent<Rigidbody>().isKinematic)//空中にいる時
            {
                //着地した時
                MoveFlag = MOVE_NONE;
                BlockUpDownFlag = BLOCK_NONE;
                InputState = true;
            }
        }
        else if (BlockUpDownFlag == BLOCK_NONE && field.nowHeight > 0)//高さが0以上の時
        {
            BlockUpDownFlag = BLOCK_DOWN;
        }
        
    }

    //ブロック降りる
    private void BlockDown()
    {

    }

    
}
