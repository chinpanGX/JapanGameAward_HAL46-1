using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int NowStage;
    public StageCreator[] stage;

    public StageCreator GetStage()
    {
        return stage[NowStage];
    }

    public StageCreator GetStage(int stageid)
    {
        return stage[stageid];
    }
}
