using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TurnPillerManager : MonoBehaviour
{
    public GameObject PillerPrefub;
    public int ReturnFlame;//���̉�]���ԂP�t���[��1/60

    //[HideInInspector] public GameObject[] Piller;//���_���I�u�W�F�N�g
    public List<GameObject> Piller { get; set; }

    private PillerManager FieldPiller;

    private void Awake()
    {
        Piller = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        FieldPiller = this.GetComponent<PillerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //�t���[���Œ�̍X�V
    private void FixedUpdate()
    {

    }

    //��]������z��ɃZ�b�g
    public void SetPiller(GameObject piller, int side, int height)
    {
        for (int i = 0; i < Piller.Count; i++)
        {
            Field field = Piller[i].GetComponent<Field>();
            int inside = field.nowPiller;
            int inheight = field.nowHeight;
            if (inside == side)
            {
                Piller.Insert(i, piller);
                return;
            }
        }

        Piller.Add(piller);
    }

    //��]�ł��������������]�J�n
    public GameObject StartReverse(int pillerid, int height)
    {
        GameObject obj = null;
        bool flag = false;
        for (int i = 0; i < Piller.Count; i++)
        {

            //�f�[�^�󂯎��
            Field field = Piller[i].GetComponent<Field>();
            TurnPiller turnPiller = Piller[i].GetComponent<TurnPiller>();

            //�������Ȃ��ꍇ
            if (field.nowPiller != pillerid)
            {
                if (flag)
                {   
                    break;
                }
                continue;
            }
            else if(!flag)
            {
                flag = true;
            }

            
            //�������łȂ������͈̔͂ɂ��邩
            int ereamax = field.nowHeight + turnPiller.size - 1;
            int ereamin = field.nowHeight - turnPiller.size;
            if (ereamax >= height && ereamin <= height)
            {
                //���łɌ����Ă鍂����菬�����ꍇ�͓���ւ���
                if (obj == null)
                {
                    obj = Piller[i];
                }
                else if (obj.GetComponent<Field>().nowHeight > field.nowHeight)
                {
                    obj = Piller[i];
                }
            }
        }

        if (obj != null)
        {
            obj.GetComponent<TurnPiller>().ReverseStart(true);
        }
        return obj;
    }
}
