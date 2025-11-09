using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : AbstractEnemyController<RangedEnemy>
{
    public EnemyMovement movement;
    public GameObject projectile_prefab;
    public float range;
    public AbstractEnemyState<RangedEnemy> attack = new EnemyRangedAttack();
    public AbstractEnemyState<RangedEnemy> chase = new EnemyChase(5, 2 / 3f);


    private void Start()
    {
        transitionState(chase);
    }


    public void spawnProjectile(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectile_prefab, transform.position, Quaternion.identity);
        projectile.transform.up = direction;
    }
}
