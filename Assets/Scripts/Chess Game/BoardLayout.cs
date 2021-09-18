using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Oject/Board/Layout")]
public class BoardLayout : ScriptableObject
{
    [Serializable]
    private class BoardSquareSetup
    {
        public Vector2Int position;
        public PieceType pieceType;
        public TeamColor teamColor;
    }

    [SerializeField] private BoardSquareSetup[] boardSquares;

    public int GetPiecesCount()
    {
        return this.boardSquares.Length;
    }

    public Vector2Int GetSquareCoordsAtIndes(int index)
    {
        if(this.boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range.");
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(boardSquares[index].position.x - 1, boardSquares[index].position.y - 1);
    }

    public string GetSquarePieceNameAtIndes(int index)
    {
        if (this.boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range.");
            return "";
        }
        return boardSquares[index].pieceType.ToString();
    }

    public TeamColor GetSquareTeamColorAtIndex(int index)
    {
        if (this.boardSquares.Length <= index)
        {
            Debug.LogError("Index of piece is out of range.");
            return TeamColor.Black;
        }
        return boardSquares[index].teamColor;
    }
}
