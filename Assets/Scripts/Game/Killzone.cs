using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField] AudioClip deathClip;
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Entity/Player") && !ItemManager.instance.epreuveHasEnded && ItemManager.instance.epreuveHasStarted)
        {
            ItemManager.instance.epreuveHasEnded = true;
            GameManager.instance.isGameOver = true;

            AudioManager.instance.Play(deathClip);

            GameManager.instance.Restart();
        }
    }
}
