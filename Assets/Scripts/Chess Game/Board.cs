using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour
{

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;    
    private ChessGameController chessGameController;
    private SquareSelectorCreator squareSelectorCreator;
    // 2d array to store pieces
    private Piece[,] grid;
    private Piece selectedPiece;
    // board size, length / width
    public const int BOARD_SIZE = 8;

    private void Awake()
    {
        this.squareSelectorCreator = GetComponent<SquareSelectorCreator>();
        this.CreateGrid();
    }

    private void CreateGrid()
    {
        this.grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }


    public void SetDependencies(ChessGameController chessGameController)
    {
        this.chessGameController = chessGameController;
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }

    public void OnGameRestarted()
    {
        this.selectedPiece = null;
        this.CreateGrid();
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        if (!this.chessGameController.IsGameInProgress())
        {
            return;
        }
        Vector2Int coords = this.CalculateCoordsFromPosition(inputPosition);
        Piece piece = this.GetPieceOnSquare(coords);
        // case that there's already a peice selected
        if (selectedPiece)
        {
            // case that player selects the same piece, should deselect
            if (piece != null && selectedPiece == piece)
            {
                this.DeselectPiece();
            }
            // case that player selects another piece (from the same team)
            else if (piece != null && selectedPiece != piece && this.chessGameController.IsTeamTurnActive(piece.color))
            {
                this.SelectPiece(piece);
            }
            // case that player clicks an empty square, should try to move the piece
            else if (this.selectedPiece.CanMoveTo(coords))
            {
                this.OnSelectedPieceMoved(coords);
            }

        }
        else
        {
            if (piece != null && chessGameController.IsTeamTurnActive(piece.color))
            {
                this.SelectPiece(piece);
            }

        }
    }

    internal void PromotePawn(Pawn pawn)
    {
        this.TakePiece(pawn);
        this.chessGameController.CreatePieceAndInitialize(pawn.occupiedSqure, pawn.color, typeof(Queen));
    }

    private void SelectPiece(Piece piece)
    {
        this.chessGameController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece);
        this.selectedPiece = piece;
        List<Vector2Int> selection = this.selectedPiece.availableMoves;
        this.ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for(int i = 0; i < selection.Count; i++)
        {
            Vector3 position = this.CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = (this.GetPieceOnSquare(selection[i]) == null);
            squaresData.Add(position, isSquareFree);
        }
        this.squareSelectorCreator.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        this.selectedPiece = null;
        this.squareSelectorCreator.ClearSelection();
    }
    private void OnSelectedPieceMoved(Vector2Int toCoords)
    {
        this.TryToTakeOppositePiece(toCoords);
        this.UpdateBoardOnPieceMove(toCoords, this.selectedPiece.occupiedSqure, this.selectedPiece, null);
        this.selectedPiece.MovePiece(toCoords);
        this.DeselectPiece();
        this.chessGameController.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        this.grid[oldCoords.x, oldCoords.y] = oldPiece;
        this.grid[newCoords.x, newCoords.y] = newPiece;
    }    
    
    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        if (piece != null && !selectedPiece.IsFromSameTeam(piece))
        {
            this.TakePiece(piece);
        }
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            this.grid[piece.occupiedSqure.x, piece.occupiedSqure.y] = null;
            this.chessGameController.OnPieceRemoved(piece);
        }
    }

    public bool HasPiece(Piece piece)
    {
        for (int i=0; i < BOARD_SIZE; i++)
        {
            for (int j=0; j < BOARD_SIZE; j++)
            {
                if(grid[i, j] == piece)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (this.CheckIfCoordinatedAreOnBoard(coords))
        {
            return grid[coords.x, coords.y];
        }
        else return null;
    }

    public bool CheckIfCoordinatedAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
        {
            return false;
        }
        else return true;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (this.CheckIfCoordinatedAreOnBoard(coords))
        {
            this.grid[coords.x, coords.y] = piece;
        }
    }
}
