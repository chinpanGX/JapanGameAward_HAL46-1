using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;//�ړ����x
    public int jumpflame;
    private PillerManager piller;//�����
    private TurnPillerManager turnpillermane;
    public TurnPiller turnpiller { get; set; }

    //�t�B�[���h�擾
    Field field;

    //���Ă�ꏊ
    Vector3 look;

    private const int BLOCK_NONE = 0;
    private const int BLOCK_UP = 1;
    private const int BLOCK_DOWN = 2;
    public int BlockUpDownFlag = BLOCK_NONE;//�u���b�N��艺��t���O
    private bool Way;

    private const int MOVE_NONE = 0;
    private const int MOVE_UP = 1;
    private const int MOVE_X = 2;
    private const int MOVE_CENTER = 3;
    private const int MOVE_DOWN = 4;
    public int MoveFlag = MOVE_NONE;

    private const int INPUT_NONE = 0;//���͂��ĂȂ�
    private const int INPUT_SET = 1;//���͂��Ă���܂��͉������珈����
    private const int INPUT_LEFT = 2;//��
    private const int INPUT_RIGHT = 3;//�E
    private const int INPUT_REVERSE = 4;//��]
    private const int INPUT_JUMP = 5;
    public int NowInput = INPUT_NONE;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        turnpillermane = manager.GetComponent<TurnPillerManager>();


        field = this.GetComponent<Field>();
        field.DefoMoveFlag = true;

        turnpiller = null;
    }

    // Update is called once per frame
    void Update()
    {
        //���͏���
        ProcesInput();

        if (turnpiller == null)
        {
            //this.transform.LookAt(look);
        }
        
    }

    private void FixedUpdate()
    {
        if (turnpiller == null)
        {
            HitProcess();
        }
        
        //��ړ�
        UpDownMove();

    }

    //���͏���
    private void ProcesInput()
    {
        if (NowInput == INPUT_NONE && turnpiller == null && !field.AirFlag)
        {
            field.SetNoMove();

            var h = Input.GetAxis("Horizontal");

            if (h != 0)//�ړ�
            {
                field.SetMove(-speed * h);
                NowInput = INPUT_RIGHT;
            }
            else if (Input.GetButton("Right"))//�E
            {
                field.SetMove(-speed);
                NowInput = INPUT_RIGHT;
            }
            else if (Input.GetButton("Left"))//��
            {
                field.SetMove(speed);
                NowInput = INPUT_LEFT;
            }
            else if (Input.GetButtonDown("Reverce"))//��]
            {
                field.SetNoMove();
                SetReverse();
            }
            else if (Input.GetButtonDown("Jump"))//�u���b�N���
            {
                field.SetNoMove();
                SetBlockUp();
            }
        }
        else
        {
            if (BlockUpDownFlag == BLOCK_NONE)
            {
                if ((NowInput == INPUT_LEFT && !Input.GetButton("Left")) ||
                (NowInput == INPUT_RIGHT && !Input.GetButton("Right"))
                )
                {
                    NowInput = INPUT_NONE;
                }
                else if (NowInput == INPUT_REVERSE && turnpiller == null && !field.AirFlag)
                {
                    NowInput = INPUT_NONE;
                }
            }
            
        }
    }

    //��]�����Z�b�g
    private void SetReverse()
    {
        GameObject obj = turnpillermane.StartReverse(field.nowPiller, field.nowHeight);
        if (obj != null)
        {
            turnpiller = obj.GetComponent<TurnPiller>();
            this.GetComponent<Rigidbody>().isKinematic = true;
            NowInput = INPUT_REVERSE;
        }
    }

    //�u���b�N����Z�b�g
    private void SetBlockUp()
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
            return;
        }


        BlockUpDownFlag = BLOCK_UP;
        NowInput = INPUT_JUMP;
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
            field.SetYMove(field.nowHeight + 1, jumpflame);
            MoveFlag = MOVE_UP;
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

    private void HitProcess()
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
}
