using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node
{
    public Vector2Int pos;
    public Node parent;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public Node(Vector2Int pos)
    {
        this.pos = pos;
    }
}