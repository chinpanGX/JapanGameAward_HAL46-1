using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    //最初の移動系
    [SerializeField] GameObject player;
    private GameObject Came;

    //UI設定
    [SerializeField] ScoreManager score;
    [SerializeField] GameObject canvas;
    private Animator canvasanime;

    //星処理
    [SerializeField] StageManager stage;


    private const int RESULT_NONE = 0;
    private const int RESULT_START = 1;
    private const int RESULT_STAR = 3;
    private const int RESULT_KEY = 4;
    private int result;

    private AudioController audioA;

    // Start is called before the first frame update
    void Start()
    {
        Came = Camera.main.gameObject;

        canvas.SetActive(false);
        canvas.transform.localScale -= new Vector3(canvas.transform.localScale.x, 0.0f, 0.0f);

        canvas.transform.Find("Star01").localScale = new Vector3(0.0f, 0.0f, 0.0f);
        canvas.transform.Find("Star02").localScale = new Vector3(0.0f, 0.0f, 0.0f);
        canvas.transform.Find("Star03").localScale = new Vector3(0.0f, 0.0f, 0.0f);

        canvasanime = canvas.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_RESULT)
        {
            return;
        }

        if (result == RESULT_KEY)
        {
            if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Z))//次のステージに進む
            {
                if (StatusFlagManager.SelectStageID >= StatusFlagManager.StageMaxNum - 1)//現在のステージが最大の場合
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;
                    Fade.FadeOut("Title");
                    audioA.FadeOutStart(20);
                    StatusFlagManager.MissCount = 0;
                }
                else
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
                    StatusFlagManager.SelectStageID++;
                    StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_START;
                    Fade.FadeOut("SampleScene");
                    audioA.FadeOutStart(20);
                    StatusFlagManager.MissCount = 0;
                }
            }
            else if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.X))//ステージ選択に進む
            {
                StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_STAGESELECT;
                Fade.FadeOut("Title");
                audioA.FadeOutStart(20);
                StatusFlagManager.MissCount = 0;
            }
            else if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.B))//タイトルに進む
            {
                if (StatusFlagManager.SelectStageID >= StatusFlagManager.StageMaxNum - 1)//現在のステージが最大の場合
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;
                    Fade.FadeOut("Title");
                    audioA.FadeOutStart(20);
                    StatusFlagManager.MissCount = 0;
                }
                else
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
                    //StatusFlagManager.SelectStageID++;
                    StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_START;
                    Fade.FadeOut("SampleScene");
                    audioA.FadeOutStart(20);
                    StatusFlagManager.MissCount = 0;
                }
            }
        }
        else if (result == RESULT_STAR)
        {
            canvas.transform.Find("Star01").GetComponent<Animator>().SetBool("Move", false);
            canvas.transform.Find("Star02").GetComponent<Animator>().SetBool("Move", false);
            canvas.transform.Find("Star03").GetComponent<Animator>().SetBool("Move", false);

            Animator anime = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerModel").GetComponent<Animator>();
            if (anime.GetCurrentAnimatorStateInfo(0).IsName("WaveHands"))
            {
                AudioManager.PlayAudio("ResultSE", false, false);
                result = RESULT_KEY;
            }
            else if(anime.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                result = RESULT_KEY;
            }
        }
        else if (result == RESULT_START && canvasanime.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            int starcount = 0;
            if (stage.stage[StatusFlagManager.SelectStageID].GetTimeCount() >= score.time)//タイム
            {
                canvas.transform.Find("Star01").GetComponent<Animator>().SetBool("Move", true);
                starcount++;
            }

            if (stage.stage[StatusFlagManager.SelectStageID].GetActionCount() >= score.action)//アクション
            {
                canvas.transform.Find("Star02").GetComponent<Animator>().SetBool("Move", true);
                starcount++;
            }

            if (0 >= StatusFlagManager.MissCount)//ミス
            {
                canvas.transform.Find("Star03").GetComponent<Animator>().SetBool("Move", true);
                starcount++;
            }

            AudioManager.PlayAudio("Star", false, false);

            if (starcount == 3)
            {
                GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerModel").GetComponent<Animator>().SetBool("WavaHands", true);
            }
            
            canvasanime.SetBool("Move", false);
            result = RESULT_STAR;
        }
        else if (result == RESULT_NONE)
        {
            //カメラ方向(正面)
            Vector3 front = Came.transform.position - player.transform.position;
            front = Vector3.Normalize(front);
            front *= 1.0f;

            //横
            Vector3 side = Vector3.Cross(front, Vector3.up);
            side = Vector3.Normalize(side);
            side *= 0.4f;

            //上
            Vector3 up = Vector3.up * 0.4f;

            //指定の位置に移動
            Came.GetComponent<BlockMove>().StartMove(player.transform.position + front + side + up);
            result = RESULT_START;

            //タイムテキスト
            GameObject Time = canvas.transform.Find("Time").gameObject;
            Time.transform.Find("Timetext").GetComponent<Text>().text = score.time.ToString();
            Time.transform.Find("TimetextMax").GetComponent<Text>().text = "/ " + stage.stage[StatusFlagManager.SelectStageID].GetTimeCount().ToString();

            //アクションテキスト
            GameObject Action = canvas.transform.Find("Action").gameObject;
            Action.transform.Find("Actiontext").GetComponent<Text>().text = score.action.ToString();
            Action.transform.Find("ActiontextMax").GetComponent<Text>().text = "/ " + stage.stage[StatusFlagManager.SelectStageID].GetActionCount().ToString();

            //ミステキスト
            GameObject Miss = canvas.transform.Find("Miss").gameObject;
            Miss.transform.Find("Misstext").GetComponent<Text>().text = StatusFlagManager.MissCount.ToString();
            Miss.transform.Find("MisstextMax").GetComponent<Text>().text = "/ 0";

            //ステージテキスト
            canvas.transform.Find("Stage").GetComponent<Text>().text = "Stage " + (StatusFlagManager.SelectStageID + 1);

            if (StatusFlagManager.SelectStageID >= StatusFlagManager.StageMaxNum - 1)//現在のステージが最大の場合
            {
                canvas.transform.Find("A").Find("Atext").GetComponent<Text>().text = "Retry";
                canvas.transform.Find("B").gameObject.SetActive(false);
            }


            audioA = AudioManager.PlayAudio("Result", true, true);

            //フラグ設定
            canvas.SetActive(true);
            canvasanime.SetBool("Move", true);
        }
    }
}
