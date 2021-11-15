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
        CreateDungeon();
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

    void CreateDungeon()
    {
        for(int i = 0; i < 10; i++)
        {
            Nodes node = new Nodes();
            Vector2 previousNodePos = nodes[nodes.Count - 1].pos;

            Utils.OrientationToDir(GetOrientation());
            previousNodePos += Utils.OrientationToDir(GetOrientation());

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
}
