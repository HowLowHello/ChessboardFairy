using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessUIController : MonoBehaviour
{
    [SerializeField] private GameObject UIParent;
    [SerializeField] private Text resultText;

    public void HideUI()
    {
        UIParent.SetActive(false);
    }

    public void OnGameFinished(string message)
    {
        UIParent.SetActive(true);
        resultText.text = string.Format(message);
    }

}
