using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    private Vector2Int[] directions = new Vector2Int[] { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };

    public override List<Vector2Int> SelectAvailableSquares()
    {
        this.availableMoves.Clear();
        float range = Board.BOARD_SIZE;
        foreach (var direction in directions)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = this.occupiedSqure + direction * i;
                Piece piece = board.GetPieceOnSquare(nextCoords);
                //
                if (!board.CheckIfCoordinatedAreOnBoard(nextCoords))
                {
                    break;
                }
                //
                if(piece == null)
                {
                    this.TryToAddMove(nextCoords);
                }
                else if (!piece.IsFromSameTeam(this))
                {
                    this.TryToAddMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                {
                    break;
                }
            }
        }
        return availableMoves;
    }
}