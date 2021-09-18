using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InstantTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] private float speed;
    [SerializeField] private float height;

    public void MoveTo(Transform transform, Vector3 toPosition)
    {
        float distance = Vector3.Distance(toPosition, transform.position);
        transform.DOJump(toPosition, height, 1, distance / speed);
    }
}
