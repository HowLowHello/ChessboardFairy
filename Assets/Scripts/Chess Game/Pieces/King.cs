using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1,-1),
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down
    };
    private Vector2Int leftCastlingMove;
    private Vector2Int rightCastlingMove;
    private Piece leftRook;
    private Piece rightRook;


    public override List<Vector2Int> SelectAvailableSquares()
    {
        this.availableMoves.Clear();
        SetStandardMoves();
        SetCastlingMoves();
        return availableMoves;
    }

    private void SetCastlingMoves()
    {
        if (this.hasMoved)
        {
            return;
        }

        leftRook = GetPieceInDirection<Rook>(this.color, Vector2Int.left);
        if (leftRook && !leftRook.hasMoved)
        {
            leftCastlingMove = occupiedSqure + Vector2Int.left * 2;
            this.availableMoves.Add(leftCastlingMove);
        }

        rightRook = GetPieceInDirection<Rook>(this.color, Vector2Int.right);
        if (rightRook && !rightRook.hasMoved)
        {
            rightCastlingMove = occupiedSqure + Vector2Int.right * 2;
            this.availableMoves.Add(rightCastlingMove);
        }
    }



    private void SetStandardMoves()
    {
        float range = 1;
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
                if (piece == null)
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
    }

    public override void MovePiece(Vector2Int toCoords)
    {
        base.MovePiece(toCoords);
        if (toCoords == leftCastlingMove)
        {
            board.UpdateBoardOnPieceMove(toCoords + Vector2Int.right, leftRook.occupiedSqure, leftRook, null);
            leftRook.MovePiece(toCoords + Vector2Int.right);
        }
        else if (toCoords == rightCastlingMove)
        {
            board.UpdateBoardOnPieceMove(toCoords + Vector2Int.left, rightRook.occupiedSqure, rightRook, null);
            rightRook.MovePiece(toCoords + Vector2Int.left);
        }
    }
}
