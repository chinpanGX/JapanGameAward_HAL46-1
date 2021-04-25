using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int nowHeight { set; get; }

    public int nowPiller { get; set; }

    public int nextPiller { get; set; }

    public bool FallFlag { get; set; }//落下フラグ

    //柱マネージャー
    private PillerManager piller;

    //フレームカウント
    private int FlameCount;
    private float MoveFlame;//移動フレーム
    
    //移動関係
    private bool YMoveFlag { get; set; }//移動フラグ true 移動中　false 移動してない
    private float EndPosi;//移動先
    private float MoveSpeed;//移動速度

    //回転移動
    private bool XMoveFlag { get; set; }
    public Quaternion RotaMove { get; set; }
    public float moveaxis { get; set; }

    public float nowmoveaxis { get; set; }



    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        SetNoMove();
    }

    // Update is called once per frame
    void Update()
    {
        this.Fall();

        if (Input.GetKeyDown(KeyCode.K))
        {
            SetYMove(nowHeight + 1, 10);
        }
    }

    private void FixedUpdate()
    {
        JumpMove();
    }

    //落下制限
    private void Fall()
    {
        if (!FallFlag)
        {
            return;
        }

        //柱のローカル座標のYが+2ずれているため補正する
        if (this.transform.localPosition.y + this.transform.parent.localPosition.y <= (float)nowHeight)
        {

            //座標固定
            this.transform.localPosition = MovePosition();

            //rigitbodyの影響を受けなくする
            this.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            //rigitbodyの影響を受けるようにする
            this.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void JumpMove()
    {
        if (YMoveFlag == true)//移動するとき
        {
            if (FlameCount >= MoveFlame)
            {
                //フラグ
                YMoveFlag = false;
                FallFlag = true;
                nowHeight++;
                this.name = "Block" + nowHeight;
            }

            this.transform.localPosition += new Vector3(0.0f, MoveSpeed, 0.0f);

            FlameCount++;
        }
        else if (XMoveFlag == true)
        {

        }
    }

    //高さを変更する
    public void ChangeHeight()
    {
        if (nowHeight == 0)//一番下から一番上
        {
            nowHeight += 3;
        }
        else if (nowHeight == 1)//真ん中下から上
        {
            nowHeight += 1;
        }
        else if (nowHeight == 2)//真ん中上から下
        {
            nowHeight -= 1;
        }
        else if (nowHeight == 3)//一番上から一番下
        {
            nowHeight -= 3;
        }

        this.name = "Block" + nowHeight;//名前を書き換え
        this.transform.localPosition = MovePosition();//座標を変更
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
        EndPosi = endposiY;
        MoveSpeed = (EndPosi - nowHeight) / (float)moveflame;
        return YMoveFlag;
    }

    public bool SetAutoMove(int movepillerid, int moveflame)
    {
        FlameCount = 0;
        XMoveFlag = true;
        FallFlag = false;
        MoveFlame = moveflame;

        Vector3 endangle = Create.CalQuaternion(movepillerid, piller.Aroundnum).eulerAngles;//オイラーもらう
        Vector3 thisangle = this.transform.rotation.eulerAngles;

        float SAngle = (endangle.y - thisangle.y) / moveflame;//移動角度計算

        RotaMove = Quaternion.AngleAxis(SAngle, Vector3.up);

        return XMoveFlag;
    }

    //ブロックが本来いるはずの座標を返す
    private Vector3 MovePosition()
    {
        return new UnityEngine.Vector3(
            this.transform.localPosition.x, 
            (float)nowHeight - this.transform.parent.localPosition.y, 
            this.transform.localPosition.z);
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
    
}
