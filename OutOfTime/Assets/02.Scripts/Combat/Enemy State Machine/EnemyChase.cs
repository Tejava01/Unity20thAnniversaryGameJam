using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : AbstractEnemyState<RangedEnemy>
{
    private float timer;
    private Vector2 direction;


    private int cycle;
    private readonly int cycles_between_attack;
    private readonly float time_in_cycle;


    public EnemyChase(int cycles_between_attack, float time_in_cycle)
    {
        this.cycles_between_attack = cycles_between_attack;
        this.time_in_cycle = time_in_cycle;
        cycle = 0;
    }


    public override void enter(RangedEnemy controller)
    {
        timer = time_in_cycle;


        Vector2 toPlayer = PlayerController.instance.transform.position - controller.transform.position;
        
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= controller.range)
        {
            Debug.Log("distance to player: " + distanceToPlayer + ", enemy range: " + controller.range + ". Strafing!");
            int multiplier = Random.Range(0, 2) * 2 - 1; // Randomly -1 or 1
            Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x).normalized;
            direction = perpendicular * multiplier;
        }
        else
            direction = toPlayer.normalized;
    }

    public override void update(RangedEnemy controller)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            cycle++;
            if (cycle >= cycles_between_attack)
            {
                cycle = 0;
                controller.transitionState(controller.attack);
                return;
            }
            timer = time_in_cycle;
            controller.transitionState(controller.chase);
        }
    }


    public override void fixedUpdate(RangedEnemy controller)
    {
        controller.movement.move(direction, false);
    }
}
