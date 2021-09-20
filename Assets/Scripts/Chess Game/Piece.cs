using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IObjectTweener))]
[RequireComponent(typeof(MaterialSetter))]
public abstract class Piece : MonoBehaviour
{
    private MaterialSetter materialSetter;
    public Board board { protected get; set; }
    public Vector2Int occupiedSqure { get; set; }
    public TeamColor color { get; set;}
    public bool hasMoved { get; private set; }

    public List<Vector2Int> availableMoves;

    private IObjectTweener tweener;
    public abstract List<Vector2Int> SelectAvailableSquares();

    private void Awake()
    {
        this.availableMoves = new List<Vector2Int>();
        this.tweener = GetComponent<IObjectTweener>();
        this.materialSetter = GetComponent<MaterialSetter>();
        this.hasMoved = false;
    }

    public void SetMaterial(Material materialIn)
    {
        if (this is Fairy)
        {
            return;
        }
        this.materialSetter.SetSingleMaterial(materialIn);
        if (this.materialSetter == null)
        {
            this.materialSetter = GetComponent<MaterialSetter>();
        }
    }

    public bool IsFromSameTeam(Piece anotherPiece)
    {
        return this.color == anotherPiece.color;
    }

    public bool IsAttackingPieceOfType<T>() where T : Piece
    {
        foreach (var square in availableMoves)
        {
            if(board.GetPieceOnSquare(square) is T)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        return this.availableMoves.Contains(coords);
    }

    public virtual void MovePiece(Vector2Int toCoords)
    {
        Vector3 toPosition = board.CalculatePositionFromCoords(toCoords);
        this.occupiedSqure = toCoords;
        this.hasMoved = true;
        this.tweener.MoveTo(transform, toPosition);
    }

    protected void TryToAddMove(Vector2Int coords)
    {
        this.availableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, TeamColor color, Board board)
    {
        this.color = color;
        this.occupiedSqure = coords;
        this.board = board;
        transform.position = board.CalculatePositionFromCoords(coords);
    }

    protected Piece GetPieceInDirection<T>(TeamColor color, Vector2Int direction) where T : Piece
    {
        for (int i = 1; i <= Board.BOARD_SIZE; i++)
        {
            Vector2Int nextCoords = occupiedSqure + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatedAreOnBoard(nextCoords))
            {
                return null;
            }
            if (piece != null)
            {
                if (piece.color != this.color || !(piece is T))
                {
                    return null;
                }
                else if (piece.color == this.color && piece is T)
                {
                    return piece;
                }
            }
        }
        return null;
    }
}
