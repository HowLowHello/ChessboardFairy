using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject introCamera;
    [SerializeField] private GameObject mainCamera;

    private void Start()
    {
        StartCoroutine(WaitAndSwitchCamera());
    }

    private IEnumerator WaitAndSwitchCamera()
    {
        yield return new WaitForSeconds(15f);
        SwitchCameraToMain();
    }

    public void SwitchCameraToMain()
    {
        introCamera.SetActive(false);
        mainCamera.SetActive(true);
    }
}
