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

        foreach (var node in nodes)
            Debug.LogError($"Node pos : {node.pos}");
    }

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
                        canContinue = false;
                }

            } while (canContinue == false);

            Nodes node = new Nodes();

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
}
