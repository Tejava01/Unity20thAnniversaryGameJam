using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage : MonoBehaviour
{
    public int flashes;
    public float flash_duration;
    public FlashEffect flash;

    private void Start()
    {
        StartCoroutine(flashCoroutine());
    }

    private IEnumerator flashCoroutine()
    {
        for (int i = 0; i < flashes; i++)
        {
            flash.flash(flash_duration);
            yield return new WaitForSeconds(flash_duration);
            if (i < flashes - 1)
            {
                yield return new WaitForSeconds(flash_duration);
            }
        }
        Destroy(gameObject);
    }
}
