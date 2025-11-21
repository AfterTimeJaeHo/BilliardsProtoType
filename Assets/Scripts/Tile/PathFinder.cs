using System.Collections.Generic;

namespace Aftertime.MyTinyStreamer.Tile
{
    /// <summary>
    /// Tile 그래프를 이용해 최단 경로를 탐색합니다.
    /// </summary>
    public static class PathFinder
    {
        public static List<Tile> FindPath(Tile startTile, Tile targetTile)
        {
            if (startTile == null || targetTile == null)
            {
                return null;
            }

            Queue<Tile> queue = new Queue<Tile>();
            Dictionary<Tile, Tile> parentLookup = new Dictionary<Tile, Tile>();

            queue.Enqueue(startTile);
            parentLookup[startTile] = null;

            while (queue.Count > 0)
            {
                Tile current = queue.Dequeue();
                if (current == targetTile)
                {
                    return BuildPath(parentLookup, current);
                }

                IReadOnlyList<Tile> neighbors = current.NextTiles;
                if (neighbors == null)
                {
                    continue;
                }

                for (int i = 0; i < neighbors.Count; i++)
                {
                    Tile neighbor = neighbors[i];
                    if (neighbor == null)
                    {
                        continue;
                    }

                    if (neighbor.BlocksMovement)
                    {
                        continue;
                    }

                    if (parentLookup.ContainsKey(neighbor))
                    {
                        continue;
                    }

                    parentLookup[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }

            return null;
        }

        private static List<Tile> BuildPath(Dictionary<Tile, Tile> parentLookup, Tile end)
        {
            List<Tile> path = new List<Tile>();
            Tile node = end;

            while (node != null)
            {
                path.Add(node);
                parentLookup.TryGetValue(node, out node);
            }

            path.Reverse();
            return path;
        }
    }
}
