using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] BlockMove SelectIcon;//�I���A�C�R��

    //�^�C�g��
    [SerializeField] GameObject title;
    private Vector3[] TitleObj;

    private const int SELECT_RETURN2 = -3;
    private const int SELECT_RETURN = -2;//�X�e�[�W�I������߂��Ă����Ƃ�
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
        //�I�u�W�F�N�g�󂯎��
        TitleObj = new Vector3[3];//3�쐬
        TitleObj[0] = title.transform.Find("NewGame").position;//�j���[�Q�[��
        TitleObj[1] = title.transform.Find("Continue").position;//�R���e�B�j���[
        TitleObj[2] = title.transform.Find("Exit").position;//�o��

        //�Z�[�u�f�[�^������ꍇ�R���e�B�j���[�ɃA�C�R�������킹��
        if (ScoreSave.IsSavaData())
        {
            select = SELECT_CONTINU;
        }

        //�A�C�R���̍��W��ݒ�
        Vector3 icon = SelectIcon.transform.position;
        SelectIcon.transform.position = new Vector3(icon.x, TitleObj[select].y, icon.z);

        //���̑I�������̂ŏ㏑��
        nextselect = select;

        title.SetActive(true);

        //�t���O
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
                if (v > 0.0f || Input.GetKey(KeyCode.W))//��
                {
                    if (select != SELECT_NEWGAME)//��ԏ�ł͂Ȃ��ꍇ
                    {
                        nextselect = select - 1;
                        SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                }
                else if (v < 0.0f || Input.GetKey(KeyCode.S))//��
                {
                    if (select != SELECT_EXIT)//��ԉ��ł͂Ȃ��ꍇ
                    {
                        nextselect = select + 1;
                        SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                }
                else if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Space))//����
                {
                    if (select == SELECT_NEWGAME)//�j���[�Q�[��
                    {
                        select = SELECT_NONE;
                        nextselect = select;

                        BlockMove block = title.GetComponent<BlockMove>();
                        block.StartMove(new Vector3(title.transform.position.x, 10.0f, title.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                        ScoreSave.SavaDataClear();
                        ScoreSave.SavaDataDelete();
                    }
                    else if (select == SELECT_CONTINU && ScoreSave.IsSavaData())//�R���e�B�j���[
                    {
                        select = SELECT_NONE;
                        nextselect = select;
                        BlockMove block = title.GetComponent<BlockMove>();
                        block.StartMove(new Vector3(title.transform.position.x, 10.0f, title.transform.position.z));
                        AudioManager.PlayAudio("IconMove", false, false);
                    }
                    else if (select == SELECT_EXIT)//�I��
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
                else if(select == SELECT_RETURN)//�߂��Ă����Ƃ�
                {
                    title.SetActive(true);

                    //�Z�[�u�f�[�^������ꍇ�R���e�B�j���[�ɃA�C�R�������킹��
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
