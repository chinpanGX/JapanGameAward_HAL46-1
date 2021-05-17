using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//移動速度
    public int jumpflame;
    private PillerManager piller;//柱情報
    private TurnPillerManager turnpillermane;
    private TurnPiller turnpiller;

    //フィールド取得
    Field field;

    //見てる場所
    Vector3 look;

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    public int BlockUpDownFlag = BLOCK_NONE;//ブロック上り下りフラグ
    private bool Way;

    private const int MOVE_NONE = 0;
    private const int MOVE_UP = 1;
    private const int MOVE_X = 2;
    private const int MOVE_CENTER = 3;
    private const int MOVE_DOWN = 4;
    public int MoveFlag = MOVE_NONE;

    private const int INPUT_NONE = 0;//入力してない
    private const int INPUT_SET = 1;//入力しているまたは何かしら処理中
    private const int INPUT_LEFT = 2;//左
    private const int INPUT_RIGHT = 3;//右
    private const int INPUT_REVERSE = 4;//回転
    private const int INPUT_JUMP = 5;
    public int NowInput = INPUT_NONE;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        turnpillermane = manager.GetComponent<TurnPillerManager>();


        field = this.GetComponent<Field>();
        field.DefoMoveFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        //入力処理
        ProcesInput();

        if (!field.StateReverse())
        {
            //this.transform.LookAt(look);
        }
        
    }

    private void FixedUpdate()
    {
        if (!field.StateReverse())
        {
            HitProcess();
        }
        
        //上移動
        UpDownMove();

    }

    //入力処理
    private void ProcesInput()
    {
        if (NowInput == INPUT_NONE && !field.StateReverse() && !field.AirFlag)
        {
            field.SetNoMove();

            if (Input.GetButton("Right"))//右
            {
                field.SetMove(-speed);
                NowInput = INPUT_RIGHT;
            }
            else if (Input.GetButton("Left"))//左
            {
                field.SetMove(speed);
                NowInput = INPUT_LEFT;
            }
            else if (Input.GetButtonDown("Reverce"))//回転
            {
                field.SetNoMove();
                SetReverse();
            }
            else if (Input.GetButtonDown("Jump"))//ブロック上る
            {
                field.SetNoMove();
                SetBlockUp();
            }
        }
        else
        {
            if (BlockUpDownFlag == BLOCK_NONE)
            {
                if ((NowInput == INPUT_LEFT && !Input.GetButton("Left")) ||
                (NowInput == INPUT_RIGHT && !Input.GetButton("Right"))
                )
                {
                    NowInput = INPUT_NONE;
                }
                else if (NowInput == INPUT_REVERSE && !turnpiller.ReturnFlag && !field.AirFlag)
                {
                    NowInput = INPUT_NONE;
                }
            }
            
        }
    }

    //回転処理セット
    private void SetReverse()
    {
        GameObject obj = turnpillermane.StartReverse(field.nowPiller, field.nowHeight);
        if (obj != null)
        {
            turnpiller = obj.GetComponent<TurnPiller>();
            this.GetComponent<Rigidbody>().isKinematic = true;
            NowInput = INPUT_REVERSE;
        }
    }

    //ブロック一個上るセット
    private void SetBlockUp()
    {
        if (field.NowWay())//右にいる場合
        {
            Way = true;
        }
        else//左にいる場合
        {
            Way = false;
        }

        //ブロックが指定した方向にあるか
        if (!piller.GetPillerBlock(field.MovePillerID(Way), field.nowHeight) ||
            piller.GetPillerBlock(field.MovePillerID(Way), field.nowHeight + 1) ||
            piller.GetPillerBlock(field.nowPiller, field.nowHeight + 1))
        {
            return;
        }


        BlockUpDownFlag = BLOCK_UP;
        NowInput = INPUT_JUMP;
    }

    private void UpDownMove()
    {
        //回転中は登り降りしない
        if (field.StateReverse())
        {
            return;
        }

        if (BlockUpDownFlag == BLOCK_UP)//登る処理
        {
            BlockUpMove();
        }
        else if (BlockUpDownFlag == BLOCK_DOWN)
        {
            BlockDownMove();
        }
        
    }

    //ブロック登る処理
    private void BlockUpMove()
    {
        if (MoveFlag == MOVE_NONE)//登る開始
        {
            field.SetYMove(field.nowHeight + 1, jumpflame);
            MoveFlag = MOVE_UP;
        }
        else if (MoveFlag == MOVE_UP && !field.YMoveFlag)//登り中
        {
            field.SetXMove(field.MovePillerID(Way), jumpflame);//右移動開始
            MoveFlag = MOVE_X;
        }
        else if (MoveFlag == MOVE_X && !field.XMoveFlag)//右移動中
        {
            //ブロック登り終わり
            NowInput = INPUT_NONE;
            MoveFlag = MOVE_NONE;
            BlockUpDownFlag = BLOCK_NONE;
        }
    }


    //落下処理
    private void BlockDownMove()
    {
        if (MoveFlag == MOVE_NONE)//中央に移動中
        {
            if (field.nowHeight - 1 < 0)//0より小さい場合
            {
                MoveFlag = MOVE_NONE;
                BlockUpDownFlag = BLOCK_NONE;
                NowInput = INPUT_NONE;
            }
            else
            {
                if (NowInput != INPUT_REVERSE)
                {
                    field.SetCenterMove(jumpflame);
                    MoveFlag = MOVE_CENTER;
                    NowInput = INPUT_SET;
                }
                else
                {
                    NowInput = INPUT_SET;
                    MoveFlag = MOVE_CENTER;
                    field.SelectChangeHeight(field.nowHeight - 1);
                }
                
            }
        }
        else if (MoveFlag == MOVE_CENTER && !field.MoveCenter)
        {
            //下にブロックがない場合
            if (HitAir())
            {
                field.SelectChangeHeight(field.nowHeight - 1);
            }
            else
            {
                MoveFlag = MOVE_DOWN;
            }
        }
        else if (MoveFlag == MOVE_DOWN && this.GetComponent<Rigidbody>().isKinematic)
        {
            //着地した時
            MoveFlag = MOVE_NONE;
            BlockUpDownFlag = BLOCK_NONE;
            NowInput = INPUT_NONE;
            field.AirFlag = false;
        }
    }

    private void HitProcess()
    {
        var obj = piller.GetObjectMulti(field.nowPiller, field.nowHeight - 1);

        bool blockflag = false;
        foreach (var item in obj)
        {
            if (item.tag == "Block")
            {
                field.nowHeight = item.GetComponent<Field>().nowHeight + 1;
                blockflag = true;
                field.AirFlag = false;
                break;
            }
        }


        if (obj.Length == 0 || blockflag == false)
        {
            if (field.nowHeight <= 0)
            {
                field.nowHeight = 0;
                field.AirFlag = false;
            }
            else if (BlockUpDownFlag == BLOCK_NONE)
            {
                BlockUpDownFlag = BLOCK_DOWN;
            }
        }
    }

    //今空中にいるかいないか
    //戻り値　正で空中　負で地面
    private bool HitAir()
    {
        if (field.nowHeight <= 0)
        {
            return false;
        }

        var obj = piller.GetObjectMulti(field.nowPiller, field.nowHeight - 1);
        foreach (var item in obj)
        {
            if (item.tag == "Block")
            {
                return false;
            }
        }

        return true;
    }
}
