using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStageSelectManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //if (StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_GAME && StatusFlagManager.GameStatusFlag == StatusFlagManager.GAME_START)
        //{
        //    if (!Fade.m_isFadeIn)//フェードが終わったら
        //    {
        //        StatusFlagManager.GameStatusFlag = StatusFlagManager.GAME_PLAY;
        //    }
        //}

        if ((StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_TITLE || StatusFlagManager.SceneFlag == StatusFlagManager.SCENE_STAGESELECT) && StatusFlagManager.TitleSelectFlag == StatusFlagManager.TS_START)
        {
            if (!Fade.m_isFadeIn)
            {
                StatusFlagManager.TitleSelectFlag = StatusFlagManager.TS_PLAY;
            }
        }
        
    }
}
