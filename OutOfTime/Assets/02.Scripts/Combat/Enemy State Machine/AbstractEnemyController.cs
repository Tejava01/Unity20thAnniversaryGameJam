using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AbstractEnemyController<T> : MonoBehaviour
    where T : AbstractEnemyController<T>
{
    public Animator animator;

    [SerializeField] private HealthManager health_manager;
    [SerializeField] private FlashEffect flash;


    [SerializeField] private GameObject dropOnDeath;
    [SerializeField] private float dropChance;


    [SerializeField] private AudioClip hurtSound;
    private AbstractEnemyState<T> state;
    private bool dead = false;


    private void Awake()
    {
        health_manager.onDamage += onDamage;
        health_manager.onDeath += onDeath;
    }

    private void OnDisable()
    {
        health_manager.onDamage -= onDamage;
        health_manager.onDeath -= onDeath;
    }


    public void transitionState(AbstractEnemyState<T> new_state)
    {
        if (dead) return;
        if (state != null) state.exit((T)this);
        state = new_state;
        state.enter((T)this); // safe cast due to CRTP
    }
    private void Update()
    {
        if (dead) return;
        state.update((T)this);
    }

    void FixedUpdate()
    {
        if (dead) return;
        state.fixedUpdate((T)this);
    }
    
    

    private void onDamage()
    {
        if (dead) return;
        flash.flash(1 / 6f);
        SoundManager.instance.playClip(hurtSound);
    }

    private void onDeath()
    {
        StartCoroutine(deathCoroutine());
    }

    private IEnumerator deathCoroutine()
    {
        dead = true;
        flash.flash(1 / 3f);

        yield return new WaitForSeconds(1 / 3f);
        animator.Play("death");
        

        yield return new WaitForSeconds(2 / 3f);
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll < dropChance && dropOnDeath != null)
        {
            Instantiate(dropOnDeath, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
