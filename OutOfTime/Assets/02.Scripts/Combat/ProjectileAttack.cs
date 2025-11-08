using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected float speed;


    [SerializeField] private int damage;
    public enum AFFINITY
    {
        PLAYER, ENEMY, NONE
    }
    [SerializeField] private AFFINITY affinity;
    [SerializeField] private float knockback;
    [SerializeField] private GameObject deathPrefab;      // spawns this game object when it destroys self


    [SerializeField] protected AudioClip sound;




    private void Start()
    {
        rb.velocity = transform.up * speed;
        SoundManager.instance.playClip(sound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Environment")) destroyObject();
        resolveHit(other);
    }

    private void resolveHit(Collider2D hit)
    {
        if (affinity == AFFINITY.PLAYER)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.gameObject.GetComponent<HealthManager>().takeDamage(damage);

                EnemyMovement movement = hit.gameObject.GetComponent<EnemyMovement>();
                movement.StartCoroutine(movement.applyKnockback(transform.up.normalized * knockback));
                destroyObject();
            }
        }
        if (affinity == AFFINITY.ENEMY)
        {
            if (hit.CompareTag("Player"))
            {
                if (PlayerController.instance.parry_active)
                {
                    transform.up = PlayerController.instance.onParrySuccess();
                    rb.velocity = transform.up * speed;
                    affinity = AFFINITY.PLAYER;
                    return;
                }
                hit.gameObject.GetComponent<HealthManager>().takeDamage(damage);
                destroyObject();
            }
        }
    }

    private void destroyObject()
    {
        if (deathPrefab != null)
        {
            GameObject deathObject = Instantiate(deathPrefab, transform.position, Quaternion.identity);
            deathObject.transform.up = transform.up;
        }
        Destroy(gameObject);
    }
}
