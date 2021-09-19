using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : Piece
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
    private int tpCharges = 0;
    public List<Vector2Int> tpMoves;
    public List<Vector2Int> SelectTPSquares()
    {
        this.tpMoves.Clear();
        SetTeleportationMoves();
        return tpMoves;
    }


    private void SetTeleportationMoves()
    {
        if (this.tpCharges > 0)
        {
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
                    if (piece == null)
                    {
                        this.addTPMove(nextCoords);
                    }
                }
            }
        }
    }

    private void addTPMove(Vector2Int coords)
    {
        this.tpMoves.Add(coords);
    }

    public bool CanTeleportTo(Vector2Int coords)
    {
        return this.tpMoves.Contains(coords);
    }

    public override List<Vector2Int> SelectAvailableSquares()
    {
        this.availableMoves.Clear();
        SetStandardMoves();
        return availableMoves;
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

    public void MoveFairy(Vector2Int toCoords, bool isTPMove)
    {
        if (isTPMove)
        {
            this.tpCharges = tpCharges - 1;
            this.MovePiece(toCoords);
        }
        else
        {
            this.MovePiece(toCoords);
        }
    }
}
