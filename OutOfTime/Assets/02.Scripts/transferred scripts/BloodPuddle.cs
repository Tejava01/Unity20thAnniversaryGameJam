using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPuddle : MonoBehaviour
{
    [SerializeField] private float knockback;
    [SerializeField] private int initialDamage;
    [SerializeField] private int tickDamage;


    [SerializeField] private Animator animator;

    [SerializeField] private float lifetime;
    [SerializeField] private AudioClip sound;

    private bool active;

    void Start()
    {
        
        StartCoroutine(mainCoroutine());
    }

    private IEnumerator mainCoroutine()
    {
        active = false;

        animator.Play("puddle spawn");
        yield return new WaitForSeconds(0.5f);

        SoundManager.instance.playClip(sound);
        // apply initial damage and knockback

        Collider2D[] results = new Collider2D[10]; // Adjust size as needed
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemy"));

        int hitCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, results);
        for (int i = 0; i < hitCount; i++)
        {
            EnemyMovement movement = results[i].GetComponent<EnemyMovement>();
            movement.StartCoroutine(movement.applyKnockback(transform.up.normalized * knockback));
            results[i].GetComponent<HealthManager>().takeDamage(initialDamage);
        }

        yield return new WaitForSeconds(1 / 6f);
        animator.Play("puddle");



        active = true;
        StartCoroutine(applyTickDamage());

        yield return new WaitForSeconds(lifetime);
        active = false;
        animator.Play("puddle despawn");

        yield return new WaitForSeconds(2/3f);
        Destroy(gameObject);
    }

    private IEnumerator applyTickDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);         // delay after being called

            if (!active) yield break;


            Collider2D[] results = new Collider2D[10]; // Adjust size as needed
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Enemy"));

            int hitCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, results);
            for (int i = 0; i < hitCount; i++)
            {
                results[i].GetComponent<HealthManager>().takeDamage(tickDamage);
            }
        }
    }

}
