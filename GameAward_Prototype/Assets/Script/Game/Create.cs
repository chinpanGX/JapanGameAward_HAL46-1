using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : MonoBehaviour
{
    FloorManager floor;//階層
    PillerManager piller;//柱
    BlockManager block;//ブロック
    PlayerManager player;//プレイヤー


    private const float defoX = 4.45f;
    private const float defoY = 0.0f;
    private const float defoZ = 0.0f;

    Vector3 DefaultPosition = new Vector3(defoX, defoY, defoZ);



    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        floor = this.GetComponent<FloorManager>();
        piller = this.GetComponent<PillerManager>();
        block = this.GetComponent<BlockManager>();
        player = this.GetComponent<PlayerManager>();


        SetFloorData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(int Pilleerid, int height)
    {
        //移動量
        Quaternion move = CalQuaternion(Pilleerid, piller.Aroundnum);

        //座標計算
        Vector3 posi = CalPosition(move, DefaultPosition, height);
        Quaternion rotation = CalRotation(move, player.Player.transform.rotation);

        //セット
        player.Player.transform.parent = piller.Piller[Pilleerid].transform;
        player.Player.transform.position = posi;
        player.Player.transform.localPosition = new Vector3(0.0f, -2.0f, 0.0f);
        player.Player.transform.localRotation = rotation;
        Field field = player.Player.GetComponent<Field>();
        field.FallFlag = true;
        field.nowHeight = height;
        field.nowPiller = Pilleerid;
    }

    //階層を設定
    public void SetFloorData()
    {
        //新しい階層を作成
        floor.Floor = CreateObject("Floor" + floor.Floornum);

        //新しいフレーム作成
        GameObject flame = Instantiate(floor.FlameObject);
        flame.transform.parent = floor.Floor.transform;

        //新しい柱を作成
        SetPiller();

        //新しいブロックを作成
        //SetBlock(1, 3);
        //SetBlock(1, 1);
        //SetBlock(2, 0);
        //SetBlock(2, 2);


        SetBlock(1, 1);
        SetBlock(1, 2);
        SetBlock(1, 3);

        SetBlock(2, 0);
        SetBlock(2, 1);

        SetBlock(3, 1);
        SetBlock(3, 2);
        SetBlock(3, 3);

        SetBlock(4, 0);
        SetBlock(4, 1);
        SetBlock(4, 2);

        
        for (int i = 5; i < piller.Aroundnum; i++)
        {
            SetBlock(i, 0);
            SetBlock(i, 1);
            SetBlock(i, 2);
        }

        //上の階層にあげる
        floor.UpFloor();
    }

    //柱を設定
    private void SetPiller()
    {
        //柱配列作成
        piller.PrePiller();

        //柱情報設定
        for (int i = 0; i < piller.Aroundnum; i++)
        {
            piller.Piller[i] = CreateObject("Piller" + i);
            piller.Piller[i].transform.parent = floor.Floor.transform;

            Quaternion move = CalQuaternion(i, piller.Aroundnum);
            piller.Piller[i].transform.position = CalPosition(move, DefaultPosition, 2);
        }
    }

    //ブロック設定
    //Pillernum ブロックを生成するブロックの柱の位置
    //height ブロックの高さ　０スタート
    private void SetBlock(int PillerID, int Height)
    {
        int Pillerid = PillerID % piller.Aroundnum;
        int height = Height % 4;

        //回転計算
        Quaternion move = CalQuaternion(Pillerid, piller.Aroundnum);

        //Transformデータ
        Vector3 posi = CalPosition(move, DefaultPosition, height);
        Quaternion rotation = CalRotation(move, block.BlockObject.transform.rotation);

        //ブロック作成
        GameObject newblock = Instantiate(block.BlockObject);
        newblock.transform.parent = piller.Piller[Pillerid].transform;//親設定
        newblock.transform.position = posi;//座標設定
        newblock.transform.localPosition = new Vector3(0.0f, -2f, 0.0f);
        newblock.transform.rotation = rotation;//回転設定
        newblock.name = "Block" + height;//名前設定
        Field field = newblock.GetComponent<Field>();
        field.nowHeight = height;//高さ
        field.nowPiller = Pillerid;
        field.nextPiller = field.nowPiller;
        field.FallFlag = true;
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
}
