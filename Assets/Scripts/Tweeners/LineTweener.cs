using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] private float speed;

    public void MoveTo(Transform transform, Vector3 toPosition)
    {
        float distance = Vector3.Distance(toPosition, transform.position);
        transform.DOMove(toPosition, distance / speed);
    }
}
