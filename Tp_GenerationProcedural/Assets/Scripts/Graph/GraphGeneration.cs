using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGeneration : MonoBehaviour
{
    List<Nodes> nodes = new List<Nodes>();
    List<GameObject> principalRooms = new List<GameObject>();
    List<Nodes> globalNodes = new List<Nodes>();
    List<Connections> connections = new List<Connections>();
    //List<Connections> secondarysConnections = new List<Connections>();

    [SerializeField]
    private GameObject prefabRoom;

    // Start is called before the first frame update
    void Start()
    {
        CreateStartNode();

        InstanceRoom(CreateDungeon(10, connections, nodes, true), connections, false);

        InstanceSecondaryPath(5);
        InstanceSecondaryPath(2);
    }

    #region Abstract graph

    void InstanceSecondaryPath(int nodesNumber)
    {
        var value = UnityEngine.Random.Range(0, nodes.Count - 1);
        var secondaryPath = SecondaryPath(nodesNumber, nodes[value]);

        InstanceRoom(secondaryPath.Item1, secondaryPath.Item2, true, value);
    }

    void CreateStartNode()
    {
        Nodes startNode = new Nodes();
        startNode.pos = Vector2.zero;
        startNode._type = Nodes.type.start;
        startNode.difficulty = 0;

        nodes.Add(startNode);
        globalNodes.Add(startNode);

        Connections startConnections = new Connections();
        startConnections.hasLocked = false;

        startConnections.previousNode = startNode;

        connections.Add(startConnections);
    }

    List<Nodes> CreateDungeon(int nodesnumber, List<Connections> connections, List<Nodes> nodes, bool canEnd)
    {
        for(int i = 0; i < nodesnumber; i++)
        {
            bool canContinue = true;
            Vector2 previousNodePos;

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

            if (i == nodesnumber - 1)
            {
                if(canEnd)
                    node._type = Nodes.type.end;

                node.difficulty = 0;
            }
            else
            {
                node._type = Nodes.type.normal;
                node.difficulty = UnityEngine.Random.Range(0, 4);

                Connections connection = new Connections();
                connection.hasLocked = UnityEngine.Random.Range(0, 2) == 0 ? false : true;

                connection.previousNode = node;

                connections.Add(connection);
            }

        }

        return nodes;
    }

    public Tuple<List<Nodes>, List<Connections>> SecondaryPath(int nodesNumber, Nodes startNodes)
    {
        List<Nodes> secondaryNodes = new List<Nodes>();
        List<Connections> connectionsSecondary = new List<Connections>();

        Connections startSecondaryConnection = new Connections();
        startSecondaryConnection.hasLocked = false;

        startSecondaryConnection.previousNode = startNodes;

        connectionsSecondary.Add(startSecondaryConnection);

        CreateDungeon(nodesNumber, connectionsSecondary, secondaryNodes, false);

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
        List<GameObject> rooms = new List<GameObject>();

        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject room = Instantiate(prefabRoom, new Vector3(nodes[i].pos.x * 11, nodes[i].pos.y * 9), Quaternion.identity, transform);
            room.GetComponent<Room>().nodeRoom = nodes[i];
            if (secondaryPath)
                rooms.Add(room);
            else
                principalRooms.Add(room);
        }

        foreach (var room in secondaryPath ? rooms : principalRooms)
        {
            foreach (var door in room.GetComponent<Room>().doors)
                door.SetState(Door.STATE.WALL);
        }

        InstanceDoor(secondaryPath ? rooms : principalRooms, connections, secondaryPath, value);
    }

    void InstanceDoor(List<GameObject> rooms, List<Connections> connections, bool secondaryPath, int value)
    {
        //Debug.LogError($"rooms : {rooms.Count}, connections : {connections.Count}");

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

            if (connections[i].previousNode.pos.y == connections[i].nextNode.pos.y - 1.0f)
            {
                if(connections[i].previousNode != null)
                    connections[i].previousNode.room.upDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);

                if (connections[i].nextNode != null)
                    connections[i].nextNode.room.downDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
            else if (connections[i].previousNode.pos.y == connections[i].nextNode.pos.y + 1.0f)
            {
                if (connections[i].previousNode != null)
                    connections[i].previousNode.room.downDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);

                if (connections[i].nextNode != null)
                    connections[i].nextNode.room.upDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
            else if (connections[i].previousNode.pos.x == connections[i].nextNode.pos.x + 1.0f)
            {
                if (connections[i].previousNode != null)
                    connections[i].previousNode.room.leftDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);

                if (connections[i].nextNode != null)
                    connections[i].nextNode.room.rightDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
            else if (connections[i].previousNode.pos.x == connections[i].nextNode.pos.x - 1.0f)
            {
                if (connections[i].previousNode != null)
                    connections[i].previousNode.room.rightDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);

                if (connections[i].nextNode != null)
                    connections[i].nextNode.room.leftDoor.SetState(connections[i].hasLocked ? Door.STATE.CLOSED : Door.STATE.OPEN);
            }
        }
    }

    #endregion Instance Prefab
}
