using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : AbstractPlayerState
{
    private float parry_duration = 2 / 3f;
    private float parry_downtime = 2 / 3f;



    public override void enter(PlayerController player)
    {
        if (!player.parry_ready)
        {
            player.transitionState(player.idle_state);
            return;
        }
        player.StartCoroutine(player.parryHelperCoroutine(parry_duration, parry_downtime));
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
            player.rb.velocity = movement_vector.normalized * player.speed * 1 / 6f;
        }
    }
}
