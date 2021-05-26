using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[CreateAssetMenu()]
public class StageCreator : ScriptableObject
{
    [SerializeField] int ActionCount;
    [SerializeField] int TimeCount;

    [SerializeField] Vector2Int Player;

    [SerializeField] Vector2Int[] Block;

    [SerializeField] Vector3Int[] TurnPiller;

    public void SetPlayer(Create create)
    {
        create.SetPlayer(Player.x, Player.y);
    }

    public void SetBlock(Create create)
    {
        foreach (var block in Block)
        {
            create.CreateBlock(block.x, block.y);
        }
    }

    public void SetTurnPiller(Create create)
    {
        foreach(var piller in TurnPiller)
        {
            create.CreateTurnPiller(piller.x, piller.y, piller.z);
        }
    }
}
