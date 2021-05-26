using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] ScoreManager score;

    public float speed;//移動速度
    public int jumpflame;
    public PillerManager piller { get; set; }//柱情報
    private TurnPillerManager turnpillermane;
    public TurnPiller turnpiller { get; set; }

    //フィールド取得
    Field field;

    //アニメーション
    Animator animator;

    //ボタン合図UI
    public GameObject Abutton;
    public GameObject Bbutton;

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
    private int MoveFlag = MOVE_NONE;

    private const int INPUT_NONE = 0;//入力してない
    private const int INPUT_SET = 1;//入力しているまたは何かしら処理中
    private const int INPUT_LEFT = 2;//左
    private const int INPUT_RIGHT = 3;//右
    private const int INPUT_REVERSE = 4;//回転
    private const int INPUT_JUMP = 5;
    private const int INPUT_STICK = 6;
    private int NowInput = INPUT_NONE;


    private const int CLEAR_NONE = 0;//まだ
    private const int CLEAR_CENTER = 1;//中央に移動
    private const int CLEAR_ANIME = 2;//アニメーション
    private const int CLEAR_ROTA1 = 3;//回転1
    private const int CLEAR_ROTA2 = 4;//回転2
    public int ClearFlag = CLEAR_NONE;
    private Vector3 clearmove;
    
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

        //ボタン操作UI
        Abutton.SetActive(false);
        Bbutton.SetActive(false);

        nowlook = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_GAME || StatusFlagManager.GameStatusFlag != StatusFlagManager.GAME_PLAY)
        {
            return;
        }

        if (ClearFlag == CLEAR_NONE)
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
    }

    private void FixedUpdate()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_GAME)
        {
            if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_PLAY)
            {
                if (ClearFlag == CLEAR_NONE)
                {
                    //上移動
                    UpDownMove();
                }

                SetClear();
            }
        }

        if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_CLEAR)
        {
            ClearMove();
        }
    }

    //入力処理
    private void ProcesInput()
    {
        //回転＆ジャンプボタン押せるか？
        ButtunUI();


        if (NowInput == INPUT_NONE && turnpiller == null && !field.AirFlag)
        {
            //移動リセット
            field.SetNoMove();
            var h = Input.GetAxis("Horizontal");
            if (h >= 0.9f || h <= -0.9f)//移動
            {
                field.SetMove(-speed * h);
                if (h < 0)
                {
                    nowlook = -1;
                }
                else
                {
                    nowlook = 1;
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
            else
            {
                animator.SetBool("Move", false);
            }
            
            if (Input.GetButton("Reverce"))//回転
            {
                if (SetReverse())
                {
                    field.SetNoMove();
                    score.CountAction();
                }
            }
            else if (Input.GetButton("Jump"))//ブロック上る
            {
                if (SetBlockUp())
                {
                    field.SetNoMove();
                    score.CountAction();
                }
            }
        }
        else
        {
            if (BlockUpDownFlag == BLOCK_NONE)
            {
                if (NowInput == INPUT_STICK)//スティック入力
                {
                    var h = Input.GetAxis("Horizontal");
                    if (h <= 0.9f && h >= -0.9f)
                    {
                        animator.SetBool("Move", false);
                        NowInput = INPUT_NONE;
                    }
                    else
                    {
                        NowInput = INPUT_NONE;
                    }
                }
                else if (NowInput == INPUT_REVERSE && turnpiller == null && !field.AirFlag)//回転処理
                {
                    NowInput = INPUT_NONE;
                }
                else if (NowInput == INPUT_LEFT)//右移動
                {
                    if (Input.GetButton("Left"))
                    {
                        NowInput = INPUT_NONE;
                    }
                    else
                    {
                        animator.SetBool("Move", false);
                        NowInput = INPUT_NONE;
                    }

                }
                else if (NowInput == INPUT_RIGHT)//左ボタン
                {
                    if (Input.GetButton("Right"))
                    {
                        NowInput = INPUT_NONE;
                    }
                    else
                    {
                        animator.SetBool("Move", false);
                        NowInput = INPUT_NONE;
                    }
                }
            }
            
        }
    }

    //回転処理セット
    private bool SetReverse()
    {
        GameObject obj = turnpillermane.StartReverse(field.nowPiller, field.nowHeight);
        if (obj != null)
        {
            turnpiller = obj.GetComponent<TurnPiller>();
            this.GetComponent<Rigidbody>().isKinematic = true;
            NowInput = INPUT_REVERSE;
            return true;
        }
        return false;
    }

    //ブロック一個上るセット
    private bool SetBlockUp()
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
            return false;
        }


        BlockUpDownFlag = BLOCK_UP;
        NowInput = INPUT_JUMP;

        return true;
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

            if (!field.MoveRest())
            {
                field.SetXMove(field.MovePillerID(Way), jumpflame);//横移動
                MoveFlag = MOVE_SIDE;
                animator.SetBool("Move", true);
            }
            else
            {
                field.SetYMove(field.nowHeight + 1, jumpflame);
                MoveFlag = MOVE_UP;
                field.XMoveFlag = false;
                animator.SetBool("Move", false);
            }
            

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

    void ButtunUI()
    {
        Abutton.SetActive(false);
        Bbutton.SetActive(false);

        if (turnpiller == null && !field.AirFlag && BlockUpDownFlag == BLOCK_NONE)
        {
            if (JudgeReverce())//回転
            {
                Abutton.SetActive(true);
            }

            if (JudgeJump())//ブロック上る
            {
                Bbutton.SetActive(true);
            }
        }
    }

    //判定回転
    bool JudgeReverce()
    {
        return turnpillermane.JudgePiller(field.nowPiller, field.nowHeight);
    }

    //判定ジャンプ
    bool JudgeJump()
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
            return false;
        }

        return true;
    }

    private void SetClear()
    {
        //最上階　回転してない　空中にいない　移動フラグが動いてない
        if (field.nowHeight == 20 && turnpiller == null && !field.AirFlag && BlockUpDownFlag == BLOCK_NONE)
        {
            if (ClearFlag == CLEAR_NONE)//クリア処理開始
            {
                //座標補正
                this.transform.position = new Vector3(this.transform.position.x, 20.0f, this.transform.position.z);

                //重力の影響を受けなくする
                this.GetComponent<Rigidbody>().isKinematic = true;

                //落下処理をしなくする
                field.FallFlag = false;

                //移動方向を計算する
                clearmove = new Vector3(0.0f, this.transform.position.y, 0.0f) - this.transform.position;
                clearmove = Vector3.Normalize(clearmove);

                //カメラをプレイヤーの子に入れる
                Camera.main.transform.parent = this.transform;

                //フラグ変更
                ClearFlag = CLEAR_ROTA1;
                animator.SetBool("Move", true);
                //field.SetNoMove();
                StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_CLEAR;
            }
        }
    }

    private void ClearMove()
    {
        if (ClearFlag == CLEAR_ROTA1)//最初の回転
        {
            //中央に向くように回転
            Quaternion target = Quaternion.LookRotation(clearmove);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target, 2.0f);
            if (target == this.transform.rotation)//中央に向いたら
            {
                ClearFlag = CLEAR_CENTER;
            }
        }
        else if (ClearFlag == CLEAR_CENTER)//中央移動
        {
            //移動
            this.transform.position += clearmove * 0.1f;

            //中央に移動したら
            if (this.transform.position.x <= 0.1f && this.transform.position.x >= -0.1f)
            {
                if (this.transform.position.z <= 0.1f && this.transform.position.z >= -0.1f)
                {
                    this.transform.position = new Vector3(0.0f, 20.0f, 0.0f);
                    Camera.main.transform.parent = null;
                    ClearFlag = CLEAR_ROTA2;
                }
            }
        }
        else if (ClearFlag == CLEAR_ROTA2)
        {
            Quaternion target = Quaternion.LookRotation(new Vector3(Camera.main.transform.position.x, 20.0f, Camera.main.transform.position.z) - this.transform.position);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target, 2.0f);
            StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_RESULT;
            if (target.y == this.transform.rotation.y)
            {
                ClearFlag = CLEAR_ANIME;
                animator.SetBool("Move", false);
            }
        }
        else if (ClearFlag == CLEAR_ANIME)
        {
            StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_RESULT;
        }
    }
}
