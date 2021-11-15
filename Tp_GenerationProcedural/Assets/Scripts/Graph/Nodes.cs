using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour
{
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
