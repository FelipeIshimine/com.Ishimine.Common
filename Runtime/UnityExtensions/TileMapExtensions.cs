using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileMapExtensions
{
    public static Vector3Int[] FindAllCoordinates(this Tilemap tilemap, Func<TileBase,bool> validator)
    {
        BoundsInt boundsInt = tilemap.cellBounds;
        List<Vector3Int> coordinates = new List<Vector3Int>();

        for (int z = boundsInt.zMin; z < boundsInt.zMax; z++)
        for (int y = boundsInt.yMin; y < boundsInt.yMax; y++)
        for (int x = boundsInt.xMin; x < boundsInt.xMax; x++)
        {
            var coordinate = new Vector3Int(x, y, z);
            if(validator.Invoke(tilemap.GetTile(coordinate)))
                coordinates.Add(coordinate);
        }
        return coordinates.ToArray();
    }
    
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


    public static Vector3Int[] GetAllTileCoordinates(this Tilemap tilemap)
    {
        List<Vector3Int> coordinates = new List<Vector3Int>();
        BoundsInt bound = tilemap.cellBounds;
        for (int z = bound.zMin; z < bound.zMax; z++)
        {
            for (int y = bound.yMin; y < bound.yMax; y++)
            {
                for (int x = bound.xMin; x < bound.xMax; x++)
                {
                    var coordinate = new Vector3Int(x, y, z);
                    if (tilemap.HasTile(coordinate))
                        coordinates.Add(coordinate);
                }
            }
        }

        return coordinates.ToArray();
    }

    public static void ApplyOffset(this Tilemap tilemap, Vector2Int offset) =>
        ApplyOffset(tilemap, new Vector3Int(offset.x, offset.y));

    public static void ApplyOffset(this Tilemap tilemap, Vector3Int offset)
    {
        Dictionary<Vector3Int, TileBase> _allTiles = new Dictionary<Vector3Int, TileBase>();

        BoundsInt bound = tilemap.cellBounds;
        for (int z = bound.zMin; z < bound.zMax; z++)
        {
            for (int y = bound.yMin; y < bound.yMax; y++)
            {
                for (int x = bound.xMin; x < bound.xMax; x++)
                {
                    var coordinate = new Vector3Int(x, y, z);
                    var tile = tilemap.GetTile(coordinate);
                    if (tile) _allTiles.Add(coordinate, tile);
                }
            }
        }
        
        
        tilemap.ClearAllTiles();
        foreach (var pair in _allTiles)
            tilemap.SetTile(pair.Key + offset,pair.Value );
    }
}