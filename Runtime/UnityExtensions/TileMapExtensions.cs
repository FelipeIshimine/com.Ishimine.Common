using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileMapExtensions
{
    public static Vector3Int[] FindAllCoordinates(this Tilemap tilemap, TileBase baseTile)
    {
        BoundsInt boundsInt = tilemap.cellBounds;
        List<Vector3Int> coordinates = new List<Vector3Int>();

        for (int z = boundsInt.zMin; z < boundsInt.zMax; z++)
        for (int y = boundsInt.yMin; y < boundsInt.yMax; y++)
        for (int x = boundsInt.xMin; x < boundsInt.xMax; x++)
        {
            var coordinate = new Vector3Int(x, y, z);
            if(baseTile == tilemap.GetTile(coordinate))
                coordinates.Add(coordinate);
        }
        return coordinates.ToArray();
    }
}