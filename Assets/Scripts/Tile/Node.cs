using UnityEngine;

namespace Waving.Tile
{
    public class Node
    {
        public Vector3Int cell;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(Vector3Int cell)
        {
            this.cell = cell;
        }
    }
}