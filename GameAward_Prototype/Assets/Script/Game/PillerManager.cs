using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerManager : MonoBehaviour
{

    public int Aroundnum;//一周の柱の数
    [HideInInspector] public GameObject[] FieldPiller;//柱管理


    //柱の配列を準備する
    public void PreFieldPiller()
    {
        if (FieldPiller.Length <= 0)//配列が設定されていない場合
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
    piller = 柱
    height = 高さ
    tag = タグ NONEでオブジェクトの有無を確認
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
