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
    // Start is called before the first frame update
    void Start()
    {
        Fade.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
