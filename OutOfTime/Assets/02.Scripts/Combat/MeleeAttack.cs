using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private ProjectileAttack.AFFINITY affinity;
    [SerializeField] private float knockback;

    [SerializeField] private AudioClip sound;



    private void Start()
    {
        Collider2D[] results = new Collider2D[10]; // Adjust size as needed
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemy", "Player"));

        int hitCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, results);
        for (int i = 0; i < hitCount; i++)
        {
            resolveHit(results[i]);
        }

        SoundManager.instance.playClip(sound);
    }

    private void resolveHit(Collider2D hit)
    {
        if (affinity == ProjectileAttack.AFFINITY.PLAYER)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.gameObject.GetComponent<HealthManager>().takeDamage(damage);
                EnemyMovement movement = hit.gameObject.GetComponent<EnemyMovement>();

                movement.StartCoroutine(movement.applyKnockback(transform.up.normalized * knockback));
            }
        }
        if (affinity == ProjectileAttack.AFFINITY.ENEMY)
        {
            if (hit.CompareTag("Player"))
            {
                hit.gameObject.GetComponent<HealthManager>().takeDamage(damage);
            }
        }
    }
}