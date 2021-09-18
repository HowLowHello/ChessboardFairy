using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] piecesPrefabs;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;

    private Dictionary<string, GameObject> nameToPieceDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach(var piece in piecesPrefabs)
        {
            this.nameToPieceDictionary.Add(piece.GetComponent<Piece>().GetType().ToString(), piece);
        }
    }

    public GameObject CreatePiece(Type type)
    {
        GameObject prefab = this.nameToPieceDictionary[type.ToString()];
        if (prefab)
        {
            GameObject newPiece = Instantiate(prefab);
            return newPiece;
        }
        return null;
    }

    public Material GetTeamMaterial(TeamColor color)
    {
        return color == TeamColor.White ? whiteMaterial : blackMaterial;
    }
}
