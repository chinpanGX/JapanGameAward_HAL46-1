using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Create : MonoBehaviour
{
    PillerManager piller;//柱
    BlockManager block;//ブロック
    [SerializeField] GameObject player;//プレイヤー
    TurnPillerManager turnpiller;//回転柱


    private const float defoX = 4.45f;
    private const float defoY = 0.0f;
    private const float defoZ = 0.0f;

    Vector3 DefaultPosition = new Vector3(defoX, defoY, defoZ);


    [SerializeField] StageManager stagemanager;

    // Start is called before the first frame update
    void Start()
    {
        piller = this.GetComponent<PillerManager>();
        block = this.GetComponent<BlockManager>();
        turnpiller = this.GetComponent<TurnPillerManager>();

        //ステージ柱作成
        SetFieldPiller();

        //プレイヤーセット
        stagemanager.GetStage(StatusFlagManager.SelectStageID).SetPlayer(this);

        //回転柱セット
        stagemanager.GetStage(StatusFlagManager.SelectStageID).SetTurnPiller(this);

        //ブロックセット
        stagemanager.GetStage(StatusFlagManager.SelectStageID).SetBlock(this);
    }

    private void SetFieldPiller()
    {
        //ステージ柱作成
        piller.PreFieldPiller();

        for (int i = 0; i < piller.Aroundnum; i++)
        {
            piller.FieldPiller[i] = CreateObject("FieldPiller" + i);
            piller.FieldPiller[i].tag = "Piller";
        }
    }

    //プレイヤー配置
    //side 横座標
    //height　高さ座標
    public void SetPlayer(int side, int height)
    {
        side = CalPillerid(side);

        //移動量
        Quaternion move = CalQuaternion(side, piller.Aroundnum);

        //座標計算
        Vector3 posi = CalPosition(move, DefaultPosition, height);
        Quaternion rotation = CalRotation(move, player.transform.rotation);

        //セット
        player.transform.parent = piller.FieldPiller[side].gameObject.transform;//親設定
        player.transform.position = posi;//座標
        player.transform.rotation = rotation;//回転
        Field field = player.GetComponent<Field>();//フィールド受け取る
        field.FallFlag = true;//落下フラグ
        field.nowHeight = height;//高さ
        field.nowPiller = side;
    }

    //回転柱設定
    public GameObject CreateTurnPiller(int side, int height, int size)
    {
        //柱位置計算
        side = CalPillerid(side);
        Quaternion move = CalQuaternion(side, piller.Aroundnum);
        Vector3 posi = CalPosition(move, DefaultPosition, height);

        //オブジェクト作成
        GameObject obj = Instantiate(turnpiller.PillerPrefub);
        obj.name = "Turn" + side + "_" + height;
        obj.transform.parent = piller.FieldPiller[side].gameObject.transform;
        obj.transform.position = posi;
        obj.GetComponent<CapsuleCollider>().height = size * 2.0f;
        Field field = obj.GetComponent<Field>();
        field.FallFlag = false;
        field.nowHeight = height;
        field.nowPiller = side;
        TurnPiller turnPiller = obj.GetComponent<TurnPiller>();
        turnPiller.size = size;


        turnpiller.SetPiller(obj, side, height);

        return obj;
    }

    //ブロック設定
    //side 横位置
    //height 高さ
    public void CreateBlock(int side, int height)
    {
        side = CalPillerid(side);

        //回転計算
        Quaternion move = CalQuaternion(side, piller.Aroundnum);

        //Transformデータ
        Vector3 posi = CalPosition(move, DefaultPosition, height);
        Quaternion rotation = CalRotation(move, block.BlockObject.transform.rotation);

        //ブロック作成
        GameObject newblock = Instantiate(block.BlockObject);
        newblock.transform.parent = piller.FieldPiller[side].gameObject.transform;
        newblock.transform.position = posi;//座標設定
        newblock.transform.rotation = rotation;//回転設定
        newblock.name = "Block" + side + "_" + height;//名前設定
        Field field = newblock.GetComponent<Field>();
        field.nowHeight = height;//高さ
        field.FallFlag = false;
        field.nowPiller = side;
    }

    //空のオブジェクト作成
    private GameObject CreateObject(string name)
    {
        return new GameObject(name);
    }

    //移動する柱の角度計算　クォータニオン
    //Pillernumは柱番号　0にすれば今の位置のまま
    //AroundNum　柱の数
    static public Quaternion CalQuaternion(int Pillerid, int Aroundnum)
    {
        //柱一個分の角度
        float OnePiller = 360.0f / (float)Aroundnum;
        Quaternion quaternion = Quaternion.Euler(CalVector3(Pillerid, Aroundnum));

        return quaternion;
    }

    //移動する柱の角度計算 オイラー角
    //Pillernumは柱番号　0にすれば今の位置のまま
    //AroundNum　柱の数
    static public Vector3 CalVector3(int Pillerid, int Aroundnum)
    {
        float OnePiller = 360.0f / (float)Aroundnum;
        return new Vector3(0.0f, -OnePiller * (float)Pillerid, 0.0f);
    }

    //座標計算
    //move CalQuaternionで計算した値
    //baceposi 座標基準
    //height 高さ0~3
    private Vector3 CalPosition(Quaternion move, Vector3 baceposi, int height)
    {
        Vector3 newposi = baceposi + new Vector3(0.0f, (float)height, 0.0f);
        return move * newposi;
    }

    //回転計算
    //move CalQuaternionで計算した値
    //BaceRotation 回転基準
    private Quaternion CalRotation(Quaternion move, Quaternion BaceRotation)
    {
        return move * BaceRotation;
    }

    private int CalPillerid(int pillerid)
    {
        return pillerid % piller.Aroundnum;
    }
}
