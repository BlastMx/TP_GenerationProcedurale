using CreativeSpore.SuperTilemapEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraphGeneration : MonoBehaviour
{
    List<Nodes> nodes = new List<Nodes>();
    List<GameObject> principalRooms = new List<GameObject>();
    List<Nodes> globalNodes = new List<Nodes>();
    List<Connections> connections = new List<Connections>();
    //List<Connections> secondarysConnections = new List<Connections>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject prefabRoom;
    [SerializeField]
    private GameObject triggerChangeEnemy;
    [SerializeField]
    private GameObject triggerChangeEnemyShoot;

    [Header("Colors")]
    [SerializeField]
    private Color firstLevelPartColor;
    [SerializeField]
    private Color SecondLevelPartColor;
    [SerializeField]
    private Color SecondaryWayColor;

    [Header("Rooms data")]
    [SerializeField]
    private RoomsData data;

    [Header("Increase difficulty")]
    [SerializeField]
    private int stepIncreaseDifficulty = 0;

    [Header("Dungeon")]
    [SerializeField]
    private int numberRooms = 0;

    // Start is called before the first frame update
    void Start()
    {
        CreateStartNode();

        InstanceRoom(CreateDungeon(numberRooms, connections, nodes, true, true), connections, false);

        //CreateHidden();

        InstanceSecondaryPath(5);
        InstanceSecondaryPath(2);
    }

    #region Abstract graph

    void InstanceSecondaryPath(int nodesNumber)
    {
        var value = UnityEngine.Random.Range(1, nodes.Count - 1);

        var secondaryPath = SecondaryPath(nodesNumber, nodes[value]);

        connections[value].hasLocked = true;
        
        if (connections[value].previousNode.pos.y == (connections[value].nextNode.pos.y - 1.0f))
        {
            connections[value].previousNode.room.upDoor.SetState(Door.STATE.CLOSED);
            connections[value].nextNode.room.downDoor.SetState(Door.STATE.CLOSED);
        }
        else if (connections[value].previousNode.pos.y == (connections[value].nextNode.pos.y + 1.0f))
        {
            connections[value].previousNode.room.downDoor.SetState(Door.STATE.CLOSED);
            connections[value].nextNode.room.upDoor.SetState(Door.STATE.CLOSED);
        }
        else if (connections[value].previousNode.pos.x == (connections[value].nextNode.pos.x + 1.0f))
        {
            connections[value].previousNode.room.leftDoor.SetState(Door.STATE.CLOSED);
            connections[value].nextNode.room.rightDoor.SetState(Door.STATE.CLOSED);
        }
        else if (connections[value].previousNode.pos.x == (connections[value].nextNode.pos.x - 1.0f))
        {
            connections[value].previousNode.room.rightDoor.SetState(Door.STATE.CLOSED);
            connections[value].nextNode.room.leftDoor.SetState(Door.STATE.CLOSED);
        }

        InstanceRoom(secondaryPath.Item1, secondaryPath.Item2, true, value);
    }

    void CreateStartNode()
    {
        Nodes startNode = new Nodes();
        startNode.pos = Vector2Int.zero;
        startNode._type = Nodes.type.start;
        startNode.difficulty = 0;

        nodes.Add(startNode);
        globalNodes.Add(startNode);

        Connections startConnections = new Connections();
        startConnections.hasLocked = false;

        startConnections.previousNode = startNode;

        connections.Add(startConnections);
        
    }

    void SetNodesConnections(List<Connections> connections, List<Nodes> nodes, int nextDifficultyStep)
    {
        int value = 0;

        for(int i = 0; i < nodes.Count - 1; i++)
        {
            connections[i].previousNode = nodes[i];
            connections[i].nextNode = nodes[i + 1];

            nodes[i].difficulty = value;

            if (IsMultipleOf(i, nextDifficultyStep))
                value++;
        }

        nodes[nodes.Count - 1].difficulty = 0;
    }

    bool IsMultipleOf(int value, int multiplacator)
    {
        while (value > 0)
            value -= multiplacator;

        if (value == 0)
            return true;

        return false;
    }

    List<Nodes> CreateDungeon(int nodesnumber, List<Connections> connections, List<Nodes> nodes, bool canEnd, bool asymetric)
    {
        for(int i = 0; i < nodesnumber/(asymetric ? 2 : 1); i++)
        {
            bool canContinue = true;
            Vector2Int previousNodePos;

            do
            {
                if(nodes.Count > 0)
                    previousNodePos = nodes[nodes.Count - 1].pos;
                else
                    previousNodePos = connections[0].previousNode.pos;

                previousNodePos += Utils.OrientationToDir(GetOrientation());

                foreach (var _node in globalNodes)
                {
                    if (_node.pos == previousNodePos)
                    {
                        canContinue = false;
                        break;
                    }
                    else
                        canContinue = true;
                }

            } while (canContinue == false);

            Nodes node = new Nodes();

            node.pos = previousNodePos;

            connections[connections.Count - 1].nextNode = node;

            nodes.Add(node);
            globalNodes.Add(node);

            node._type = Nodes.type.normal;
            node.difficulty = 1;

            if (i == nodesnumber - 1)
            {
                if (canEnd)
                    node._type = Nodes.type.end;
                else
                    node._type = Nodes.type.key;

                node.difficulty = 0;
            }
            else
            {
                Connections connection = new Connections();
                connection.hasLocked = false;

                connection.previousNode = node;

                connections.Add(connection);
            }
        }

        if (asymetric)
        {
            List<Connections> rConnections = connections;
            rConnections.Reverse();

            for (int j = nodesnumber / 2; j < nodesnumber; j++)
            {
                bool canContinue = true;
                Vector2Int previousNodePos;

                do
                {
                    if (j == nodesnumber / 2)
                    {
                        previousNodePos = nodes[nodes.Count - 1].pos;
                        previousNodePos += Utils.OrientationToDir(GetOrientation());

                        foreach (var _node in nodes)
                        {
                            if (_node.pos == previousNodePos)
                            {
                                canContinue = false;
                                break;
                            }
                            else
                                canContinue = true;
                        }
                    }
                    else
                    {
                        Vector2Int symPos = Vector2Int.zero;
                        symPos.x = rConnections[j - (nodesnumber / 2)].nextNode.pos.x - rConnections[j - (nodesnumber / 2)].previousNode.pos.x;
                        symPos.y = rConnections[j - (nodesnumber / 2)].nextNode.pos.y - rConnections[j - (nodesnumber / 2)].previousNode.pos.y;

                        previousNodePos = nodes[nodes.Count - 1].pos + symPos;

                        canContinue = true;
                    }

                } while (canContinue == false);

                Nodes node = new Nodes();

                node.pos = previousNodePos;

                connections[connections.Count - 1].nextNode = node;

                nodes.Add(node);
                globalNodes.Add(node);

                node._type = Nodes.type.normal;

                Connections connection = new Connections();
                connection.hasLocked = false;
                
                connection.previousNode = node;

                connections.Add(connection);
                
            }
            
            Vector2Int previousNodePosEnd = Vector2Int.zero;
            previousNodePosEnd.x = nodes[1].pos.x;
            previousNodePosEnd.y = nodes[1].pos.y;

            Nodes nodeEnd = new Nodes();

            nodeEnd.pos = nodes[nodes.Count - 1].pos + previousNodePosEnd;

            connections[connections.Count - 1].nextNode = nodeEnd;

            nodes.Add(nodeEnd);
            globalNodes.Add(nodeEnd);

            if (canEnd)
            nodeEnd._type = Nodes.type.end;

            nodeEnd.difficulty = 0;

            SetNodesConnections(connections, nodes, stepIncreaseDifficulty);
        }
        
        return nodes;
    }

    void CreateHidden()
    {
        int value = UnityEngine.Random.Range(1, nodes.Count / 2);
        var roomShown = SecondaryPath(1, nodes[value]);

        if (connections[value].previousNode.pos.y == (connections[value].nextNode.pos.y - 1.0f))
        {
            connections[value].previousNode.room.upDoor.SetState(Door.STATE.OPEN);
            connections[value].nextNode.room.downDoor.SetState(Door.STATE.OPEN);
        }
        else if (connections[value].previousNode.pos.y == (connections[value].nextNode.pos.y + 1.0f))
        {
            connections[value].previousNode.room.downDoor.SetState(Door.STATE.OPEN);
            connections[value].nextNode.room.upDoor.SetState(Door.STATE.OPEN);
        }
        else if (connections[value].previousNode.pos.x == (connections[value].nextNode.pos.x + 1.0f))
        {
            connections[value].previousNode.room.leftDoor.SetState(Door.STATE.OPEN);
            connections[value].nextNode.room.rightDoor.SetState(Door.STATE.OPEN);
        }
        else if (connections[value].previousNode.pos.x == (connections[value].nextNode.pos.x - 1.0f))
        {
            connections[value].previousNode.room.rightDoor.SetState(Door.STATE.OPEN);
            connections[value].nextNode.room.leftDoor.SetState(Door.STATE.OPEN);
        }

        int hiddenValue = nodes.Count-value-1;
        InstanceRoom(roomShown.Item1, roomShown.Item2, true, value);

        var roomHidden = SecondaryPath(1, nodes[nodes.Count - hiddenValue]);

        if (connections[hiddenValue].previousNode.pos.y == (connections[hiddenValue].nextNode.pos.y - 1.0f))
        {
            connections[hiddenValue].previousNode.room.upDoor.SetState(Door.STATE.SECRET);
            connections[hiddenValue].nextNode.room.downDoor.SetState(Door.STATE.SECRET);
        }
        else if (connections[hiddenValue].previousNode.pos.y == (connections[hiddenValue].nextNode.pos.y + 1.0f))
        {
            connections[hiddenValue].previousNode.room.downDoor.SetState(Door.STATE.SECRET);
            connections[hiddenValue].nextNode.room.upDoor.SetState(Door.STATE.SECRET);
        }
        else if (connections[hiddenValue].previousNode.pos.x == (connections[hiddenValue].nextNode.pos.x + 1.0f))
        {
            connections[hiddenValue].previousNode.room.leftDoor.SetState(Door.STATE.SECRET);
            connections[hiddenValue].nextNode.room.rightDoor.SetState(Door.STATE.SECRET);
        }
        else if (connections[hiddenValue].previousNode.pos.x == (connections[hiddenValue].nextNode.pos.x - 1.0f))
        {
            connections[hiddenValue].previousNode.room.rightDoor.SetState(Door.STATE.SECRET);
            connections[hiddenValue].nextNode.room.leftDoor.SetState(Door.STATE.SECRET);
        }

        InstanceRoom(roomHidden.Item1, roomHidden.Item2, true, hiddenValue);

    }

    public Tuple<List<Nodes>, List<Connections>> SecondaryPath(int nodesNumber, Nodes startNodes)
    {
        List<Nodes> secondaryNodes = new List<Nodes>();
        List<Connections> connectionsSecondary = new List<Connections>();

        Connections startSecondaryConnection = new Connections();
        startSecondaryConnection.hasLocked = false;

        startSecondaryConnection.previousNode = startNodes;

        connectionsSecondary.Add(startSecondaryConnection);

        CreateDungeon(nodesNumber, connectionsSecondary, secondaryNodes, false, false);

        return Tuple.Create(secondaryNodes, connectionsSecondary);
    }

    Utils.ORIENTATION GetOrientation()
    {
        switch (UnityEngine.Random.Range(0,4))
        {
            case 0:
                return Utils.ORIENTATION.NORTH;
            case 1:
                return Utils.ORIENTATION.SOUTH;
            case 2:
                return Utils.ORIENTATION.EAST;
            case 3:
                return Utils.ORIENTATION.WEST;

            default:
                return Utils.ORIENTATION.NORTH;

        }

    }

    #endregion Abstract Graph

    #region Instance Prefab
    void InstanceRoom(List<Nodes> nodes, List<Connections> connections, bool secondaryPath, int value = 0)
    {
        List<GameObject> rooms = secondaryPath ? new List<GameObject>() : principalRooms;

        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject prefabSelected = prefabRoom;

            if (!secondaryPath)
            {
                if (i == 0) prefabSelected = SelectRoom(nodes[i].difficulty, false, RoomParameters.State.START);
                else if (i == nodes.Count - 1) prefabSelected = SelectRoom(nodes[i].difficulty, false, RoomParameters.State.END);
                else prefabSelected = SelectRoom(nodes[i].difficulty, false, RoomParameters.State.NORMAL);
            }
            else
            {
                if(i == nodes.Count- 1) prefabSelected = SelectRoom(nodes[i].difficulty, true, RoomParameters.State.NORMAL);
                else prefabSelected = SelectRoom(nodes[i].difficulty, false, RoomParameters.State.NORMAL);
            }

            GameObject room = Instantiate(prefabSelected, new Vector3(nodes[i].pos.x * 11, nodes[i].pos.y * 9), Quaternion.identity, transform);

            room.GetComponent<Room>().nodeRoom = nodes[i];
            room.GetComponent<Room>().position = nodes[i].pos;

            rooms.Add(room);
        }

        foreach (var room in rooms)
        {
            foreach (var door in room.GetComponent<Room>().doors)
                door.SetState(Door.STATE.WALL);

            if (secondaryPath)
                room.transform.GetChild(0).GetComponent<STETilemap>().TintColor = SecondaryWayColor;
            else
            {
                room.transform.GetChild(0).GetComponent<STETilemap>().TintColor = SecondLevelPartColor;
                for(int i = 0; i < nodes.Count/2; i++)
                    rooms[i].transform.GetChild(0).GetComponent<STETilemap>().TintColor = firstLevelPartColor;
            }
        }

        if (!secondaryPath)
        {
            Instantiate(triggerChangeEnemyShoot, rooms[nodes.Count / 2].transform);
            Instantiate(triggerChangeEnemy, rooms[(nodes.Count / 2) - 1].transform);
        }

        InstanceDoor(rooms, connections, secondaryPath, value);
    }

    void InstanceDoor(List<GameObject> rooms, List<Connections> connections, bool secondaryPath, int value)
    {
        for(int i = 0; i < connections.Count; i++)
        {
            if(secondaryPath)
            {
                if(i == 0)
                {
                    connections[i].previousNode.room = principalRooms[value].GetComponent<Room>();
                    connections[i].nextNode.room = rooms[i].GetComponent<Room>();
                }
                else
                {
                    connections[i].previousNode.room = rooms[i - 1].GetComponent<Room>();
                    connections[i].nextNode.room = rooms[i].GetComponent<Room>();
                }
            }
            else
            {
                connections[i].previousNode.room = rooms[i].GetComponent<Room>();
                connections[i].nextNode.room = rooms[i + 1].GetComponent<Room>();
            }

            if (connections[i].previousNode.pos.y == (connections[i].nextNode.pos.y - 1.0f))
            {
                connections[i].previousNode.room.upDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
                connections[i].nextNode.room.downDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
            else if (connections[i].previousNode.pos.y == (connections[i].nextNode.pos.y + 1.0f))
            {
                connections[i].previousNode.room.downDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
                connections[i].nextNode.room.upDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
            else if (connections[i].previousNode.pos.x == (connections[i].nextNode.pos.x + 1.0f))
            {
                connections[i].previousNode.room.leftDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
                connections[i].nextNode.room.rightDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
            else if (connections[i].previousNode.pos.x == (connections[i].nextNode.pos.x - 1.0f))
            {
                connections[i].previousNode.room.rightDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
                connections[i].nextNode.room.leftDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
        }
    }

    GameObject SelectRoom(int difficulty = 0, bool hasKey = false, RoomParameters.State state = RoomParameters.State.NORMAL)
    {
        GameObject selected;
        List<GameObject> Selection = new List<GameObject>();

        foreach (RoomParameters param in data.rooms)
        {
            if (param.myState == state 
                && param.difficulty == difficulty
                    && param.hasKey == hasKey) 
                Selection.Add(param.GetComponent<Room>().gameObject);
        }

        selected = Selection[UnityEngine.Random.Range(0, Selection.Count)];

        Debug.Log(selected);
        

        return selected;
    }

    #endregion Instance Prefab
}

 
