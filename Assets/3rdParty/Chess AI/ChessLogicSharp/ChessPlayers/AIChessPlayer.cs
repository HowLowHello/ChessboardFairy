using System.Collections;
using System.Threading;
using ChessLogicSharp.DataStructures;
using ExampleChessGame;
using UnityEngine;

namespace ChessLogicSharp.ChessPlayers
{
    /// <summary>
    /// A example of an AI chess player. When it's its turn it creates a new thread and calculates the best move and then makes the move.
    /// </summary>
    public class AIChessPlayer : ChessPlayer
    {
        private readonly MinMaxMoveCalc _moveCalc;
        private Board _board;
        private ChessGame _game;
    
        public AIChessPlayer(ChessGame game, Board board, Player player, int searchDepth) : base(board, player)
        {
            _game = game;
            _moveCalc = new MinMaxMoveCalc(searchDepth);
            _board = board;

        }

        public void MakeFirstMove()
        {
            if (_board.PlayerTurn == Player)
            {
                Debug.Log("Player starts calculating");
                //ThreadPool.QueueUserWorkItem((state) => CalculateAndMove());
                this.CalculateAndMove();
            }
        }

        protected override void OnTurnSwapped(Player player)
        {
            if (player == Player)
            {
                //ThreadPool.QueueUserWorkItem((state) => CalculateAndMove());
                _game.WaitForNextMove(this);
            }
        }

        public IEnumerator WaitAndMakeNextMove()
        {
            yield return new WaitForSeconds(6f);
            CalculateAndMove();
        }
        
        public void CalculateAndMove()
        {

            BoardPieceMove move = _moveCalc.GetBestMove(Board);
            MovePiece(move);

            PieceMoveData moveData = new PieceMoveData(new Vector2Int(move.From.X, move.From.Y), new Vector2Int(move.To.X, move.To.Y)); ;
            _game.chessGameController.movesToApply.Add(moveData);

            Debug.Log("An AI Piece Move has been made by" + Board.PlayerTurn);
            Debug.Log("From: (" + move.From.X +","+ move.From.Y +")");
            Debug.Log("To: (" + move.To.X +","+ move.To.Y + ")");
            
        }
    }
}