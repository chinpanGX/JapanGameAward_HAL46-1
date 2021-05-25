using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] BlockMove SelectIcon;//選択アイコン

    //タイトル
    [SerializeField] GameObject title;
    private Vector3[] TitleObj;

    private const int SELECT_NONE = -1;
    private const int SELECT_NEWGAME = 0;
    private const int SELECT_CONTINU = 1;
    private const int SELECT_EXIT = 2;
    private int select = SELECT_NEWGAME;
    private int nextselect;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクト受け取る
        TitleObj = new Vector3[3];//3個作成
        TitleObj[0] = title.transform.Find("NewGame").position;//ニューゲーム
        TitleObj[1] = title.transform.Find("Continue").position;//コンティニュー
        TitleObj[2] = title.transform.Find("Exit").position;//出口

        //アイコンの座標を設定
        Vector3 icon = SelectIcon.transform.position;
        SelectIcon.transform.position = new Vector3(icon.x, TitleObj[select].y, icon.z);

        //次の選択を今ので上書き
        nextselect = select;

        //タイトルをアクティブをtrueにする
        title.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (select == nextselect && StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_TITLE && select != SELECT_NONE)
        {
            var h = Input.GetAxis("Vertical");
            if (h > 0.0f || Input.GetKey(KeyCode.W))//上
            {
                if (select != SELECT_NEWGAME)//一番上ではない場合
                {
                    nextselect = select - 1;
                    SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                }
            }
            else if (h < 0.0f ||Input.GetKey(KeyCode.S))//下
            {
                if (select != SELECT_EXIT)//一番下ではない場合
                {
                    nextselect = select + 1;
                    SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                }
            }
            else if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Space))//決定
            {
                if (select == SELECT_NEWGAME)
                {
                    select = SELECT_NONE;
                    nextselect = select;

                    BlockMove block = title.GetComponent<BlockMove>();
                    block.StartMove(new Vector3(title.transform.position.x, 10.0f, title.transform.position.z));
                }
                else if (select == SELECT_CONTINU)
                {
                    select = SELECT_NONE;
                    nextselect = select;
                    BlockMove block = title.GetComponent<BlockMove>();
                    block.StartMove(new Vector3(title.transform.position.x, 10.0f, title.transform.position.z));
                }
                else if (select == SELECT_EXIT)
                {
                    UnityEngine.Application.Quit();
                }
            }
        }
        else
        {
            if (select != nextselect)
            {
                if (!SelectIcon.moveflag)
                {
                    select = nextselect;
                }
            }
            else if (select == SELECT_NONE)
            {
                BlockMove block = title.GetComponent<BlockMove>();
                if (!block.moveflag)
                {
                    title.SetActive(false);
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_STAGESELECT;
                }
            }
        }
    }
}
