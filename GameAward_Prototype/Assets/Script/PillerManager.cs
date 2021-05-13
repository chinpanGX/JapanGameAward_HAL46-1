using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillerManager : MonoBehaviour
{

    public int Aroundnum;//ˆêü‚Ì’Œ‚Ì”
    [HideInInspector] public GameObject[] FieldPiller;//’ŒŠÇ—

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


    //’Œ‚Ì”z—ñ‚ğ€”õ‚·‚é
    public void PreFieldPiller()
    {
        if (FieldPiller.Length <= 0)//”z—ñ‚ªİ’è‚³‚ê‚Ä‚¢‚È‚¢ê‡
        {
            FieldPiller = new GameObject[Aroundnum];
        }
    }



    public bool GetPillerBlock(int piller, int height)
    {
        GameObject obj = GetObject(piller, height);
        
        if (obj == null || obj.tag != "Block")
        {
            return false;
        }

        return true;
    }

    public GameObject GetObject(int piller, int height)
    {
        foreach (Transform child in FieldPiller[piller].transform)
        {
            Field field = child.GetComponent<Field>();
            
            if (height == field.nowHeight)
            {
                return child.gameObject;
            }
        }
        GameObject obj = null;
        return obj;
    }

}
