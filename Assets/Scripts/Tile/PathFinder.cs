using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Waving.Tile
{
    public class PathFinder : MonoBehaviour
    {
        // 4방향 이동 (상하좌우). isometric에서도 “셀 기준”이므로 이렇게 씀
        private static readonly Vector3Int[] NeighborDirs =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        public static List<Vector3Int> FindPath(
            GridMap map,
            Vector3Int startCell,
            Vector3Int targetCell)
        {
            var openSet = new List<Node>();
            var allNodes = new Dictionary<Vector3Int, Node>();
            var closedSet = new HashSet<Vector3Int>();

            Node startNode = GetOrCreateNode(startCell, allNodes);
            Node targetNode = GetOrCreateNode(targetCell, allNodes);

            startNode.gCost = 0;
            startNode.hCost = Heuristic(startCell, targetCell);
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // fCost가 가장 낮은 노드 선택
                Node current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < current.fCost ||
                        (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                    {
                        current = openSet[i];
                    }
                }

                if (current.cell == targetCell)
                {
                    // 경로 복원
                    return RetracePath(startNode, current);
                }

                openSet.Remove(current);
                closedSet.Add(current.cell);

                foreach (var dir in NeighborDirs)
                {
                    Vector3Int neighborCell = current.cell + dir;

                    if (!map.IsWalkable(neighborCell))
                        continue;

                    if (closedSet.Contains(neighborCell))
                        continue;

                    Node neighbor = GetOrCreateNode(neighborCell, allNodes);

                    int tentativeG = current.gCost + 10; // 상하좌우 이동 비용 고정 10

                    if (!openSet.Contains(neighbor) || tentativeG < neighbor.gCost)
                    {
                        neighbor.gCost = tentativeG;
                        neighbor.hCost = Heuristic(neighborCell, targetCell);
                        neighbor.parent = current;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            // 경로 없음
            return null;
        }

        private static Node GetOrCreateNode(Vector3Int cell, Dictionary<Vector3Int, Node> dict)
        {
            if (!dict.TryGetValue(cell, out Node node))
            {
                node = new Node(cell);
                dict[cell] = node;
            }

            return node;
        }

        // 맨해튼 거리 * 10
        private static int Heuristic(Vector3Int a, Vector3Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return (dx + dy) * 10;
        }

        private static List<Vector3Int> RetracePath(Node startNode, Node endNode)
        {
            List<Vector3Int> path = new List<Vector3Int>();
            Node current = endNode;

            while (current != startNode)
            {
                path.Add(current.cell);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }
    }
}