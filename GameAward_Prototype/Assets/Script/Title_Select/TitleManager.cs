using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] BlockMove SelectIcon;//選択アイコン

    //タイトル
    [SerializeField] GameObject title;
    private Vector3[] TitleObj;

    private const int SELECT_RETURN2 = -3;
    private const int SELECT_RETURN = -2;//ステージ選択から戻ってきたとき
    private const int SELECT_NONE = -1;
    private const int SELECT_NEWGAME = 0;
    private const int SELECT_CONTINU = 1;
    private const int SELECT_EXIT = 2;
    private int select = SELECT_NEWGAME;
    private int nextselect;

    private AudioController titleaudio;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクト受け取る
        TitleObj = new Vector3[3];//3個作成
        TitleObj[0] = title.transform.Find("NewGame").position;//ニューゲーム
        TitleObj[1] = title.transform.Find("Continue").position;//コンティニュー
        TitleObj[2] = title.transform.Find("Exit").position;//出口

        //セーブデータがある場合コンティニューにアイコンを合わせる
        if (ScoreSave.IsSavaData())
        {
            select = SELECT_CONTINU;
        }

        //アイコンの座標を設定
        Vector3 icon = SelectIcon.transform.position;
        SelectIcon.transform.position = new Vector3(icon.x, TitleObj[select].y, icon.z);

        //次の選択を今ので上書き
        nextselect = select;

        title.SetActive(true);

        //フラグ
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_GAME)
        {
            StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;
            titleaudio = AudioManager.PlayAudio("Title", true, true);
        }
        else if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_TITLE)
        {
            titleaudio = AudioManager.PlayAudio("Title", true, true);
        }

        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_STAGESELECT)
        {
            title.SetActive(false);
            select = SELECT_RETURN;
            nextselect = select;

            title.transform.position = new Vector3(0.0f, 10.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_TITLE )
        {
            if (select == nextselect && select > SELECT_NONE && !title.GetComponent<BlockMove>().moveflag && StatusFlagManager.TitleSelectFlag == StatusFlagManager.TS_PLAY)
            {
                var v = Input.GetAxis("Vertical");
                if (v > 0.0f || Input.GetKey(KeyCode.W))//上
                {
                    if (select != SELECT_NEWGAME)//一番上ではない場合
                    {
                        nextselect = select - 1;
                        SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                }
                else if (v < 0.0f || Input.GetKey(KeyCode.S))//下
                {
                    if (select != SELECT_EXIT)//一番下ではない場合
                    {
                        nextselect = select + 1;
                        SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                }
                else if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Space))//決定
                {
                    if (select == SELECT_NEWGAME)//ニューゲーム
                    {
                        select = SELECT_NONE;
                        nextselect = select;

                        BlockMove block = title.GetComponent<BlockMove>();
                        block.StartMove(new Vector3(title.transform.position.x, 10.0f, title.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                        ScoreSave.SavaDataClear();
                        ScoreSave.SavaDataDelete();
                    }
                    else if (select == SELECT_CONTINU && ScoreSave.IsSavaData())//コンティニュー
                    {
                        select = SELECT_NONE;
                        nextselect = select;
                        BlockMove block = title.GetComponent<BlockMove>();
                        block.StartMove(new Vector3(title.transform.position.x, 10.0f, title.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                    else if (select == SELECT_EXIT)//終了
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
                        select = SELECT_RETURN;
                        nextselect = select;

                        titleaudio.FadeOutStart();
                        titleaudio = null;
                    }
                }
                else if(select == SELECT_RETURN)//戻ってきたとき
                {
                    title.SetActive(true);

                    //セーブデータがある場合コンティニューにアイコンを合わせる
                    if (ScoreSave.IsSavaData())
                    {
                        select = SELECT_CONTINU;
                    }
                    else
                    {
                        select = 0;
                    }

                    BlockMove block = title.GetComponent<BlockMove>();
                    block.StartMove(new Vector3(0.0f, 0.0f, 0.0f));

                    SelectIcon.StartMove(new Vector3(-1, TitleObj[select].y, SelectIcon.transform.position.z));

                    select = SELECT_RETURN2;
                    nextselect = select;

                    titleaudio = AudioManager.PlayAudio("Title", true, true);
                }
                else if (select == SELECT_RETURN2 && !title.GetComponent<BlockMove>().moveflag)
                {
                    if (ScoreSave.IsSavaData())
                    {
                        select = SELECT_CONTINU;
                    }
                    else
                    {
                        select = SELECT_NEWGAME;
                    }
                    nextselect = select;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (select == SELECT_RETURN2)
        {
            GameObject iconimage = SelectIcon.transform.Find("Canvas").Find("Image").gameObject;
            if (iconimage.transform.eulerAngles.z == 0.0f)
            {
                iconimage.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            }
            iconimage.transform.rotation = Quaternion.RotateTowards(iconimage.transform.rotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), 180.0f / 14.0f);
        }
    }
}
