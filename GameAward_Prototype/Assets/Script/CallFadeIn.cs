/*------------------------------------------------------------
 
    [CallFadeIn.cs]
    Author : 出合翔太

    遷移先のシーンでFadeInだけをするクラス
    アタッチするオブジェクトはどれでもいい
 
 -------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFadeIn : MonoBehaviour
{

    private void Awake()
    {
        Fade.FadeIn();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
