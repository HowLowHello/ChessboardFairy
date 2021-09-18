using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInputReceiver : InputReceiver
{
    [SerializeField] private UnityEvent clickEvent;

    public override void OnInputReceived()
    {
        Debug.Log("click received");
        foreach (var handler in inputHandlers)
        {
            handler.ProcessInput(Input.mousePosition, gameObject, () => clickEvent.Invoke());
        }
    }

}
