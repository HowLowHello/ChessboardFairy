using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FairyEffectsCreator))]
public class Fairy : Piece
{
    public FairyEffectsCreator fairyEffectsCreator;
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
    internal List<Vector2Int> traces;
    public bool hasProtectionSheild { get; set; }
    public int tpCharges { get; private set; }
    public List<Vector2Int> tpMoves;
    public List<Vector2Int> SelectTPSquares()
    {
        this.tpMoves.Clear();
        SetTeleportationMoves();
        return tpMoves;
    }
    public bool hasTPCharges()
    {
        return this.tpCharges > 0;
    }

    void Awake()
    {
        base.Awake();
        this.tpCharges = 3;
        this.hasProtectionSheild = true;
        this.fairyEffectsCreator = GetComponent<FairyEffectsCreator>();
        this.fairyEffectsCreator.InstantiateShield(this);
    }


    private void SetTeleportationMoves()
    {
        if (this.hasTPCharges())
        {
            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Board.BOARD_SIZE; j++)
                {
                    Vector2Int nextCoords = new Vector2Int(i, j);
                    Piece piece = board.GetPieceOnSquare(nextCoords);
                    if (piece == null)
                    {
                        this.tpMoves.Add(nextCoords);
                    }
                }
            }
            foreach (var coords in availableMoves)
            {
                if (tpMoves.Contains(coords))
                {
                    tpMoves.Remove(coords);
                }
            }
            foreach (var traceSquare in fairyEffectsCreator.traceSquares)
            {
                if (tpMoves.Contains(traceSquare.coords))
                {
                    tpMoves.Remove(traceSquare.coords);
                }
            }
        }
    }


    public bool CanTeleportTo(Vector2Int coords)
    {
        return (this.tpMoves.Contains(coords)) && this.hasTPCharges();
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
                else
                {
                    break;
                }
            }
            foreach (var traceSquare in fairyEffectsCreator.traceSquares)
            {
                if (availableMoves.Contains(traceSquare.coords))
                {
                    availableMoves.Remove(traceSquare.coords);
                }
            }
        }
    }

    public override void MovePiece(Vector2Int toCoords)
    {
        Vector3 toPos = this.board.CalculatePositionFromCoords(toCoords);
        Vector3 fromPos = this.board.CalculatePositionFromCoords(this.occupiedSqure);
        Vector3 relativePos = toPos - fromPos;
        transform.rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));

        this.fairyEffectsCreator.UpdateTrace(this);

        base.MovePiece(toCoords);

    }

    public void PrepareToTeleport(Vector3 toPos)
    {
        this.fairyEffectsCreator.CreateTeleportEffects(this, toPos);
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
    }

    public void Teleport(Vector2Int toCoords)
    {
        if (this.tpCharges > 0)
        {
            this.tpCharges--;
        }
        this.occupiedSqure = toCoords;
        transform.position = board.CalculatePositionFromCoords(toCoords);
        Quaternion.LookRotation(new Vector3(0, 0, -1));

        foreach (var square in fairyEffectsCreator.traceSquares)
        {
            GameObject.Destroy(square.traceSquare);
        }
        this.fairyEffectsCreator.traceSquares.Clear();
        if (this.board.nonpawnPiecesTakenOut - 2 >= 0)
        {
            this.board.nonpawnPiecesTakenOut -= 2;
        }
        else if (this.board.nonpawnPiecesTakenOut == 1)
        {
            this.board.nonpawnPiecesTakenOut --;
        }
        else
        {
            return;
        }

    }

    internal void OnShieldBroken()
    {
        this.fairyEffectsCreator.OnShieldBroken();
    }
}
