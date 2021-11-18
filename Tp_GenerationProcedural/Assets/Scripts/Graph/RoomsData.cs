using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RoomsData", order = 1)]
public class RoomsData : ScriptableObject
{
    public List<RoomParameters> rooms = new List<RoomParameters>();

    public int value;
}
