using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] BlockMove SelectIcon;//�I���A�C�R��

    //�^�C�g��
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
        //�I�u�W�F�N�g�󂯎��
        TitleObj = new Vector3[3];//3�쐬
        TitleObj[0] = title.transform.Find("NewGame").position;//�j���[�Q�[��
        TitleObj[1] = title.transform.Find("Continue").position;//�R���e�B�j���[
        TitleObj[2] = title.transform.Find("Exit").position;//�o��

        //�A�C�R���̍��W��ݒ�
        Vector3 icon = SelectIcon.transform.position;
        SelectIcon.transform.position = new Vector3(icon.x, TitleObj[select].y, icon.z);

        //���̑I�������̂ŏ㏑��
        nextselect = select;

        //�^�C�g�����A�N�e�B�u��true�ɂ���
        title.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (select == nextselect && StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_TITLE && select != SELECT_NONE)
        {
            var h = Input.GetAxis("Vertical");
            if (h > 0.0f || Input.GetKey(KeyCode.W))//��
            {
                if (select != SELECT_NEWGAME)//��ԏ�ł͂Ȃ��ꍇ
                {
                    nextselect = select - 1;
                    SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                }
            }
            else if (h < 0.0f ||Input.GetKey(KeyCode.S))//��
            {
                if (select != SELECT_EXIT)//��ԉ��ł͂Ȃ��ꍇ
                {
                    nextselect = select + 1;
                    SelectIcon.StartMove(new Vector3(SelectIcon.transform.position.x, TitleObj[nextselect].y, SelectIcon.transform.position.z));
                }
            }
            else if (Input.GetButtonDown("Reverce") || Input.GetKeyDown(KeyCode.Space))//����
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
