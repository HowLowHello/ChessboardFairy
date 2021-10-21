using EasyGameStudio.Jeremy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FairyEffectsCreator : MonoBehaviour
{
    [SerializeField] private GameObject magicCirclePrefab;
    [SerializeField] private GameObject magicBeamPrefab;
    [SerializeField] private GameObject traceSquarePrefab;
    [SerializeField] private Material traceSquareMaterial;
    [SerializeField] private GameObject forceShieldPrefab;
    private GameObject instantiatedMagicCircle;
    private GameObject instantiatedMagicBeam;
    private GameObject protectionShield;
    public List<TraceUnit> traceSquares = new List<TraceUnit>();


    public bool hasTeleportEffectActivated()
    {
        return this.instantiatedMagicCircle != null;
    }

    internal void InstantiateShield(Fairy fairy)
    {
        GameObject shield = Instantiate(forceShieldPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        protectionShield = shield;
        protectionShield.transform.SetParent(fairy.transform);
        
    }

    internal void OnShieldBroken()
    {
        Force_field_control shieldController = protectionShield.GetComponent<Force_field_control>();
        shieldController.hide();

    }

    public void UpdateTrace(Fairy fairy)
    {
        int traceLength = (int)Math.Floor((double)(fairy.board.nonpawnPiecesTakenOut / 2));

        if (traceLength < traceSquares.Count)
        {
            this.RemoveFirstUnit();
        }
        else if (traceLength == 0)
        {
            return;
        }
        else if (traceLength == traceSquares.Count)
        {
            this.TryToAddTrace(fairy.occupiedSqure, fairy.transform.position);
            this.RemoveFirstUnit();
        }
        else if (traceLength > traceSquares.Count)
        {
            this.TryToAddTrace(fairy.occupiedSqure, fairy.transform.position);
        }
    }

    private void TryToAddTrace(Vector2Int coords, Vector3 pos)
    {
        if (traceSquares.Exists((TraceUnit u) => u != null))
        {
            TraceUnit lastUnit = traceSquares.FindLast((TraceUnit u) => u.nextUnit == null);
            TraceUnit nextUnit = this.CreateUnitAndInstantiate(coords, pos);
            if (lastUnit != null)
            {
                lastUnit.nextUnit = nextUnit;
                traceSquares.Add(nextUnit);
            }
            else
            {
                Debug.LogError("Last Trace Unit could not be found. ");
            }
            return;
        }
        else
        {
            traceSquares.Add(this.CreateUnitAndInstantiate(coords, pos));
        }


    }

    private TraceUnit CreateUnitAndInstantiate(Vector2Int coords, Vector3 pos)
    {
        GameObject traceSquare = Instantiate(traceSquarePrefab, pos, Quaternion.identity);
        foreach (var setter in traceSquare.GetComponentsInChildren<MaterialSetter>())
        {
            setter.SetSingleMaterial(traceSquareMaterial);
        }
        TraceUnit traceUnit = new TraceUnit(traceSquare, coords);
        return traceUnit;
    }

    private void RemoveFirstUnit()
    {
        if (traceSquares[0] != null)
        {
            GameObject.Destroy(traceSquares[0].traceSquare);
            traceSquares.Remove(traceSquares[0]);
        }
    }

    public void CreateTeleportEffects(Fairy fairy, Vector3 toPos)
    {
        instantiatedMagicCircle = Instantiate(this.magicCirclePrefab, fairy.transform.position, Quaternion.identity);
        instantiatedMagicBeam = Instantiate(this.magicBeamPrefab, toPos, Quaternion.identity);

        Invoke("RemoveTeleportEffects", 1.2f);
    }

    public void RemoveTeleportEffects()
    {
        GameObject.Destroy(this.instantiatedMagicCircle.gameObject);
        GameObject.Destroy(this.instantiatedMagicBeam.gameObject);
        this.instantiatedMagicCircle = null;
        this.instantiatedMagicBeam = null;
    }


    public class TraceUnit
    {
        public GameObject traceSquare { get; private set; }
        public Vector2Int coords { get; private set; }
        public TraceUnit nextUnit;

        public TraceUnit(GameObject traceSquare, Vector2Int coords)
        {
            this.traceSquare = traceSquare;
            this.coords = coords;
        }
    }
}
