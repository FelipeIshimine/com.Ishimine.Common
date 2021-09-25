using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class UnityTilemapExtensions
{
    public static void Replace(this Tilemap tilemap, List<(TileBase from, TileBase to)> replacementPairs)
    {
        Dictionary<TileBase, TileBase> replacementTargets = new Dictionary<TileBase, TileBase>();
        foreach (var item in replacementPairs)
            replacementTargets.Add(item.from, item.to);
        Replace(tilemap, replacementTargets);
    }
    public static void Replace(this Tilemap tilemap, Dictionary<TileBase, TileBase> replacementPairs)
    {
        Debug.Log("Replace");
        tilemap.CompressBounds();

        foreach (Vector3Int coordinate in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase targetTile = tilemap.GetTile(coordinate);
            if (targetTile == null || !replacementPairs.ContainsKey(targetTile)) 
                continue;

            var matrix = tilemap.GetTransformMatrix(coordinate);

            tilemap.SetTile(coordinate, replacementPairs[targetTile]);
            tilemap.SetTransformMatrix(coordinate, matrix);
        }
    }
}