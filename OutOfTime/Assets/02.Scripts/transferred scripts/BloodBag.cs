using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBag : MonoBehaviour
{
    [SerializeField] private int healAmount;
    [SerializeField] private AudioClip onPickupSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<HealthManager>().gainHealth(healAmount);
            SoundManager.instance.playClip(onPickupSound);
            Destroy(gameObject);
        }
    }
}
