using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ステータスやフラグを管理するマネージャー
public class StatusFlagManager : MonoBehaviour
{
    static readonly int GAME_NONE = 0;
    static readonly int GAME_START = 1;
    static readonly int GAME_OVER = 2;
    static readonly int GAME_CLEAR = 3;
    static int GameStatusFlag = GAME_NONE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
