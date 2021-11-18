using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes
{
    public void InitNode(Nodes nodes)
    {
        pos = nodes.pos;
        _type = nodes._type;
        difficulty = nodes.difficulty;
    }

    public Vector2Int pos;

    public enum type
    {
        start,
        normal,
        inverted,
        key,
        end
    }

    public type _type;

    public int difficulty = 0;

    public Room room;
}
