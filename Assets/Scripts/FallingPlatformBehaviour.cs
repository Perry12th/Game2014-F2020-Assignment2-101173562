using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformBehaviour : MonoBehaviour
{
    [SerializeField]
    int disppearingTime = 5;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Disappear());
        }
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disppearingTime);
        Destroy(gameObject);
    }
}
