using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public GameObject FlameObject;//フレームオブジェクト

    public GameObject Floor { get; set; }//現在のフロアオブジェクト
    public int Floornum { get; set; } //フロア数

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //上の階層に進む
    public void UpFloor()
    {
        Floornum++;
    }
}
