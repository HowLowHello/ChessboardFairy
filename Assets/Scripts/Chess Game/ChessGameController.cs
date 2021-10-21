using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PieceAudioManager))]
[RequireComponent(typeof(PiecesCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Finished }
    private GameState gameState;
    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private ChessUIController uiController;
    private PieceAudioManager audioManager;
    private PiecesCreator piecesCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer fairyPlayer;
    private ChessPlayer activePlayer;
    private ChessPlayer previousPlayer;

    private void Awake()
    {
        this.SetDependencies();
        this.CreatePlayers();
    }

    private void CreatePlayers()
    {
        this.whitePlayer = new ChessPlayer(TeamColor.White, board);
        this.blackPlayer = new ChessPlayer(TeamColor.Black, board);
        this.fairyPlayer = new ChessPlayer(TeamColor.Fairy, board);
    }

    private void SetDependencies()
    {
        this.piecesCreator = GetComponent<PiecesCreator>();
        this.audioManager = GetComponent<PieceAudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.StartNewGame();
    }

    private void StartNewGame()
    {
        this.SetGameState(GameState.Init);
        this.board.SetDependencies(this);
        this.uiController.HideUI();
        this.CreatePiecesFromLayout(this.startingBoardLayout);
        this.activePlayer = whitePlayer;
        this.GenerateAllPossiblePlayerMoves(activePlayer);
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
        this.fairyPlayer.activePieces.ForEach(p => GameObject.Destroy(p.gameObject));
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
        if (color == TeamColor.Fairy)
        {
            Piece newFairy = this.piecesCreator.CreatePiece(type).GetComponent<Piece>();
            newFairy.SetData(squareCoords, color, board);
            board.SetPieceOnBoard(squareCoords, newFairy);
            fairyPlayer.AddPiece(newFairy);
        }
        else
        {
            Piece newPiece = this.piecesCreator.CreatePiece(type).GetComponent<Piece>();
            newPiece.SetData(squareCoords, color, board);

            Material teamMaterial = piecesCreator.GetTeamMaterial(color);
            newPiece.SetMaterial(teamMaterial);

            board.SetPieceOnBoard(squareCoords, newPiece);

            ChessPlayer currentPlayer = color == TeamColor.White ? whitePlayer : blackPlayer;
            currentPlayer.AddPiece(newPiece);
        }

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
        this.GenerateAllPossiblePlayerMoves(whitePlayer);
        this.GenerateAllPossiblePlayerMoves(blackPlayer);
        this.GenerateAllPossiblePlayerMoves(fairyPlayer);
        if (activePlayer != fairyPlayer)
        {
            if (CheckIfPlayerCanFinishGame(activePlayer))
            {
                this.EndGame(activePlayer);
            }
            else
            {
                this.ChangeActiveTeam();
                return;
            }
        }
        else
        {
            this.ChangeActiveTeam();
        }
    }

    private void EndGame(ChessPlayer player)
    {
        if (player.teamColor == TeamColor.Fairy)
        {
            this.uiController.OnGameFinished("You Fail");
            this.SetGameState(GameState.Finished);
            Debug.Log("Game Finished");
        }
        else
        {
            this.uiController.OnGameFinished("You survived to the last turn! Congratulations");
            this.SetGameState(GameState.Finished);
            Debug.Log("Game Finished");
        }
    }

    private bool CheckIfPlayerCanFinishGame(ChessPlayer player)
    {
        // player should be activePlayer
        Piece[] kingAttackingPieces = player.GetPiecesAttackingOppositePieceOfType<King>();
        if (kingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = this.GetOpponentTo(player);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(player, attackedKing);

            int availableKingMoves = attackedKing.availableMoves.Count;
            if (availableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(player);
                if (!canCoverKing)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ChangeActiveTeam()
    {
        if (activePlayer != fairyPlayer)
        {
            this.previousPlayer = activePlayer;
        }
        this.activePlayer = GetPlayerNextTo(activePlayer);
    }

    private ChessPlayer GetPlayerNextTo(ChessPlayer player)
    {
        if (player == fairyPlayer)
            return GetOpponentTo(previousPlayer);
        else
            return fairyPlayer;
    }

    private ChessPlayer GetOpponentTo(ChessPlayer player)
    {
        if (player == fairyPlayer)
        {
            Debug.LogError("GetOpponentTo() produces a bug");
        }
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }


    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(this.GetOpponentTo(activePlayer), piece);
    }

    public void OnPieceRemoved(Piece piece)
    {
        if (piece is Fairy)
        {
            Fairy fairy = (Fairy)piece;
            if (fairy.hasProtectionSheild)
            {
                fairy.hasProtectionSheild = false;
                fairy.OnShieldBroken();
                this.SetPieceOnRandomCoords(fairy);
            }
            else
            {
                this.EndGame(this.fairyPlayer);
            }
            return;

        }
        else
        {
            ChessPlayer pieceOwner = (piece.color == TeamColor.White) ? whitePlayer : blackPlayer;
            pieceOwner.RemovePiece(piece);
            GameObject.Destroy(piece.gameObject);
        }

    }

    private void SetPieceOnRandomCoords(Fairy fairy)
    {
        System.Random random = new System.Random();
        Vector2Int nextCoords = new Vector2Int(random.Next(0, Board.BOARD_SIZE + 1), random.Next(0, Board.BOARD_SIZE + 1));
        while (!board.CheckIfCoordinatedAreOnBoard(nextCoords) || board.GetPieceOnSquare(nextCoords) != null)
        {
            nextCoords = new Vector2Int(random.Next(0, Board.BOARD_SIZE + 1), random.Next(0, Board.BOARD_SIZE + 1));
        }
        board.UpdateBoardOnPieceMove(nextCoords, fairy.occupiedSqure, fairy, null);
        fairy.MovePiece(nextCoords);
        return;
    }

    public void PlayTakePieceAudio()
    {
        audioManager.PlayPieceTakenSound();
    }

    public void PlayMovePieceAudio()
    {
        audioManager.PlayMovementSound();
    }
}
