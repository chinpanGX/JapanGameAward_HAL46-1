using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPiller : MonoBehaviour
{
    //��]���x
    public int ReturnFlame;

    public int size { get; set; }

    private Field field;//�t�B�[���h

    //���}�l�[�W���[
    private PillerManager piller;

    //�v���C���[
    GameObject player;

    //��]
    public bool ReturnFlag { get; set; }
    private Quaternion Move;
    private Vector3 Axis;

    private int flamecount;


    // Start is called before the first frame update
    void Start()
    {
        ReturnFlag = false;
        Move = Quaternion.identity;
        Axis = Vector3.zero;
        flamecount = 0;

        field = this.GetComponent<Field>();

        GameObject manager = GameObject.Find("Manager");
        piller = manager.GetComponent<PillerManager>();

        GameObject child = this.transform.Find("Cube").gameObject;
        child.transform.localScale = new Vector3(1.0f, size, 1.0f);

        player = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Reverse();
    }

    //�\�����]
    //Pillerid ��ID
    //rotedirection�@��]�����@true ���@false ��O
    public void ReverseStart(bool rotedirection)
    {
        //��]��
        //���S(�����ƍ�������)�܂ł̕����x�N�g��
        Axis = new Vector3(0.0f, this.transform.position.y, 0.0f) - this.transform.position;

        //��]�������߂�
        Axis = Vector3.Cross(Axis, Vector3.up);
        Axis.Normalize();//�Ȃ�ƂȂ����K��

        //�u���b�N��񓙂��󂯎��
        Transform[] set = new Transform[size * 2];
        for (int i = 0; i < set.Length; i++)
        {
            set[i] = null;
        }

        int childid = 0;
        foreach(Transform child in piller.FieldPiller[field.nowPiller].transform)
        {
            int maxheight = field.nowHeight + size - 1;
            int minheight = field.nowHeight - size;
            Field cfield = child.GetComponent<Field>();
            if (child.tag != "TurnPiller" && (maxheight >= cfield.nowHeight && minheight <= cfield.nowHeight))
            {
                set[childid] = child;

                if (child.tag == "Player")
                {
                    player = child.gameObject;
                }

                childid++;
            }
        }

        foreach (var child in set)
        {
            if (child == null)
            {
                continue;
            }
            child.transform.parent = this.transform;
        }



        //�t���O�֌W
        foreach (Transform child in this.transform)
        {
            if (child.name == "Cube")
            {
                continue;
            }
            Field field = child.GetComponent<Field>();
            field.FallFlag = false;
            field.AirFlag = true;
        }

        //��]����
        float angle = 180.0f / (float)ReturnFlame;
        if (rotedirection == false)
        {
            angle *= -1;
        }
        Move = Quaternion.AngleAxis(angle, Axis);//���̉�]��

        ReturnFlag = true;
    }

    //��]����
    private void Reverse()
    {
        if (ReturnFlag == true)//��]���Ă���ꍇ
        {

            //��]
            this.transform.rotation *= Move;

            //�t���[���J�E���g
            flamecount++;

            if (flamecount >= ReturnFlame)//�t���[���J�E���g���w��̐��l�𒴂�����
            {

                //�����t���܂ɂȂ����̂���]�O�ɖ߂�
                this.transform.Rotate(Axis, 180.0f);

                //�u���b�N�t���O�֌W
                Transform[] obj = new Transform[size * 2];
                int id = 0;
                foreach (Transform child in this.transform)
                {
                    if (child.name == "Cube")
                    {
                        continue;
                    }
                    Field field = child.GetComponent<Field>();
                    field.FallFlag = true;
                    field.ChangeHeight(this.field.nowHeight);
                    child.transform.position = new Vector3(child.transform.position.x, field.nowHeight, child.transform.position.z);
                    obj[id] = child;
                    id++;
                }

                foreach(var child in obj)
                {
                    if (child == null)
                    {
                        continue;
                    }
                    child.parent = this.transform.parent;
                }

                flamecount = 0;//�t���[���J�E���g���Z�b�g
                ReturnFlag = false;//��]���钌�����Z�b�g
            }
        }
    }
}
