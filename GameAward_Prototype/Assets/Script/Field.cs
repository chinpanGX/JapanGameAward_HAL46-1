using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int nowHeight; //{ set; get; }

    public int nowPiller;//{ get; set; }

    public int nextPiller;// { get; set; }

    public bool FallFlag;//{ get; set; }//落下フラグ

    //柱マネージャー
    private PillerManager piller;

    //フレームカウント
    private int FlameCount;
    private float MoveFlame;//移動フレーム
    
    //移動関係
    public bool YMoveFlag { get; set; }//移動フラグ true 移動中　false 移動してない
    private float YEndPosi;//移動先
    private float YMoveSpeed;//移動速度

    public bool XMoveFlag { get; set; }

    //中央に移動
    public bool MoveCenter { get; set; }

    //回転移動
    public bool DefoMoveFlag { get; set; }
    public Quaternion RotaMove { get; set; }
    public float moveaxis { get; set; }

    public float nowmoveaxis { get; set; }

    private void Awake()
    {
        DefoMoveFlag = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        YMoveFlag = false;
        YEndPosi = 0.0f;
        YMoveSpeed = 0.0f;

        XMoveFlag = false;
        MoveCenter = false;

        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        SetNoMove();

        this.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        this.Fall();
    }

    private void FixedUpdate()
    {
        Move();
    }

    //当たり判定
    void OnCollisionEnter(Collision collision)
    {
        if (FallFlag)
        {
            //rigitbodyの影響を受けなくする
            this.GetComponent<Rigidbody>().isKinematic = true;
        }

    }

    //落下制限
    private void Fall()
    {
        if (!FallFlag)
        {
            return;
        }

        //０以下にならないようにする
        if (this.transform.position.y <= 0.0f)
        {
            this.transform.position = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);
            this.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            this.GetComponent<Rigidbody>().isKinematic = false;
        }

        if (this.transform.position.y < nowHeight)//現在の高さより小さい場合
        {
            nowHeight--;
        }
    }

    

    private void Move()
    {
        if (YMoveFlag == true)//移動するとき
        {
            if (FlameCount >= MoveFlame)
            {
                //フラグ
                YMoveFlag = false;
                FallFlag = true;
                SelectChangeHeight(nowHeight + 1);
            }

            this.transform.localPosition += new Vector3(0.0f, YMoveSpeed, 0.0f);

            FlameCount++;
        }
        else if (XMoveFlag == true)
        {

            if ((nextPiller == nowPiller &&
                ((moveaxis >= 0.0f && !NowWay()) ||//左移動
                 (moveaxis <= 0.0f && NowWay()))))//右移動
            {
                FallFlag = true;
                XMoveFlag = false;
                SetNoMove();
            }
            else
            {
                ProcesMove();
            }

            //柱変更処理
            ProcesPiller();
        }
        else if (MoveCenter == true)
        {
            if (((moveaxis >= 0.0f && !NowWay()) ||//左移動
                 (moveaxis <= 0.0f && NowWay())))//右移動
            {
                FallFlag = true;
                MoveCenter = false;
                SetNoMove();
            }
            else
            {
                ProcesMove();
            }

            //柱変更処理
            ProcesPiller();
        }
        else if(DefoMoveFlag == true)
        {
            //左右移動制限処理
            MoveRest();

            //左右移動
            ProcesMove();

            //柱変更処理
            ProcesPiller();
        }

        
    }

    //高さを変更する
    //rotateheight 回転軸　高さ
    public void ChangeHeight(int rotateheight)
    {

        int range = rotateheight - nowHeight;
        nowHeight = rotateheight + range - 1;
        
    }

    public void SelectChangeHeight(int height)
    {
        nowHeight = height;
        if (this.name != "Player")
        {
            this.name = "Block" + nowHeight;//名前を書き換え
        }
    }

    //設定されているオブジェクトを直線移動する
    //戻り値　true 移動中　false 移動してない
    //endposi 移動先
    //flame 移動フレーム
    public bool SetYMove(float endposiY, int moveflame)
    {
        FlameCount = 0;
        YMoveFlag = true;
        FallFlag = false;
        MoveFlame = moveflame;
        YEndPosi = endposiY;
        YMoveSpeed = (YEndPosi - nowHeight) / (float)moveflame;
        return YMoveFlag;
    }

    //横自動移動
    //way true 右　false 左
    //moveflame 移動フレーム数
    public bool SetXMove(bool way, int moveflame)
    {
        XMoveFlag = true;
        FallFlag = false;
        nextPiller = MovePillerID(way);

        float XMoveSpeed = CalFlameMove(moveflame);


        if (way == true)
        {
            XMoveSpeed *= -1;
        }

        SetMove(XMoveSpeed);//移動角度セット

        return XMoveFlag;
    }

    public bool SetCenterMove(int moveflame)
    {
        MoveCenter = true;
        FallFlag = false;
        nextPiller = nowPiller;

        float XMoveSpeed = CalFlameMove(moveflame);

        if (NowWay() == true)
        {
            XMoveSpeed *= -1;
        }

        SetMove(XMoveSpeed);//移動角度セット

        return MoveCenter;
    }

    private float CalFlameMove(float moveflame)
    {
        float oneangle = 360.0f / (float)piller.Aroundnum;//柱一個分の移動角度
        float XMoveSpeed = oneangle / moveflame;//移動角度計算
        return XMoveSpeed = Mathf.Abs(XMoveSpeed);
    }

    //ブロックが本来いるはずの座標を返す
    private Vector3 MovePosition()
    {
        return new UnityEngine.Vector3(
            this.transform.localPosition.x, 
            (float)nowHeight - this.transform.parent.localPosition.y, 
            this.transform.localPosition.z);
    }

    //柱内の移動幅計算
    private void ProcesPiller()
    {
        //柱処理
        float pillerposi = ProcessPillerPosi();
        if (Mathf.Abs(nowmoveaxis) > pillerposi)//小さい
        {

            //変数の中身変更
            if (nowmoveaxis <= 0)//右にいる
            {
                nowmoveaxis = pillerposi + (nowmoveaxis + pillerposi);//移動幅リセット
                nowPiller = MovePillerID(true);//現在の柱変更
                this.transform.parent = piller.FieldPiller[nowPiller].transform;
            }
            else if (nowmoveaxis > 0)//左にいる
            {
                nowmoveaxis = -pillerposi + (nowmoveaxis - pillerposi);//移動幅リセット
                nowPiller = MovePillerID(false);//現在の柱
                this.transform.parent = piller.FieldPiller[nowPiller].transform;
            }
        }
    }

    //移動停止処理
    private bool MoveRest()
    {
        float nextmove = nowmoveaxis + moveaxis;//次の移動角度を計算
        if (Mathf.Abs(nextmove) + 3.0f > ProcessPillerPosi())
        {
            if ((NowWay() && piller.GetPillerBlock(MovePillerID(true), nowHeight)) || //右にいる
                (!NowWay() && piller.GetPillerBlock(MovePillerID(false), nowHeight)))   //左にいる
            {
                //移動しない
                SetNoMove();

                return false;
            }
        }

        return true;
    }

    //柱移動
    private float ProcessPillerPosi()
    {
        return ((360.0f / piller.Aroundnum) / 2.0f);
    }

    //柱の移動ID
    //true 右　false 左
    public int MovePillerID(bool way)
    {
        if (way)//右
        {
            return (nowPiller + (piller.Aroundnum + 1)) % piller.Aroundnum;
        }

        //左
        return (nowPiller + (piller.Aroundnum - 1)) % piller.Aroundnum;
    }

    //移動処理
    public void ProcesMove()
    {
        this.transform.position = RotaMove * this.transform.position;
        nowmoveaxis += moveaxis;
    }
    
    //移動登録
    //angle 移動角度
    public void SetMove(float angle)
    {
        RotaMove = Quaternion.AngleAxis(angle, Vector3.up);
        moveaxis = angle;
    }

    //移動しない処理
    public void SetNoMove()
    {
        RotaMove = Quaternion.identity;
        moveaxis = 0.0f;
    }

    public void SetNoDown()
    {
        SetNoMove();
        this.GetComponent<Rigidbody>().isKinematic = false;
        FallFlag = false;
    }

    //現在の柱の位置
    //true 柱の中心から右側にいる
    //false 柱の中心から左側にいる
    public bool NowWay()
    {
        if (nowmoveaxis <= 0.0f)
        {
            return true;
        }
        return false;
    }

    public bool StateReverse()
    {
        if (this.transform.parent.name.Contains("Turn"))
        {
            return true;
        }

        return false;
    }
    
}
