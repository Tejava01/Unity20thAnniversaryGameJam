using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : AbstractPlayerState
{
    public override void enter(PlayerController player)
    {
        player.rb.velocity = Vector2.zero;
        player.allow_rotation = false;
        player.attack();
    }

    public override void fixed_update(PlayerController player)
    {
        if (player.pressed_keys.Count == 0)
        {
            player.rb.velocity = Vector2.zero;
        }
        else
        {
            Vector2 movement_vector = player.vector_map[player.facing_direction];
            player.rb.velocity = movement_vector.normalized * player.speed * 1 / 12f;
        }
    }

    public override void exit(PlayerController player)
    {
        player.allow_rotation = true;
    }
}
