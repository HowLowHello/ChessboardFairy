using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoveData
{
    public Vector2Int fromCoords;
    public Vector2Int toCoords;

    public PieceMoveData(Vector2Int fromCoords, Vector2Int toCoords)
    {
        this.fromCoords = fromCoords;
        this.toCoords = toCoords;
    }
}
