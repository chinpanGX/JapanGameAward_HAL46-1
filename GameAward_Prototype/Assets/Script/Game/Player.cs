using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] ScoreManager score;

    public float speed;//�ړ����x
    public int jumpflame;
    public PillerManager piller { get; set; }//�����
    private TurnPillerManager turnpillermane;
    public TurnPiller turnpiller { get; set; }

    //�t�B�[���h�擾
    Field field;

    //�A�j���[�V����
    Animator animator;

    //�{�^�����}UI
    public GameObject Abutton;
    public GameObject Bbutton;

    //���Ă�ꏊ
    Vector3 look;//�����Ă�
    int nowlook;//�����Ă����

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    public int BlockUpDownFlag = BLOCK_NONE;//�u���b�N��艺��t���O
    private bool Way;

    private const int MOVE_NONE = 0;
    private const int MOVE_UP = 1;//���
    private const int MOVE_X = 2;//���ړ�
    private const int MOVE_CENTER = 3;//�����ړ�
    private const int MOVE_DOWN = 4;//����
    private const int MOVE_SIDE = 5;//���w��ړ�
    private int MoveFlag = MOVE_NONE;

    private const int INPUT_NONE = 0;//���͂��ĂȂ�
    private const int INPUT_SET = 1;//���͂��Ă���܂��͉������珈����
    private const int INPUT_LEFT = 2;//��
    private const int INPUT_RIGHT = 3;//�E
    private const int INPUT_REVERSE = 4;//��]
    private const int INPUT_JUMP = 5;
    private const int INPUT_STICK = 6;
    private int NowInput = INPUT_NONE;


    private const int CLEAR_NONE = 0;//�܂�
    private const int CLEAR_CENTER = 1;//�����Ɉړ�
    private const int CLEAR_ANIME = 2;//�A�j���[�V����
    private const int CLEAR_ROTA1 = 3;//��]1
    private const int CLEAR_ROTA2 = 4;//��]2
    public int ClearFlag = CLEAR_NONE;
    private Vector3 clearmove;
    
    // Start is called before the first frame update
    void Start()
    {
        //�}�l�[�W���[�֌W
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        turnpillermane = manager.GetComponent<TurnPillerManager>();

        //�A�j���[�^�[
        GameObject playermodel = this.transform.Find("PlayerModel").gameObject;
        animator = playermodel.GetComponent<Animator>();

        //�t�B�[���h
        field = this.GetComponent<Field>();
        field.DefoMoveFlag = true;

        //�ϐ�������
        turnpiller = null;

        //�{�^������UI
        Abutton.SetActive(false);
        Bbutton.SetActive(false);

        nowlook = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (StatusFlagManager.SceneFlag != StatusFlagManager.SCENE_GAME || StatusFlagManager.GameStatusFlag != StatusFlagManager.GAME_PLAY)
        {
            return;
        }

        if (ClearFlag == CLEAR_NONE)
        {
            if (turnpiller == null)
            {
                DownHitProcess();
            }

            //���͏���
            ProcesInput();

            //�v���C���[����
            CalLook();
        }
    }

    private void FixedUpdate()
    {
        if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_GAME)
        {
            if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_PLAY)
            {
                if (ClearFlag == CLEAR_NONE)
                {
                    //��ړ�
                    UpDownMove();
                }

                SetClear();
            }
        }

        if (StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_CLEAR)
        {
            ClearMove();
        }
    }

    //���͏���
    private void ProcesInput()
    {
        //��]���W�����v�{�^�������邩�H
        ButtunUI();


        if (NowInput == INPUT_NONE && turnpiller == null && !field.AirFlag)
        {
            //�ړ����Z�b�g
            field.SetNoMove();
            var h = Input.GetAxis("Horizontal");
            if (h >= 0.9f || h <= -0.9f)//�ړ�
            {
                field.SetMove(-speed * h);
                if (h < 0)
                {
                    nowlook = -1;
                }
                else
                {
                    nowlook = 1;
                }
                animator.SetBool("Move", true);
                NowInput = INPUT_STICK;
            }
            else if (Input.GetButton("Right"))//�E
            {
                SetMove(true);
            }
            else if (Input.GetButton("Left"))//��
            {
                SetMove(false);
            }
            else
            {
                animator.SetBool("Move", false);
            }
            
            if (Input.GetButton("Reverce"))//��]
            {
                if (SetReverse())
                {
                    field.SetNoMove();
                    score.CountAction();
                }
            }
            else if (Input.GetButton("Jump"))//�u���b�N���
            {
                if (SetBlockUp())
                {
                    field.SetNoMove();
                    score.CountAction();
                }
            }
        }
        else
        {
            if (BlockUpDownFlag == BLOCK_NONE)
            {
                if (NowInput == INPUT_STICK)//�X�e�B�b�N����
                {
                    var h = Input.GetAxis("Horizontal");
                    if (h <= 0.9f && h >= -0.9f)
                    {
                        animator.SetBool("Move", false);
                        NowInput = INPUT_NONE;
                    }
                    else
                    {
                        NowInput = INPUT_NONE;
                    }
                }
                else if (NowInput == INPUT_REVERSE && turnpiller == null && !field.AirFlag)//��]����
                {
                    NowInput = INPUT_NONE;
                }
                else if (NowInput == INPUT_LEFT)//�E�ړ�
                {
                    if (Input.GetButton("Left"))
                    {
                        NowInput = INPUT_NONE;
                    }
                    else
                    {
                        animator.SetBool("Move", false);
                        NowInput = INPUT_NONE;
                    }

                }
                else if (NowInput == INPUT_RIGHT)//���{�^��
                {
                    if (Input.GetButton("Right"))
                    {
                        NowInput = INPUT_NONE;
                    }
                    else
                    {
                        animator.SetBool("Move", false);
                        NowInput = INPUT_NONE;
                    }
                }
            }
            
        }
    }

    //��]�����Z�b�g
    private bool SetReverse()
    {
        GameObject obj = turnpillermane.StartReverse(field.nowPiller, field.nowHeight);
        if (obj != null)
        {
            turnpiller = obj.GetComponent<TurnPiller>();
            this.GetComponent<Rigidbody>().isKinematic = true;
            NowInput = INPUT_REVERSE;
            return true;
        }
        return false;
    }

    //�u���b�N����Z�b�g
    private bool SetBlockUp()
    {
        if (field.NowWay())//�E�ɂ���ꍇ
        {
            Way = true;
        }
        else//���ɂ���ꍇ
        {
            Way = false;
        }

        //�u���b�N���w�肵�������ɂ��邩
        if (!piller.GetPillerBlock(field.MovePillerID(Way), field.nowHeight) ||
            piller.GetPillerBlock(field.MovePillerID(Way), field.nowHeight + 1) ||
            piller.GetPillerBlock(field.nowPiller, field.nowHeight + 1))
        {
            return false;
        }


        BlockUpDownFlag = BLOCK_UP;
        NowInput = INPUT_JUMP;

        return true;
    }

    private void UpDownMove()
    {
        //��]���͓o��~�肵�Ȃ�
        if (turnpiller != null)
        {
            return;
        }

        if (BlockUpDownFlag == BLOCK_UP)//�o�鏈��
        {
            BlockUpMove();
        }
        else if (BlockUpDownFlag == BLOCK_DOWN)
        {
            BlockDownMove();
        }
        
    }

    //�u���b�N�o�鏈��
    private void BlockUpMove()
    {
        if (MoveFlag == MOVE_NONE)//�o��J�n
        {
            //field.SetYMove(field.nowHeight + 1, jumpflame);
            //MoveFlag = MOVE_UP;

            if (!field.MoveRest())
            {
                field.SetXMove(field.MovePillerID(Way), jumpflame);//���ړ�
                MoveFlag = MOVE_SIDE;
                animator.SetBool("Move", true);
            }
            else
            {
                field.SetYMove(field.nowHeight + 1, jumpflame);
                MoveFlag = MOVE_UP;
                field.XMoveFlag = false;
                animator.SetBool("Move", false);
            }
            

            if (Way)
            {
                nowlook = 1;
            }
            else
            {
                nowlook = -1;
            }
        }
        else if(MoveFlag == MOVE_SIDE && field.MoveRest())//�ǂɂԂ�������
        {
            field.SetYMove(field.nowHeight + 1, jumpflame);
            MoveFlag = MOVE_UP;
            field.XMoveFlag = false;
            animator.SetBool("Move", false);
        }
        else if (MoveFlag == MOVE_UP && !field.YMoveFlag)//�o�蒆
        {
            field.SetXMove(field.MovePillerID(Way), jumpflame);//�E�ړ��J�n
            MoveFlag = MOVE_X;
        }
        else if (MoveFlag == MOVE_X && !field.XMoveFlag)//�E�ړ���
        {
            //�u���b�N�o��I���
            NowInput = INPUT_NONE;
            MoveFlag = MOVE_NONE;
            BlockUpDownFlag = BLOCK_NONE;
        }
    }


    //��������
    private void BlockDownMove()
    {
        if (MoveFlag == MOVE_NONE)//�����Ɉړ���
        {
            if (field.nowHeight - 1 < 0)//0��菬�����ꍇ
            {
                MoveFlag = MOVE_NONE;
                BlockUpDownFlag = BLOCK_NONE;
                NowInput = INPUT_NONE;
            }
            else
            {
                if (NowInput != INPUT_REVERSE)
                {
                    field.SetCenterMove(jumpflame);
                    MoveFlag = MOVE_CENTER;
                    NowInput = INPUT_SET;
                }
                else
                {
                    NowInput = INPUT_SET;
                    MoveFlag = MOVE_CENTER;
                    field.SelectChangeHeight(field.nowHeight - 1);
                }
                
            }
        }
        else if (MoveFlag == MOVE_CENTER && !field.MoveCenter)
        {
            //���Ƀu���b�N���Ȃ��ꍇ
            if (HitAir())
            {
                field.SelectChangeHeight(field.nowHeight - 1);
            }
            else
            {
                MoveFlag = MOVE_DOWN;
            }
        }
        else if (MoveFlag == MOVE_DOWN && this.GetComponent<Rigidbody>().isKinematic)
        {
            //���n������
            MoveFlag = MOVE_NONE;
            BlockUpDownFlag = BLOCK_NONE;
            NowInput = INPUT_NONE;
            field.AirFlag = false;
        }
    }

    private void DownHitProcess()
    {
        var obj = piller.GetObjectMulti(field.nowPiller, field.nowHeight - 1);

        bool blockflag = false;
        foreach (var item in obj)
        {
            if (item.tag == "Block")
            {
                field.nowHeight = item.GetComponent<Field>().nowHeight + 1;
                blockflag = true;
                field.AirFlag = false;
                break;
            }
        }


        if (obj.Length == 0 || blockflag == false)
        {
            if (field.nowHeight <= 0)
            {
                field.nowHeight = 0;
                field.AirFlag = false;
            }
            else if (BlockUpDownFlag == BLOCK_NONE)
            {
                animator.SetBool("Move", false);
                BlockUpDownFlag = BLOCK_DOWN;
            }
        }
    }

    //���󒆂ɂ��邩���Ȃ���
    //�߂�l�@���ŋ󒆁@���Œn��
    private bool HitAir()
    {
        if (field.nowHeight <= 0)
        {
            return false;
        }

        var obj = piller.GetObjectMulti(field.nowPiller, field.nowHeight - 1);
        foreach (var item in obj)
        {
            if (item.tag == "Block")
            {
                return false;
            }
        }

        return true;
    }

    //�ړ�
    //direction true=�E�@false=��
    void SetMove(bool direction)
    {
        if (direction)//�E
        {
            field.SetMove(-speed);
            nowlook = 1;
            NowInput = INPUT_RIGHT;
        }
        else//��
        {
            field.SetMove(speed);
            nowlook = -1;
            NowInput = INPUT_LEFT;
        }

        animator.SetBool("Move", true);
    }

    //����������v�Z����₟��
    void CalLook()
    {
        Vector3 center = new Vector3(0.0f, this.transform.position.y, 0.0f);
        Vector3 vec = center - this.transform.position;
        look = Vector3.Cross(Vector3.up, vec);
        look = Vector3.Normalize(look);
        this.transform.LookAt(this.transform.position + (nowlook * look));
    }

    void ButtunUI()
    {
        Abutton.SetActive(false);
        Bbutton.SetActive(false);

        if (turnpiller == null && !field.AirFlag && BlockUpDownFlag == BLOCK_NONE)
        {
            if (JudgeReverce())//��]
            {
                Abutton.SetActive(true);
            }

            if (JudgeJump())//�u���b�N���
            {
                Bbutton.SetActive(true);
            }
        }
    }

    //�����]
    bool JudgeReverce()
    {
        return turnpillermane.JudgePiller(field.nowPiller, field.nowHeight);
    }

    //����W�����v
    bool JudgeJump()
    {
        if (field.NowWay())//�E�ɂ���ꍇ
        {
            Way = true;
        }
        else//���ɂ���ꍇ
        {
            Way = false;
        }

        //�u���b�N���w�肵�������ɂ��邩
        if (!piller.GetPillerBlock(field.MovePillerID(Way), field.nowHeight) ||
            piller.GetPillerBlock(field.MovePillerID(Way), field.nowHeight + 1) ||
            piller.GetPillerBlock(field.nowPiller, field.nowHeight + 1))
        {
            return false;
        }

        return true;
    }

    private void SetClear()
    {
        //�ŏ�K�@��]���ĂȂ��@�󒆂ɂ��Ȃ��@�ړ��t���O�������ĂȂ�
        if (field.nowHeight == 20 && turnpiller == null && !field.AirFlag && BlockUpDownFlag == BLOCK_NONE)
        {
            if (ClearFlag == CLEAR_NONE)//�N���A�����J�n
            {
                //���W�␳
                this.transform.position = new Vector3(this.transform.position.x, 20.0f, this.transform.position.z);

                //�d�͂̉e�����󂯂Ȃ�����
                this.GetComponent<Rigidbody>().isKinematic = true;

                //�������������Ȃ�����
                field.FallFlag = false;

                //�ړ��������v�Z����
                clearmove = new Vector3(0.0f, this.transform.position.y, 0.0f) - this.transform.position;
                clearmove = Vector3.Normalize(clearmove);

                //�J�������v���C���[�̎q�ɓ����
                Camera.main.transform.parent = this.transform;

                //�t���O�ύX
                ClearFlag = CLEAR_ROTA1;
                animator.SetBool("Move", true);
                //field.SetNoMove();
                StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_CLEAR;
            }
        }
    }

    private void ClearMove()
    {
        if (ClearFlag == CLEAR_ROTA1)//�ŏ��̉�]
        {
            //�����Ɍ����悤�ɉ�]
            Quaternion target = Quaternion.LookRotation(clearmove);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target, 2.0f);
            if (target == this.transform.rotation)//�����Ɍ�������
            {
                ClearFlag = CLEAR_CENTER;
            }
        }
        else if (ClearFlag == CLEAR_CENTER)//�����ړ�
        {
            //�ړ�
            this.transform.position += clearmove * 0.1f;

            //�����Ɉړ�������
            if (this.transform.position.x <= 0.1f && this.transform.position.x >= -0.1f)
            {
                if (this.transform.position.z <= 0.1f && this.transform.position.z >= -0.1f)
                {
                    this.transform.position = new Vector3(0.0f, 20.0f, 0.0f);
                    Camera.main.transform.parent = null;
                    ClearFlag = CLEAR_ROTA2;
                }
            }
        }
        else if (ClearFlag == CLEAR_ROTA2)
        {
            Quaternion target = Quaternion.LookRotation(new Vector3(Camera.main.transform.position.x, 20.0f, Camera.main.transform.position.z) - this.transform.position);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target, 2.0f);
            StatusFlagManager.SceneFlag = StatusFlagManager.SCENE_RESULT;
            if (target.y == this.transform.rotation.y)
            {
                ClearFlag = CLEAR_ANIME;
                animator.SetBool("Move", false);
            }
        }
        else if (ClearFlag == CLEAR_ANIME)
        {
            StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_RESULT;
        }
    }
}
