using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    [SerializeField] internal Material freeSquareMaterial;
    [SerializeField] internal Material opponentSquareMaterial;
    [SerializeField] internal Material teleportationSquareMaterial;
    [SerializeField] internal GameObject selectorPrefab;
    private List<GameObject> instantiatedSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3, bool> squareData)
    {
        this.ClearAllSelectors();
        foreach (var data in squareData)
        {
            GameObject selector = Instantiate(this.selectorPrefab, data.Key, Quaternion.identity);
            this.instantiatedSelectors.Add(selector);
            foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
            {
                setter.SetSingleMaterial(data.Value ? freeSquareMaterial : opponentSquareMaterial);
            }
        }
    }

    public void ShowTeleportSelection(List<Vector3> squares)
    {
        foreach (var square in squares)
        {
            GameObject selector = Instantiate(this.selectorPrefab, square, Quaternion.identity);
            this.instantiatedSelectors.Add(selector);
            foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
            {
                setter.SetSingleMaterial(teleportationSquareMaterial);
            }
        }
    }

    public void ClearAllSelectors()
    {
        foreach (var selector in instantiatedSelectors)
        {
            GameObject.Destroy(selector.gameObject);
        }
        this.instantiatedSelectors.Clear();
    }
}
