using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ステータスやフラグを管理するマネージャー
public class StatusFlagManager : MonoBehaviour
{
    static public readonly int GAME_NONE = 0;
    static public readonly int GAME_START = 1;
    static public readonly int GAME_OVER = 2;
    static public readonly int GAME_CLEAR = 3;
    static public int GameStatusFlag = GAME_NONE;//ゲームの現在の状態

    static public readonly int SCENE_NONE = 0;
    static public readonly int SCENE_TITLE = 1;
    static public readonly int SCENE_STAGESELECT = 2;
    static public readonly int SCENE_PAUSE = 3;
    static public readonly int SCENE_RESULT = 4;
    static public readonly int SCENE_GAME = 5;
    static public int SceneFlag = SCENE_NONE;//シーンの現在の状態

    [SerializeField] StageManager stagecon;//ステージマネージャー
    static public int SelectStageID = 0;//現在選んでるステージID

    private void Awake()
    {
        //一番最初のシーン
        SceneFlag = SCENE_GAME;

        //現在選んでるステージ(デバッグのやつ)
        SelectStageID = stagecon.NowStage;
    }
}
