using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> SelectAvailableSquares()
    {
        this.availableMoves.Clear();
        Vector2Int direction = this.color == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = this.hasMoved ? 1 : 2;
        for (int i = 1; i <= range; i++)
        {
            Vector2Int nextCoords = this.occupiedSqure + direction * i;
            Piece piece = this.board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatedAreOnBoard(nextCoords))
            {
                break;
            }
            //
            if (piece == null)
            {
                this.TryToAddMove(nextCoords);
            }
            else if (piece.IsFromSameTeam(this))
            {
                break;
            }
        }

        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y)};
        for (int i = 0; i < takeDirections.Length; i++)
        {
            Vector2Int nextCoords = this.occupiedSqure + takeDirections[i];
            Piece piece = this.board.GetPieceOnSquare(nextCoords);
            //
            if (piece != null && !piece.IsFromSameTeam(this))
            {
                this.TryToAddMove(nextCoords);
            }
        }

        return availableMoves;
    }

    public override void MovePiece(Vector2Int toCoords)
    {
        base.MovePiece(toCoords);
        this.CheckPromotion();
    }

    private void CheckPromotion()
    {
        int endOfBoardYCoord = this.color == TeamColor.White ? (Board.BOARD_SIZE - 1) : 0;
        if (occupiedSqure.y == endOfBoardYCoord)
        {
            board.PromotePawn(this);
        }
    }
}