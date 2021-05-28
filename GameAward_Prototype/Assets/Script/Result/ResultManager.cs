using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    //�ŏ��̈ړ��n
    [SerializeField] GameObject player;
    private GameObject Came;

    //UI�ݒ�
    [SerializeField] ScoreManager score;
    [SerializeField] GameObject canvas;
    private Animator canvasanime;

    //������
    [SerializeField] StageManager stage;


    private const int RESULT_NONE = 0;
    private const int RESULT_START = 1;
    private const int RESULT_STAR = 3;
    private const int RESULT_KEY = 4;
    private int result;

    private AudioController audio;

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
            if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Z))//���̃X�e�[�W�ɐi��
            {
                if (StatusFlagManager.SelectStageID >= StatusFlagManager.StageMaxNum - 1)//���݂̃X�e�[�W���ő�̏ꍇ
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;
                    Fade.FadeOut("Title");
                    audio.FadeOutStart(20);
                }
                else
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_GAME;
                    StatusFlagManager.SelectStageID++;
                    StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_START;
                    Fade.FadeOut("SampleScene");
                    audio.FadeOutStart(20);
                }
            }
            else if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.X))//�X�e�[�W�I���ɐi��
            {
                StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_STAGESELECT;
                Fade.FadeOut("Title");
                audio.FadeOutStart(20);
            }
            else if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.B))//�^�C�g���ɐi��
            {
                if (StatusFlagManager.SelectStageID < StatusFlagManager.StageMaxNum - 1)
                {
                    StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_TITLE;
                    Fade.FadeOut("Title");
                    audio.FadeOutStart(20);
                }
            }
        }
        else if (result == RESULT_STAR)
        {
            canvas.transform.Find("Star01").GetComponent<Animator>().SetBool("Move", false);
            canvas.transform.Find("Star02").GetComponent<Animator>().SetBool("Move", false);
            canvas.transform.Find("Star03").GetComponent<Animator>().SetBool("Move", false);

            result = RESULT_KEY;
        }
        else if (result == RESULT_START && canvasanime.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if (stage.stage[StatusFlagManager.SelectStageID].GetTimeCount() >= score.time)//�^�C��
            {
                canvas.transform.Find("Star01").GetComponent<Animator>().SetBool("Move", true);
            }

            if (stage.stage[StatusFlagManager.SelectStageID].GetActionCount() >= score.action)//�A�N�V����
            {
                canvas.transform.Find("Star02").GetComponent<Animator>().SetBool("Move", true);
            }

            if (0 >= score.miss)//�~�X
            {
                canvas.transform.Find("Star03").GetComponent<Animator>().SetBool("Move", true);
            }
            
            

            canvasanime.SetBool("Move", false);
            result = RESULT_STAR;
        }
        else if (result == RESULT_NONE)
        {
            //�J��������(����)
            Vector3 front = Came.transform.position - player.transform.position;
            front = Vector3.Normalize(front);
            front *= 1.0f;

            //��
            Vector3 side = Vector3.Cross(front, Vector3.up);
            side = Vector3.Normalize(side);
            side *= 0.4f;

            //��
            Vector3 up = Vector3.up * 0.4f;

            //�w��̈ʒu�Ɉړ�
            Came.GetComponent<BlockMove>().StartMove(player.transform.position + front + side + up);
            result = RESULT_START;

            //�e�L�X�g�ݒ�
            canvas.transform.Find("Time").Find("Timetext").GetComponent<Text>().text = score.time.ToString();
            canvas.transform.Find("Action").Find("Actiontext").GetComponent<Text>().text = score.action.ToString();
            canvas.transform.Find("Miss").Find("Misstext").GetComponent<Text>().text = score.miss.ToString();
            canvas.transform.Find("Stage").GetComponent<Text>().text = "Stage " + (StatusFlagManager.SelectStageID + 1);

            if (StatusFlagManager.SelectStageID >= StatusFlagManager.StageMaxNum - 1)//���݂̃X�e�[�W���ő�̏ꍇ
            {
                canvas.transform.Find("A").Find("Atext").GetComponent<Text>().text = "Title";
                canvas.transform.Find("B").gameObject.SetActive(false);
            }


            audio = AudioManager.PlayAudio("Result", true, true);

            //�t���O�ݒ�
            canvas.SetActive(true);
            canvasanime.SetBool("Move", true);
        }
    }
}
