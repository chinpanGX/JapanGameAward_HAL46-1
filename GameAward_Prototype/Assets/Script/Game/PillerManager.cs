using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerManager : MonoBehaviour
{

    public int Aroundnum;//����̒��̐�
    [HideInInspector] public GameObject[] FieldPiller;//���Ǘ�


    //���̔z�����������
    public void PreFieldPiller()
    {
        if (FieldPiller.Length <= 0)//�z�񂪐ݒ肳��Ă��Ȃ��ꍇ
        {
            FieldPiller = new GameObject[Aroundnum];
        }
    }



    public bool GetPillerBlock(int piller, int height)
    {
        GameObject obj = GetObject(piller, height, "Block");
        
        if (obj == null || obj.tag != "Block")
        {
            return false;
        }

        return true;
    }

    /*
    piller = ��
    height = ����
    tag = �^�O NONE�ŃI�u�W�F�N�g�̗L�����m�F
    */
    public GameObject GetObject(int piller, int height, string tag)
    {
        foreach (Transform child in FieldPiller[piller].transform)
        {
            Field field = child.GetComponent<Field>();
            
            if ((child.tag == tag || tag == "NONE") && height == field.nowHeight)
            {
                return child.gameObject;
            }
        }
        GameObject obj = null;
        return obj;
    }

    public GameObject[] GetObjectMulti(int piller, int height)
    {
        var list = new List<GameObject>();
        foreach (Transform child in FieldPiller[piller].transform)
        {
            Field field = child.GetComponent<Field>();

            if (height == field.nowHeight)
            {
                list.Add(child.gameObject);
            }
        }

        GameObject[] obj = new GameObject[list.Count];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = list[i];
        }

        return obj;
    }
}
