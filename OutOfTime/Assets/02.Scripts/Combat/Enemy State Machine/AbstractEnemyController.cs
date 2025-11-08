using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private Animator animator;

    [SerializeField] private float range;
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private FlashEffect damageFlash;

    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private float attackCooldown;
    [SerializeField] private bool isProjectile;


    [SerializeField] private GameObject dropOnDeath;
    [SerializeField] private float dropChance;


    [SerializeField] private AudioClip hurtSound;

    private bool dead;


    private float time;         // used for managing attack cooldown

    private void Awake()
    {
        Debug.Log("awake: subscribing to hm events");
        healthManager.onDamage += onDamage;
        healthManager.onDeath += onDeath;
        dead = false;
    }

    private void OnDisable()
    {
        healthManager.onDamage -= onDamage;
        healthManager.onDeath -= onDeath;
    }

    private void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (dead) return;

        Vector2 toPlayer = PlayerController.instance.transform.position - transform.position;
        if (toPlayer.magnitude < range)
        {
            if (time <= 0)
            {
                Vector2 offset = Vector2.zero;
                if (!isProjectile)
                    offset = toPlayer.normalized * 0.7f;
                animator.Play("attack");
                Invoke("spawnAttack", 2 / 3f);

                time = attackCooldown;

            }
            movement.move(toPlayer, true);
        }
        else
            movement.move(toPlayer, false);
    }
    
    private void spawnAttack()
    {
        Vector2 toPlayer = PlayerController.instance.transform.position - transform.position;
        GameObject attackObject = Instantiate(attackPrefab, (Vector2)transform.position + toPlayer.normalized * 0.7f, Quaternion.identity);
        attackObject.transform.up = toPlayer.normalized;
    }








    private void onDamage()
    {
        damageFlash.flash(1 / 3f);
        SoundManager.instance.playClip(hurtSound);
    }

    private void onDeath()
    {
        if (dead) return;
        dead = true;
        StartCoroutine(deathCoroutine());
    }

    private IEnumerator deathCoroutine()
    {
        damageFlash.flash(1f);

        yield return new WaitForSeconds(2/3f);

        //GameManager.instance.addScore(score);

        float roll = Random.Range(0f, 1f);
        if (roll < dropChance && dropOnDeath != null)
        {
            Instantiate(dropOnDeath, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
