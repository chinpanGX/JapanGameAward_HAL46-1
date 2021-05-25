using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] GameObject icon;
    private GameObject iconimage;
    private float turnicon;

    [SerializeField] GameObject select;//�X�e�[�W�Z���N�g
    private GameObject[] stageobj;
    private GameObject[] stagetext;

    public int selectstageid;//���ݑI�����Ă�X�e�[�WID

    private int nowselect;
    private int nextselect;

    // Start is called before the first frame update
    void Start()
    {
        //�X�e�[�W�u���b�N�󂯎��
        stageobj = new GameObject[6];
        stagetext = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            stageobj[i] = select.transform.Find("Stage0" + (i + 1)).gameObject;
            stagetext[i] = stageobj[i].transform.Find("Canvas").gameObject;
            stagetext[i].transform.Find("Text").GetComponent<Text>().text = "Stage " + (i + 1);
        }

        //�A�C�R���C���[�W�󂯎��
        iconimage = icon.transform.Find("Canvas").Find("Image").gameObject;

        //�X�e�[�W�Z���N�g�����ʒu
        select.transform.position = new Vector3(0.0f, 10.0f, 0.0f);

        //�X�e�[�W�Z���N�g��A�N�e�B�u
        select.SetActive(false);

        //���ݑI�����Ă�X�e�[�W
        selectstageid = -1;

        nowselect = selectstageid;
        nextselect = 0;

        //�t���O
        turnicon = -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_STAGESELECT)
        {
            if (nowselect == nextselect)
            {
                if (Input.GetKey(KeyCode.W))//��
                {
                    if (nowselect > 1)//��ԏ�̎�
                    {
                        selectstageid -= 2;

                        nextselect = nowselect - 2;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(icon.transform.position.x, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                    }
                }
                else if (Input.GetKey(KeyCode.S))//��
                {
                    if (nowselect < 4)//��ԏ�̎�
                    {
                        selectstageid += 2;

                        nextselect = nowselect + 2;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(icon.transform.position.x, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                    }
                }
                else if (Input.GetKey(KeyCode.A))//��
                {
                    if (nowselect % 2 == 1)
                    {
                        selectstageid -= 1;

                        turnicon = 0.0f;
                        nextselect = nowselect - 1;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x + 2, icon.transform.position.y, icon.transform.position.z));
                    }
                    else if(nowselect != 0)
                    {
                        selectstageid -= 1;

                        turnicon = 180.0f;
                        nextselect = nowselect - 1;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x - 2, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                    }
                }
                else if (Input.GetKey(KeyCode.D))//�E
                {
                    if (nowselect % 2 != 1)
                    {
                        selectstageid += 1;

                        turnicon = 180.0f;
                        nextselect = nowselect + 1;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x - 2, icon.transform.position.y, icon.transform.position.z));
                    }
                    else if (nowselect != 5)
                    {
                        selectstageid += 1;

                        turnicon = 0.0f;
                        nextselect = nowselect + 1;
                        icon.GetComponent<BlockMove>().StartMove(new Vector3(stageobj[nextselect].transform.position.x + 2, stageobj[nextselect].transform.position.y, icon.transform.position.z));
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    StatusFlagManager.SelectStageID = selectstageid;
                    Scene.ChangeScene("SampleScene");
                }
            }
            else if (nowselect != nextselect)
            {
                if (selectstageid != -1)
                {
                    if (!icon.GetComponent<BlockMove>().moveflag)
                    {
                        nowselect = nextselect;
                    }
                }
                else if (selectstageid == -1)
                {
                    select.SetActive(true);
                    selectstageid = 0;
                    nextselect = selectstageid;

                    select.GetComponent<BlockMove>().StartMove(new Vector3(0.0f, 0.0f, 0.0f));

                    Vector3 set = icon.transform.position;
                    icon.GetComponent<BlockMove>().StartMove(new Vector3(set.x, 4.1f, set.z));
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