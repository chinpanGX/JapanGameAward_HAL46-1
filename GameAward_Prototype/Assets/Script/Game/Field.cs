using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int nowHeight { set; get; }


    public int nowPiller { get; set; }

    public bool FallFlag { get; set; }//落下フラグ

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.Fall();
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


    //指定した高さに変更
    //height  変更する高さ
    public int SelectChangeHeight(int height)
    {
        int prevheight = nowHeight;//前の高さを保存
        nowHeight = height;//変更する高さを変更

        return prevheight;
    }

    //ブロックが本来いるはずの座標を返す
    private Vector3 MovePosition()
    {
        return new Vector3(
            this.transform.localPosition.x, 
            (float)nowHeight - this.transform.parent.localPosition.y, 
            this.transform.localPosition.z);
    }
}
