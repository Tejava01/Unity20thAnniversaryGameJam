using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : AbstractEnemyState<RangedEnemy>
{
    private readonly float time_before_impact = 4 / 6f;
    private readonly float time_after_impact =  2 / 6f;
    public override void enter(RangedEnemy controller)
    {
        if (Vector2.Distance(controller.transform.position, PlayerController.instance.transform.position) > controller.range)
        {
            controller.transitionState(controller.chase);
            return;
        }
        controller.StartCoroutine(attackCoroutine(controller));
    }

    private IEnumerator attackCoroutine(RangedEnemy controller)
    {
        controller.animator.Play("attack");

        yield return new WaitForSeconds(time_before_impact);
        controller.spawnProjectile((PlayerController.instance.transform.position - controller.transform.position).normalized);

        yield return new WaitForSeconds(time_after_impact);
        controller.transitionState(controller.chase);
    }

    public override void fixedUpdate(RangedEnemy controller)
    {
        controller.movement.move(Vector2.zero, true);
    }
}
