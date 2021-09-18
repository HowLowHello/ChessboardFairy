using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(1, 2),
        new Vector2Int(-1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-1, -2),
        new Vector2Int(2, 1),
        new Vector2Int(-2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(-2, -1)

    };

    public override List<Vector2Int> SelectAvailableSquares()
    {
        this.availableMoves.Clear();
        for (int i=0; i < offsets.Length; i++)
        {
            Vector2Int nextCoords = occupiedSqure + offsets[i];
            Piece piece = this.board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatedAreOnBoard(nextCoords))
            {
                continue;
            }
            if (piece == null || !piece.IsFromSameTeam(this))
            {
                this.TryToAddMove(nextCoords);
            }
        }

        return availableMoves;
    }
}