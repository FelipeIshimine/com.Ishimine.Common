using UnityEngine;

public static class GridExtensions
{
    public static Vector3 CellToWorld(this Grid grid, Vector2Int coordinate) => grid.CellToWorld(new Vector3Int(coordinate.x, coordinate.y));
}