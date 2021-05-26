using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public int nowHeight; //{ set; get; }

    public int nowPiller;//{ get; set; }

    public int nextPiller;// { get; set; }

    public bool FallFlag;//{ get; set; }//�����t���O

    public bool AirFlag;// { get; set; }//�󒆃t���O

    //���}�l�[�W���[
    private PillerManager piller;

    //�t���[���J�E���g
    private float MoveFlame;//�ړ��t���[��
    
    //�ړ��֌W
    public bool YMoveFlag { get; set; }//�ړ��t���O true �ړ����@false �ړ����ĂȂ�
    private float YEndPosi;//�ړ���
    private float YMoveSpeed;//�ړ����x

    public bool XMoveFlag { get; set; }

    //�����Ɉړ�
    public bool MoveCenter;// { get; set; }

    //��]�ړ�
    public bool DefoMoveFlag { get; set; }
    public Quaternion RotaMove { get; set; }
    public float moveaxis { get; set; }

    public float nowmoveaxis;// { get; set; }

    private void Awake()
    {
        DefoMoveFlag = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        YMoveFlag = false;
        YEndPosi = 0.0f;
        YMoveSpeed = 0.0f;

        XMoveFlag = false;
        MoveCenter = false;

        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();
        SetNoMove();

        this.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        this.Fall();
    }

    private void FixedUpdate()
    {
        Move();
    }

    //�����蔻��
    void OnCollisionEnter(Collision collision)
    {
        if (FallFlag)
        {
            //rigitbody�̉e�����󂯂Ȃ�����
            this.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    //��������
    private void Fall()
    {
        if (FallFlag)
        {
            //�O�ȉ��ɂȂ�Ȃ��悤�ɂ���
            if (this.transform.position.y <= nowHeight)
            {
                this.transform.position = new Vector3(this.transform.position.x, nowHeight, this.transform.position.z);
                this.GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                this.GetComponent<Rigidbody>().isKinematic = false;
                AirFlag = true;
            }
        }
    }

    

    private void Move()
    {
        if (YMoveFlag == true)//�ړ�����Ƃ�
        {
            if (YEndPosi <= this.transform.position.y)
            {
                //�t���O
                YMoveFlag = false;
                FallFlag = false;
                AirFlag = false;
                SelectChangeHeight(nowHeight + 1);
            }

            this.transform.position += new Vector3(0.0f, YMoveSpeed, 0.0f);
        }
        else if (XMoveFlag == true)
        {

            if ((nextPiller == nowPiller &&
                ((moveaxis >= 0.0f && !NowWay()) ||//���ړ�
                 (moveaxis <= 0.0f && NowWay()))))//�E�ړ�
            {
                FallFlag = true;
                XMoveFlag = false;
                SetNoMove();
            }
            else
            {
                ProcesMove();
            }

            //���ύX����
            ProcesPiller();
        }
        else if (MoveCenter == true)
        {
            if ((moveaxis >= 0.0f && !NowWay()) ||//���ړ�
                (moveaxis <= 0.0f && NowWay()))//�E�ړ�
            {
                FallFlag = true;
                MoveCenter = false;
                SetNoMove();
            }
            else
            {
                ProcesMove();
            }

            //���ύX����
            ProcesPiller();
        }
        else if(DefoMoveFlag == true)
        {
            //���E�ړ���������
            if (MoveRest())
            {
                SetNoMove();
            }

            //���E�ړ�
            ProcesMove();

            //���ύX����
            ProcesPiller();
        }

        
    }

    //������ύX����
    //rotateheight ��]���@����
    public void ChangeHeight(int rotateheight)
    {
        int range = rotateheight - nowHeight;
        nowHeight = rotateheight + range - 1;
        Mathf.Max(0, nowHeight);
    }

    public void SelectChangeHeight(int height)
    {
        nowHeight = height;
        if (this.tag == "Block")
        {
            this.name = "Block" + nowHeight;//���O����������
        }
    }

    //�ݒ肳��Ă���I�u�W�F�N�g�𒼐��ړ�����
    //�߂�l�@true �ړ����@false �ړ����ĂȂ�
    //endposi �ړ���
    //flame �ړ��t���[��
    public bool SetYMove(float endposiY, int moveflame)
    {
        YMoveFlag = true;
        FallFlag = false;
        AirFlag = true;
        MoveFlame = moveflame;
        YEndPosi = endposiY;
        YMoveSpeed = (YEndPosi - nowHeight) / (float)moveflame;
        this.GetComponent<Rigidbody>().isKinematic = true;
        return YMoveFlag;
    }

    //�������ړ�
    //way true �E�@false ��
    //moveflame �ړ��t���[����
    public bool SetXMove(int MovePiller, int moveflame)
    {
        XMoveFlag = true;
        FallFlag = false;
        nextPiller = MovePiller;

        float XMoveSpeed = CalFlameMove(moveflame);

        int now = (nowPiller + piller.Aroundnum);
        int next = (nextPiller + piller.Aroundnum) - now;
        now = now - now;

        if (next < 0)
        {
            next += piller.Aroundnum;
        }

        if (next <= 11)
        {
            XMoveSpeed *= -1;
        }

        SetMove(XMoveSpeed);//�ړ��p�x�Z�b�g

        return XMoveFlag;
    }

    public bool SetCenterMove(int moveflame)
    {
        MoveCenter = true;
        FallFlag = false;
        nextPiller = nowPiller;

        float XMoveSpeed = CalFlameMove(moveflame);

        if (NowWay() == false)
        {
            XMoveSpeed *= -1;
        }

        SetMove(XMoveSpeed);//�ړ��p�x�Z�b�g
        return MoveCenter;
    }

    private float CalFlameMove(float moveflame)
    {
        float oneangle = 360.0f / (float)piller.Aroundnum;//������̈ړ��p�x
        float XMoveSpeed = oneangle / moveflame;//�ړ��p�x�v�Z
        return XMoveSpeed = Mathf.Abs(XMoveSpeed);
    }

    //�u���b�N���{������͂��̍��W��Ԃ�
    private Vector3 MovePosition()
    {
        return new UnityEngine.Vector3(
            this.transform.localPosition.x, 
            (float)nowHeight - this.transform.parent.localPosition.y, 
            this.transform.localPosition.z);
    }

    //�����̈ړ����v�Z
    private void ProcesPiller()
    {
        //������
        float pillerposi = ProcessPillerPosi();
        if (Mathf.Abs(nowmoveaxis) > pillerposi)//������
        {

            //�ϐ��̒��g�ύX
            if (nowmoveaxis <= 0)//�E�ɂ���
            {
                nowmoveaxis = pillerposi + (nowmoveaxis + pillerposi);//�ړ������Z�b�g
                nowPiller = MovePillerID(true);//���݂̒��ύX
                this.transform.parent = piller.FieldPiller[nowPiller].transform;
            }
            else if (nowmoveaxis > 0)//���ɂ���
            {
                nowmoveaxis = -pillerposi + (nowmoveaxis - pillerposi);//�ړ������Z�b�g
                nowPiller = MovePillerID(false);//���݂̒�
                this.transform.parent = piller.FieldPiller[nowPiller].transform;
            }
        }
    }

    //�ړ���~����
    public bool MoveRest()
    {
        float nextmove = nowmoveaxis + moveaxis;//���̈ړ��p�x���v�Z
        if (Mathf.Abs(nextmove) + 1.0f > ProcessPillerPosi())
        {
            if ((NowWay() && piller.GetPillerBlock(MovePillerID(true), nowHeight)) || //�E�ɂ���
                (!NowWay() && piller.GetPillerBlock(MovePillerID(false), nowHeight)))   //���ɂ���
            {
                return true;
            }
        }

        return false;
    }

    //���ړ�
    private float ProcessPillerPosi()
    {
        return ((360.0f / piller.Aroundnum) / 2.0f);
    }

    //���̈ړ�ID
    //true �E�@false ��
    public int MovePillerID(bool way)
    {
        if (way)//�E
        {
            return (nowPiller + (piller.Aroundnum + 1)) % piller.Aroundnum;
        }

        //��
        return (nowPiller + (piller.Aroundnum - 1)) % piller.Aroundnum;
    }

    //�ړ�����
    public void ProcesMove()
    {
        this.transform.position = RotaMove * this.transform.position;
        nowmoveaxis += moveaxis;
    }
    
    //�ړ��o�^
    //angle �ړ��p�x
    public void SetMove(float angle)
    {
        RotaMove = Quaternion.AngleAxis(angle, Vector3.up);
        moveaxis = angle;
    }

    //�ړ����Ȃ�����
    public void SetNoMove()
    {
        RotaMove = Quaternion.identity;
        moveaxis = 0.0f;
    }

    public void SetNoDown()
    {
        SetNoMove();
        this.GetComponent<Rigidbody>().isKinematic = false;
        FallFlag = false;
    }

    //���݂̒��̈ʒu
    //true ���̒��S����E���ɂ���
    //false ���̒��S���獶���ɂ���
    public bool NowWay()
    {
        if (nowmoveaxis <= 0.0f)
        {
            return true;
        }
        return false;
    }

    public bool StateReverse()
    {
        if (this.transform.parent.name.Contains("Turn"))
        {
            return true;
        }

        return false;
    }
    
}
