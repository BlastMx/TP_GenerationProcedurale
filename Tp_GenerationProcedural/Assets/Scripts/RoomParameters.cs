using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomParameters: MonoBehaviour
{
    [Range(0, 6)] public int difficulty;
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
