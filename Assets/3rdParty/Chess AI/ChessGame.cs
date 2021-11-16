using System;
using System.Collections.Generic;
using ChessLogicSharp;
using ChessLogicSharp.ChessPlayers;
using ChessLogicSharp.DataStructures;
using UnityEngine;

namespace ExampleChessGame
{
    [RequireComponent(typeof(ChessGameController))]
    public class ChessGame : MonoBehaviour
    {
        public ChessGameController chessGameController;
        private AIChessPlayer player1;
        private AIChessPlayer player2;

        void Start()
        {
            GetComponent<ChessGame>().enabled = false;
            return;


            // Set dependencies
            chessGameController = GetComponent<ChessGameController>();
            if (chessGameController == null)
            {
                Debug.LogError("Chess Game Controller not found.");
                return;
            }


            // Create the board
            var board = BoardFactory.CreateBoard();

            
            player1 = new AIChessPlayer(this, board, Player.PlayerOne, 3);
            board.AddPlayer(player1);
            player2 = new AIChessPlayer(this, board, Player.PlayerTwo, 3);
            board.AddPlayer(player2);


            player1.MakeFirstMove();
            Debug.Log("Player1 made 1st move.");


            /*

            // Loop until the game is over
            while (board.GameState != GameState.Ended)
            {
                
                // Fetch the current players turn
                Console.Write(board.PlayerTurn.ToFriendlyString() + " enter your move: ");
                var moveString = Console.ReadLine();

                // Ensure the input is in the correct format (xFrom,yFrom,xTo,yTo) - no commas and must be in the correct chess notation e.g. a1a3
                if (!BoardHelpers.ValidMoveRepresentation(moveString))
                {
                    Console.WriteLine("Invalid string, please enter it again.");
                    continue;
                }

                // Fetch the available moves for the player
                var validMoves = new HashSet<BoardPieceMove>();
                ValidMovesCalc.GetValidMovesForPlayer(board, board.PlayerTurn, validMoves);
                
                // Create an instance of the move
                var from = BoardHelpers.ConvertStringRepIntoPos(moveString.Substring(0, 2));
                var to = BoardHelpers.ConvertStringRepIntoPos(moveString.Substring(2, 2));
                var move = new BoardPieceMove(from, to);

                // Make sure the move is legal
                if (!validMoves.Contains(move))
                {
                    Console.WriteLine("Invalid move, please enter it again.");
                    continue;
                }

                // Apply the move
                if (board.PlayerTurn == Player.PlayerOne)
                {
                    player1.ApplyMove(move);
                }
                else
                {
                    player2.ApplyMove(move);
                }
            }

            */
            
            Console.WriteLine(board.PlayerTurn.ToFriendlyString() + " wins!");

            Console.ReadLine();
        }

        public void WaitForNextMove(AIChessPlayer playerAI)
        {
            StartCoroutine(playerAI.WaitAndMakeNextMove());
        }


        private static string GetStringRepForPiece(BoardPiece piece)
        {
            switch (piece.PieceType)
            {
                case PieceType.Castle:
                    return piece.PieceOwner == Player.PlayerOne ? "c" : "C";
                case PieceType.Knight:
                    return piece.PieceOwner == Player.PlayerOne ? "n" : "N";
                case PieceType.Bishop:
                    return piece.PieceOwner == Player.PlayerOne ? "b" : "B";
                case PieceType.Queen:
                    return piece.PieceOwner == Player.PlayerOne ? "q" : "Q";
                case PieceType.King:
                    return piece.PieceOwner == Player.PlayerOne ? "k" : "K";
                case PieceType.Pawn:
                    return piece.PieceOwner == Player.PlayerOne ? "p" : "P";
                default:
                    return "e";
            }
        }

        private static BoardPiece[,] RotateArray(BoardPiece[,] board)
        {
            var ret = new BoardPiece[8, 8];

            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    ret[i, j] = board[j, 8 - i - 1];
                }
            }

            return ret;
        }
    }
}