using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPiller : MonoBehaviour
{
    //回転速度
    public int ReturnFlame;

    public int size { get; set; }

    private Field field;//フィールド

    //柱マネージャー
    private PillerManager piller;

    //プレイヤー
    GameObject player;

    //回転
    public bool ReturnFlag { get; set; }
    private Quaternion Move;
    private Vector3 Axis;
    private float angle;

    private int flamecount;


    // Start is called before the first frame update
    void Start()
    {
        ReturnFlag = false;
        Move = Quaternion.identity;
        Axis = Vector3.zero;
        flamecount = 0;

        field = this.GetComponent<Field>();

        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();

        GameObject child = this.transform.Find("Cube").gameObject;
        child.transform.localScale = new Vector3(1.0f, size, 1.0f);

        player = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Reverse();
    }

    //表裏反転
    //Pillerid 柱ID
    //rotedirection　回転方向　true 後ろ　false 手前
    public void ReverseStart(bool rotedirection)
    {
        //回転軸
        //中心(自分と高さ同じ)までの方向ベクトル
        Axis = new Vector3(0.0f, this.transform.position.y, 0.0f) - this.transform.position;

        //回転軸を求める
        Axis = Vector3.Cross(Axis, Vector3.up);
        //Axis.Normalize();//なんとなく正規化

        //ブロック情報等を受け取る
        Transform[] set = new Transform[size * 2];
        for (int i = 0; i < set.Length; i++)
        {
            set[i] = null;
        }

        int childid = 0;
        foreach(Transform child in piller.FieldPiller[field.nowPiller].transform)
        {
            if (child.tag == "TurnPiller")
            {
                continue;
            }

            //プレイヤーを受け取る
            if (child.tag == "Player")
            {
                player = child.gameObject;
                continue;
            }

            int maxheight = field.nowHeight + size - 1;
            int minheight = field.nowHeight - size;
            Field cfield = child.GetComponent<Field>();
            if ((maxheight >= cfield.nowHeight && minheight <= cfield.nowHeight))
            {
                set[childid] = child;
                childid++;
            }
        }

        foreach (var child in set)
        {
            if (child == null)
            {
                continue;
            }
            child.transform.parent = this.transform;
        }



        //フラグ関係
        foreach (Transform child in this.transform)
        {
            if (child.name == "Cube")
            {
                continue;
            }
            Field field = child.GetComponent<Field>();
            field.FallFlag = false;
            field.AirFlag = true;
        }

        if (player != null)
        {
            Field field = player.GetComponent<Field>();
            field.FallFlag = false;
            field.AirFlag = true;
        }

        //回転処理
        angle = 180.0f / (float)ReturnFlame;
        if (rotedirection == false)
        {
            angle *= -1;
        }
        Move = Quaternion.AngleAxis(angle, Axis);//一回の回転量

        ReturnFlag = true;
    }

    //回転処理
    private void Reverse()
    {
        if (ReturnFlag == true)//回転している場合
        {

            //回転
            this.transform.rotation *= Move;
            if (player != null)
            {
                //Quaternion PMove = Quaternion.AngleAxis(-angle, Axis);//一回の回転量
                //Vector3 subposi = player.transform.position;//プレイヤー座標避難
                //player.transform.position = this.transform.position;//一回柱の位置にずらす
                //player.transform.rotation *= Move;//回転
                //player.transform.position = subposi;//戻す

                Vector3 center = this.transform.position - new Vector3(0.0f, 0.5f, 0.0f);

                Vector3 posi = player.transform.position;
                
                posi -= center;
                posi = Move * posi;
                posi += center;

                player.transform.position = posi;
                
            }

            //フレームカウント
            flamecount++;

            if (flamecount >= ReturnFlame)//フレームカウントが指定の数値を超えたら
            {

                //柱が逆さまになったのを回転前に戻す
                this.transform.Rotate(Axis, 180.0f);

                //ブロックフラグ関係
                Transform[] obj = new Transform[size * 2];
                int id = 0;
                foreach (Transform child in this.transform)
                {
                    //違うオブジェクトの時
                    if (child.name == "Cube")
                    {
                        continue;
                    }
                    Field field = child.GetComponent<Field>();
                    field.FallFlag = true;
                    field.ChangeHeight(this.field.nowHeight);
                    child.transform.position = new Vector3(child.transform.position.x, field.nowHeight, child.transform.position.z);
                    obj[id] = child;
                    id++;
                }

                if (player != null)
                {
                    player.GetComponent<Player>().turnpiller = null;

                    Field field = player.GetComponent<Field>();
                    field.FallFlag = true;
                    field.ChangeHeight(this.field.nowHeight);
                    player.transform.position = new Vector3(player.transform.position.x, field.nowHeight, player.transform.position.z);
                }

                foreach (var child in obj)
                {
                    if (child == null)
                    {
                        continue;
                    }
                    child.parent = this.transform.parent;
                }

                player = null;
                flamecount = 0;//フレームカウントリセット
                ReturnFlag = false;//回転する柱をリセット
            }
        }
    }
}
