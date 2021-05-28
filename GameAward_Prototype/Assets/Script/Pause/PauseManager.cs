using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] StageManager stage;

    //�X�R�A�擾
    [SerializeField] ScoreManager score;

    //�e�L�X�g�֌W
    [SerializeField] GameObject PauseObj;
    [SerializeField] Text Time;
    [SerializeField] Text Action;
    [SerializeField] Text Miss;

    //�I���̂��
    [SerializeField] GameObject Icon;
    [SerializeField] Transform[] SelectUI;

    private const int SELECT_NONE = -1;
    private const int SELECT_CLOSE = 0;
    private const int SELECT_RETRY = 1;
    private const int SELECT_STAGESELECT = 2;
    private const int SELECT_TITLE = 3;
    private int select = SELECT_CLOSE;

    bool inputflag;

    

    // Start is called before the first frame update
    void Start()
    {
        inputflag = false;
        PauseObj.SetActive(false);
        PauseObj.transform.Find("StageText").GetComponent<Text>().text = "Stage " + (StatusFlagManager.SelectStageID + 1);
        PauseObj.transform.Find("Time").Find("TimeTextMax").GetComponent<Text>().text = "/ " + stage.stage[StatusFlagManager.SelectStageID].GetTimeCount().ToString();
        PauseObj.transform.Find("Action").Find("ActionTextMax").GetComponent<Text>().text = "/ " + stage.stage[StatusFlagManager.SelectStageID].GetActionCount().ToString();
        PauseObj.transform.Find("Miss").Find("MissTextMax").GetComponent<Text>().text = "/ 0";
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_PAUSE)
        {
            PauseObj.SetActive(false);
            return;
        }

        PauseObj.SetActive(true);
        Time.text = score.time.ToString();
        Action.text = score.action.ToString();
        Miss.text = StatusFlagManager.MissCount.ToString();

        var v = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Reverce"))
        {
            if (select == SELECT_CLOSE)
            {
                StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
                select = SELECT_CLOSE;
                inputflag = false;
            }
            else if (select == SELECT_RETRY)
            {
                StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
                StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_START;
                Fade.FadeOut("SampleScene");
                AudioManager.AllFadeOutAudio();
                StatusFlagManager.MissCount++;
            }
            else if (select == SELECT_STAGESELECT)
            {
                StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_STAGESELECT;
                Fade.FadeOut("Title");
                AudioManager.AllFadeOutAudio();
                StatusFlagManager.MissCount = 0;
            }
            else if (select == SELECT_TITLE)
            {
                StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;
                Fade.FadeOut("Title");
                AudioManager.AllFadeOutAudio();
                StatusFlagManager.MissCount = 0;
            }
        }
        else if (inputflag && (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 7") || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.X)))//�|�[�Y���
        {
            StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
            select = SELECT_CLOSE;
            inputflag = false;
        }
        else if (inputflag && (v > 0.0f || Input.GetKeyDown(KeyCode.W)))//��
        {
            if (select > SELECT_CLOSE)
            {
                select--;//�t���O�ύX
                Icon.transform.position = new Vector3(Icon.transform.position.x, SelectUI[select].position.y, Icon.transform.position.z);//�ʒu�ύX
                inputflag = false;
            }
        }
        else if (inputflag && (v < 0.0f || Input.GetKeyDown(KeyCode.S)))//��
        {
            if (select < SELECT_TITLE)
            {
                select++;//�t���O�ύX
                Icon.transform.position = new Vector3(Icon.transform.position.x, SelectUI[select].position.y, Icon.transform.position.z);//�ʒu�ύX
                inputflag = false;
            }
        }
        else if(v == 0.0f)
        {
            inputflag = true;
        }
        
        
    }
}
