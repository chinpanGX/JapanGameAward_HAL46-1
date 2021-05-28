using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] GameObject icon;
    private GameObject iconimage;
    private float turnicon;

    [SerializeField] GameObject select;//ステージセレクト
    private GameObject[] stageobj;
    private GameObject[] stagetext;

    [SerializeField] GameObject arrowdown;
    [SerializeField] GameObject arrowup;

    public int selectstageid;//現在選択してるステージID

    public int nowselect;
    private int nextselect;

    private int changemove;
    private int changetext;

    AudioController selectaudio;

    // Start is called before the first frame update
    void Start()
    {
        //ステージブロック受け取る
        stageobj = new GameObject[6];
        stagetext = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            stageobj[i] = select.transform.Find("Stage0" + (i + 1)).gameObject;
            stagetext[i] = stageobj[i].transform.Find("Canvas").gameObject;
            stagetext[i].transform.Find("Text").GetComponent<Text>().text = "Stage " + (i + 1);
        }
        changetext = 0;

        //アイコンイメージ受け取る
        iconimage = icon.transform.Find("Canvas").Find("Image").gameObject;

        //ステージセレクト初期位置
        select.transform.position = new Vector3(0.0f, 10.0f, 0.0f);

        //ステージセレクト非アクティブ
        select.SetActive(false);

        //現在選択してるステージ
        selectstageid = -1;

        nowselect = selectstageid;
        nextselect = 0;

        //フラグ
        turnicon = -1.0f;
        changemove = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_STAGESELECT && StatusFlagManager.TitleSelectFlag == StatusFlagManager.TS_PLAY)
        {
            if (nowselect == nextselect && !select.GetComponent<BlockMove>().moveflag)
            {
                var h = Input.GetAxis("Horizontal");
                var v = Input.GetAxis("Vertical");
                if (v > 0.5f || Input.GetKey(KeyCode.W))//上
                {
                    if (nowselect > 1)//一番上の時
                    {
                        selectstageid -= 2;

                        nextselect = nowselect - 2;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(icon.transform.position.x, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                    else if (selectstageid > 1)
                    {
                        selectstageid -= 2;
                        nextselect = nowselect + 4;
                        changetext -= 6;
                        changemove = 1;

                        select.GetComponent<BlockMove>().StartMove(new Vector3(0.0f, 10.0f, 0.0f));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                }
                else if (v < -0.5f || Input.GetKey(KeyCode.S))//下
                {
                    if (nowselect < 4)//一番上の時
                    {
                        selectstageid += 2;

                        nextselect = nowselect + 2;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(icon.transform.position.x, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                    else if(selectstageid < StatusFlagManager.StageMaxNum - 2)
                    {
                        selectstageid += 2;
                        nextselect = nowselect - 4;
                        changetext += 6;
                        changemove = 1;

                        select.GetComponent<BlockMove>().StartMove(new Vector3(0.0f, 10.0f, 0.0f));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                }
                else if (h < -0.5f || Input.GetKey(KeyCode.A))//左
                {
                    if (nowselect % 2 == 1)
                    {
                        selectstageid -= 1;

                        turnicon = 0.0f;
                        nextselect = nowselect - 1;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x + 2, icon.transform.position.y, icon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                    //else if(nowselect != 0)
                    //{
                    //    selectstageid -= 1;

                    //    turnicon = 180.0f;
                    //    nextselect = nowselect - 1;
                    //    icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x - 2, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                    //}
                }
                else if (h > 0.5f || Input.GetKey(KeyCode.D))//右
                {
                    if (nowselect % 2 != 1)
                    {
                        selectstageid += 1;

                        turnicon = 180.0f;
                        nextselect = nowselect + 1;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x - 2, icon.transform.position.y, icon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                    //else if (nowselect != 5)
                    //{
                    //    selectstageid += 1;

                    //    turnicon = 0.0f;
                    //    nextselect = nowselect + 1;
                    //    icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x + 2, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                    //}
                }
                else if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Space))
                {
                    selectaudio.FadeOutStart();
                    selectaudio = null;

                    StatusFlagManager.TitleSelectFlag = StatusFlagManager.TS_START;
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
                    StatusFlagManager.SelectStageID = selectstageid;
                    StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_START;
                    Fade.FadeOut("SampleScene");
                    AudioManager.PlayAudio("IconMove", false, false);
                }
                else if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.X))//戻るBボタン
                {
                    changemove = 2;
                    nextselect = -1;
                    select.GetComponent<BlockMove>().StartMove(new Vector3(0.0f, 10.0f, 0.0f));
                    AudioManager.PlayAudio("IconMove", false, false);
                }
            }
            else if (nowselect != nextselect)
            {
                if (selectstageid != -1)//ステージ選択中
                {
                    if (changemove == 1 && !select.GetComponent<BlockMove>().moveflag)//ステージブロックチェンジ
                    {
                        changemove = 0;
                        for (int i = 0; i < 6; i++)
                        {
                            stagetext[i].transform.Find("Text").GetComponent<Text>().text = "Stage " + (i + 1 + changetext);
                        }
                        select.GetComponent<BlockMove>().StartMove(new Vector3(0.0f, 0.0f, 0.0f));
                        if (nowselect <= 1)
                        {
                            icon.GetComponent<BlockMove>().StartMove(new Vector3(icon.transform.position.x, 0.1f, icon.transform.position.z));
                        }
                        else if (nowselect >= 4)
                        {
                            icon.GetComponent<BlockMove>().StartMove(new Vector3(icon.transform.position.x, 4.1f, icon.transform.position.z));
                        }

                        if (changetext == 0)//上矢印非表示
                        {
                            arrowup.SetActive(false);
                        }
                        else
                        {
                            arrowup.SetActive(true);
                        }

                        if (changetext + 6 > StatusFlagManager.StageMaxNum - 2)
                        {
                            arrowdown.SetActive(false);
                        }
                        else
                        {
                            arrowdown.SetActive(true);
                        }
                    }
                    else if (changemove == 0 && !icon.GetComponent<BlockMove>().moveflag)//アイコン移動
                    {
                        nowselect = nextselect;
                    }
                    else if (nextselect == -1 && !select.GetComponent<BlockMove>().moveflag)//タイトル戻る
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            stagetext[i].transform.Find("Text").GetComponent<Text>().text = "Stage " + (i + 1);
                        }
                        changetext = 0;

                        //アイコンイメージ受け取る
                        iconimage = icon.transform.Find("Canvas").Find("Image").gameObject;

                        //ステージセレクト初期位置
                        select.transform.position = new Vector3(0.0f, 10.0f, 0.0f);

                        //ステージセレクト非アクティブ
                        select.SetActive(false);

                        //現在選択してるステージ
                        selectstageid = -1;

                        nowselect = selectstageid;
                        nextselect = 0;

                        //フラグ
                        turnicon = -1.0f;
                        changemove = 0;
                        StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;

                        selectaudio.FadeOutStart();
                        selectaudio = null;
                    }
                }
                else if (selectstageid == -1)//ステージ選択前
                {
                    select.SetActive(true);
                    selectstageid = 0;
                    nextselect = selectstageid;

                    select.GetComponent<BlockMove>().StartMove(new Vector3(0.0f, 0.0f, 0.0f));

                    Vector3 set = icon.transform.position;
                    icon.GetComponent<BlockMove>().StartMove(new Vector3(set.x, 4.1f, set.z));

                    selectaudio = AudioManager.PlayAudio("StageSelect", true, true);

                    if (changetext == 0)//上矢印非表示
                    {
                        arrowup.SetActive(false);
                    }
                    else
                    {
                        arrowup.SetActive(true);
                    }

                    if (changetext + 6 > StatusFlagManager.StageMaxNum - 2)
                    {
                        arrowdown.SetActive(false);
                    }
                    else
                    {
                        arrowdown.SetActive(true);
                    }
                }
                else if (nowselect == -1 && !select.GetComponent<BlockMove>().moveflag && !icon.GetComponent<BlockMove>().moveflag)
                {
                    nowselect = nextselect;
                }
                
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (turnicon != -1.0f)
        {
            iconimage.transform.rotation = Quaternion.RotateTowards(iconimage.transform.rotation, Quaternion.Euler(0.0f, 0.0f, turnicon), 180.0f / 14.0f);
            if (iconimage.transform.eulerAngles.z == 180.0f)
            {
                turnicon = -1.0f;
            }
        }
    }
}
