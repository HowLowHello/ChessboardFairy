using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PiecesCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Finished}
    private GameState gameState;
    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private ChessUIController uiController;
    private PiecesCreator piecesCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;

    private void Awake()
    {
        this.SetDependencies();
        this.CreatePlayers();
    }

    private void CreatePlayers()
    {
        this.whitePlayer = new ChessPlayer(TeamColor.White, board);
        this.blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    private void SetDependencies()
    {
        this.piecesCreator = GetComponent<PiecesCreator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.StartNewGame();
    }

    private void StartNewGame()
    {
        this.SetGameState(GameState.Init);
        this.uiController.HideUI();
        this.CreatePiecesFromLayout(this.startingBoardLayout);
        this.activePlayer = whitePlayer;
        this.GenerateAllPossiblePlayerMoves(activePlayer);
        this.board.SetDependencies(this);
        this.SetGameState(GameState.Play);
    }

    public void RestartGame()
    {
        this.DestroyPieces();
        this.board.OnGameRestarted();
        this.whitePlayer.OnGameRestarted();
        this.blackPlayer.OnGameRestarted();
        this.StartNewGame();
    }

    private void DestroyPieces()
    {
        this.whitePlayer.activePieces.ForEach(p => GameObject.Destroy(p.gameObject));
        this.blackPlayer.activePieces.ForEach(p => GameObject.Destroy(p.gameObject));
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++) 
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndes(i);
            TeamColor color = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndes(i);

            Type type = Type.GetType(typeName);
            this.CreatePieceAndInitialize(squareCoords, color, type);
        }
    }
    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor color, Type type)
    {
        if (this.piecesCreator == null)
        {
            this.piecesCreator = GetComponent<PiecesCreator>();
        }

        Piece newPiece = this.piecesCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, color, board);

        Material teamMaterial = piecesCreator.GetTeamMaterial(color);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = color == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    private void SetGameState(GameState state)
    {
        this.gameState = state;
    }

    public bool IsGameInProgress()
    {
        return this.gameState == GameState.Play;
    }

    public bool IsTeamTurnActive(TeamColor color)
    {
        return this.activePlayer.teamColor == color;
    }

    public void EndTurn()
    {
        this.GenerateAllPossiblePlayerMoves(activePlayer);
        this.GenerateAllPossiblePlayerMoves(this.GetOpponentTo(activePlayer));
        if (CheckIfGameIsFinished())
        {
            this.EndGame();
        }
        else
        {
            this.ChangeActiveTeam();
        }
    }

    private void EndGame()
    {
        this.uiController.OnGameFinished(activePlayer.teamColor.ToString());
        this.SetGameState(GameState.Finished);
        Debug.Log("Game Finished");
    }

    private bool CheckIfGameIsFinished()
    {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if (kingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = this.GetOpponentTo(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.availableMoves.Count;
            if (availableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private ChessPlayer GetOpponentTo(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }
    private void ChangeActiveTeam()
    {
        this.activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(this.GetOpponentTo(activePlayer), piece);
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.color == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        GameObject.Destroy(piece.gameObject);
    }
}
