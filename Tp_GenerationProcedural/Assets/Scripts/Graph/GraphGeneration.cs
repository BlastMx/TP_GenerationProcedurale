using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGeneration : MonoBehaviour
{
    List<Nodes> nodes = new List<Nodes>();
    List<Connections> connections = new List<Connections>();

    // Start is called before the first frame update
    void Start()
    {
        CreateStartNode();
        CreateDungeon(10);
        FinishDungeon();
        SecondaryPath(5);

        foreach (var node in nodes)
            Debug.LogError($"Node pos : {node.pos}");
    }

    #region Abstract graph

    void CreateStartNode()
    {
        Nodes startNode = new Nodes();
        startNode.pos = Vector2.zero;
        startNode._type = Nodes.type.start;
        startNode.difficulty = 0;

        nodes.Add(startNode);

        Connections startConnections = new Connections();
        startConnections.hasLocked = false;

        connections.Add(startConnections);
    }

    void CreateDungeon(int nodesnumber)
    {
        for(int i = 0; i < nodesnumber; i++)
        {
            bool canContinue = true;
            Vector2 previousNodePos;

            do
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

            } while (canContinue == false);

            Nodes node = new Nodes();

            switch (GetOrientation())
            {
                case Utils.ORIENTATION.NORTH:
                    node._orientation = Nodes.orientation.NORTH;
                    break;
                case Utils.ORIENTATION.EAST:
                    node._orientation = Nodes.orientation.EAST;
                    break;
                case Utils.ORIENTATION.SOUTH:
                    node._orientation = Nodes.orientation.SOUTH;
                    break;
                case Utils.ORIENTATION.WEST:
                    node._orientation = Nodes.orientation.WEST;
                    break;

                default:
                    break;
            }

            node.pos = previousNodePos;
            node._type = Nodes.type.normal;
            node.difficulty = Random.Range(0, 4);

            nodes.Add(node);

            Connections connection = new Connections();
            connection.hasLocked = Random.Range(0, 2) == 0 ? false : true;

            connections.Add(connection);
        }
    }

    void FinishDungeon()
    {
        Nodes endNode = new Nodes();
        Vector2 previousNodePos = nodes[nodes.Count - 1].pos;

        previousNodePos += Utils.OrientationToDir(Utils.ORIENTATION.NORTH);

        endNode.pos = previousNodePos;
        endNode._type = Nodes.type.end;
        endNode.difficulty = 0;

        nodes.Add(endNode);
    }

    void SecondaryPath(int nodesnumber)
    {
        Nodes startingNode;
        Nodes checkedNode;

        bool north = false;
        bool south = false;
        bool east = false;
        bool west = false;

        do{

            north = false;
            south = false;
            east = false;
            west = false;

            checkedNode = nodes[Random.Range(0, nodes.Count - 1)];

            foreach (var _node in nodes)
            {

                if (_node.pos == checkedNode.pos + Utils.OrientationToDir(Utils.ORIENTATION.NORTH))
                {
                    north = true;
                }
                if (_node.pos == checkedNode.pos + Utils.OrientationToDir(Utils.ORIENTATION.SOUTH))
                {
                    south = true;
                }
                if (_node.pos == checkedNode.pos + Utils.OrientationToDir(Utils.ORIENTATION.EAST))
                {
                    east = true;
                }
                if (_node.pos == checkedNode.pos + Utils.OrientationToDir(Utils.ORIENTATION.WEST))
                {
                    west = true;
                }
            }
        }
        while (north && south && east && west);

        startingNode = checkedNode;

        for (int i = 0; i < nodesnumber; i++)
        {
            bool canContinue = true;
            Vector2 previousNodePos;

            do
            {
                if(i == 0) previousNodePos = startingNode.pos;
                else previousNodePos = nodes[nodes.Count - 1].pos;
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

            } while (canContinue == false);

            Nodes node = new Nodes();

            switch (GetOrientation())
            {
                case Utils.ORIENTATION.NORTH:
                    node._orientation = Nodes.orientation.NORTH;
                    break;
                case Utils.ORIENTATION.EAST:
                    node._orientation = Nodes.orientation.EAST;
                    break;
                case Utils.ORIENTATION.SOUTH:
                    node._orientation = Nodes.orientation.SOUTH;
                    break;
                case Utils.ORIENTATION.WEST:
                    node._orientation = Nodes.orientation.WEST;
                    break;

                default:
                    break;
            }

            node.pos = previousNodePos;
            node._type = Nodes.type.normal;
            node.difficulty = Random.Range(0, 4);

            nodes.Add(node);

            Connections connection = new Connections();
            connection.hasLocked = Random.Range(0, 2) == 0 ? false : true;

            connections.Add(connection);
        }
    }

    Utils.ORIENTATION GetOrientation()
    {
        switch (Random.Range(0,4))
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



    #endregion Instance Prefab
}
