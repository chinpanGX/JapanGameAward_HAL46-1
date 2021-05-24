using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//移動速度
    public int jumpflame;
    private PillerManager piller;//柱情報
    private TurnPillerManager turnpillermane;
    public TurnPiller turnpiller { get; set; }

    //フィールド取得
    Field field;

    //アニメーション
    Animator animator;

    //見てる場所
    Vector3 look;//向いてる基準
    int nowlook;//向いてる方向

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    public int BlockUpDownFlag = BLOCK_NONE;//ブロック上り下りフラグ
    private bool Way;

    private const int MOVE_NONE = 0;
    private const int MOVE_UP = 1;//上る
    private const int MOVE_X = 2;//横移動
    private const int MOVE_CENTER = 3;//中央移動
    private const int MOVE_DOWN = 4;//落下
    private const int MOVE_SIDE = 5;//横指定移動
    public int MoveFlag = MOVE_NONE;

    private const int INPUT_NONE = 0;//入力してない
    private const int INPUT_SET = 1;//入力しているまたは何かしら処理中
    private const int INPUT_LEFT = 2;//左
    private const int INPUT_RIGHT = 3;//右
    private const int INPUT_REVERSE = 4;//回転
    private const int INPUT_JUMP = 5;
    private const int INPUT_STICK = 6;
    public int NowInput = INPUT_NONE;
    
    // Start is called before the first frame update
    void Start()
    {
        //マネージャー関係
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        turnpillermane = manager.GetComponent<TurnPillerManager>();

        //アニメーター
        GameObject playermodel = this.transform.Find("PlayerModel").gameObject;
        animator = playermodel.GetComponent<Animator>();

        //フィールド
        field = this.GetComponent<Field>();
        field.DefoMoveFlag = true;

        //変数初期化
        turnpiller = null;

        nowlook = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (turnpiller == null)
        {
            DownHitProcess();
        }

        //入力処理
        ProcesInput();

        //プレイヤー向き
        CalLook();
    }

    private void FixedUpdate()
    {
        
        
        //上移動
        UpDownMove();

    }

    //入力処理
    private void ProcesInput()
    {
        if (NowInput == INPUT_NONE && turnpiller == null && !field.AirFlag)
        {
            field.SetNoMove();

            var h = Input.GetAxis("Horizontal");

            if (h != 0)//移動
            {
                field.SetMove(-speed * h);
                if (h < 0)
                {
                    nowlook = 1;
                }
                else
                {
                    nowlook = -1;
                }
                animator.SetBool("Move", true);
                NowInput = INPUT_STICK;
            }
            else if (Input.GetButton("Right"))//右
            {
                SetMove(true);
            }
            else if (Input.GetButton("Left"))//左
            {
                SetMove(false);
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
                var h = Input.GetAxis("Horizontal");
                if ((NowInput == INPUT_LEFT && !Input.GetButton("Left")) ||
                (NowInput == INPUT_RIGHT && !Input.GetButton("Right") || 
                (NowInput == INPUT_STICK && h == 0))
                )
                {
                    animator.SetBool("Move", false);
                    NowInput = INPUT_NONE;
                }
                else if (NowInput == INPUT_REVERSE && turnpiller == null && !field.AirFlag)
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
        if (turnpiller != null)
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
            //field.SetYMove(field.nowHeight + 1, jumpflame);
            //MoveFlag = MOVE_UP;
            field.SetXMove(field.MovePillerID(Way), jumpflame);//横移動
            MoveFlag = MOVE_SIDE;
            animator.SetBool("Move", true);

            if (Way)
            {
                nowlook = 1;
            }
            else
            {
                nowlook = -1;
            }
        }
        else if(MoveFlag == MOVE_SIDE && field.MoveRest())//壁にぶつかったら
        {
            field.SetYMove(field.nowHeight + 1, jumpflame);
            MoveFlag = MOVE_UP;
            field.XMoveFlag = false;
            animator.SetBool("Move", false);
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

    private void DownHitProcess()
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
                animator.SetBool("Move", false);
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

    //移動
    //direction true=右　false=左
    void SetMove(bool direction)
    {
        if (direction)//右
        {
            field.SetMove(-speed);
            nowlook = 1;
            NowInput = INPUT_RIGHT;
        }
        else//左
        {
            field.SetMove(speed);
            nowlook = -1;
            NowInput = INPUT_LEFT;
        }

        animator.SetBool("Move", true);
    }

    //見る方向を計算するやぁつ
    void CalLook()
    {
        Vector3 center = new Vector3(0.0f, this.transform.position.y, 0.0f);
        Vector3 vec = center - this.transform.position;
        look = Vector3.Cross(Vector3.up, vec);
        look = Vector3.Normalize(look);
        this.transform.LookAt(this.transform.position + (nowlook * look));
    }
}
