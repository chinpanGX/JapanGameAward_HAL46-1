using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerManager : MonoBehaviour
{

    public int Aroundnum;//一周の柱の数
    [HideInInspector] public GameObject[] FieldPiller;//柱管理

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


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
        
        foreach (Transform child in FieldPiller[piller].transform)//柱に置いてあるブロックを見る
        {
            Field field = child.GetComponent<Field>();
            if (height == field.nowHeight)//同じブロックが存在した場合
            {
                return true;
            }
        }
        return false;
    }

    

}
