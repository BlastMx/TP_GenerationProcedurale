using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomParameters
{
    [Range(0, 3)] public int difficulty;
    public bool hasKey;

    public enum State {
        START = 0,
        END = 1,
        NORMAL = 2,
        INVERTED = 3,
        HIDDEN = 4
    };

    public State myState;
}
