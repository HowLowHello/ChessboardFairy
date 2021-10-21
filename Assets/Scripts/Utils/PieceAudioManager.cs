using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceAudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> pieceMoveAudios;
    [SerializeField] private List<AudioClip> pieceTakenAudios;

    public void PlayMovementSound()
    {
        System.Random random = new System.Random();
        int i = random.Next(0, pieceMoveAudios.Count);
        if (pieceMoveAudios[i])
        {
            AudioSource.PlayClipAtPoint(pieceMoveAudios[i], Camera.main.transform.position);
        }
    }

    public void PlayPieceTakenSound()
    {
        System.Random random = new System.Random();
        int i = random.Next(0, pieceTakenAudios.Count);
        if (pieceTakenAudios[i])
        {
            AudioSource.PlayClipAtPoint(pieceTakenAudios[i], Camera.main.transform.position);
        }
    }
}
