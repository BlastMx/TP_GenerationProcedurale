using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour
{
    public void InitNode(Nodes nodes)
    {
        pos = nodes.pos;
        _type = nodes._type;
        difficulty = nodes.difficulty;
    }

    public Vector2 pos;

    public enum type
    {
        start,
        normal,
        end
    }

    public type _type;

    public int difficulty = 0;
}
