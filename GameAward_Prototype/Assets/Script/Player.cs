using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//移動速度
    public int jumpflame;
    private PillerManager piller;//柱情報
    private TurnPillerManager turnpiller;

    //フィールド取得
    Field field;

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    private int BlockUpDownFlag = BLOCK_NONE;//ブロック上り下りフラグ
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
    private int NowInput = INPUT_NONE;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        turnpiller = manager.GetComponent<TurnPillerManager>();


        field = this.GetComponent<Field>();
        field.DefoMoveFlag = true;
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

    private void OnTriggerEnter(Collider collider)
    {
        field.SetNoMove();
        Debug.Log("当たった");
        if(collider.gameObject.tag == "Magma")
        {
            Debug.Log("ボボボ");
        }
    }

    //入力処理
    private void ProcesInput()
    {
        if (NowInput == INPUT_NONE)
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
            //else if (Input.GetButtonDown("Reverce"))//回転
            //{
            //    SetReverse();
            //}
            //else if (Input.GetButtonDown("Jump"))//ブロック上る
            //{
            //    SetBlockUp();
            //}
            //else if (Input.GetKey(KeyCode.P))//デバッグ用長押しで回転し続けるやつ
            //{
            //    SetReverse();
            //}
        }
        else
        {
            if ((NowInput == INPUT_LEFT && !Input.GetButton("Left")) ||
                (NowInput == INPUT_RIGHT && !Input.GetButton("Right"))
                )
            {
                NowInput = INPUT_NONE;
            }
        }
    }

    //回転処理セット
    private void SetReverse()
    {
        turnpiller.ReverseStart(this.GetComponent<Field>().nowPiller, false);
        NowInput = INPUT_REVERSE;
    }

    //ブロック一個上るセット
    private void SetBlockUp()
    {
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
        NowInput = INPUT_JUMP;
    }

    private void UpDownMove()
    {
        //回転中は登り降りしない
        if (turnpiller.StateReverce())
        {
            return;
        }

        if (BlockUpDownFlag == BLOCK_NONE && field.nowHeight > 0)//高さが0以上の時
        {
            BlockUpDownFlag = BLOCK_DOWN;
            NowInput = INPUT_NONE;
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
            field.SetXMove(Way, jumpflame);//右移動開始
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
            else//落下開始
            {
                MoveFlag = MOVE_DOWN;
                NowInput = INPUT_SET;
            }
        }
        else if (MoveFlag == MOVE_DOWN && this.GetComponent<Rigidbody>().isKinematic)//空中にいる時
        {
            //着地した時
            MoveFlag = MOVE_NONE;
            BlockUpDownFlag = BLOCK_NONE;
            NowInput = INPUT_NONE;
        }
    }

    
}
