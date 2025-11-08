using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : AbstractPlayerState
{
    private float parry_duration = 2 / 3f;
    private float parry_timer = 0f;



    public override void enter(PlayerController player)
    {
        parry_timer = 0;
        player.StartCoroutine(player.parryHelperCoroutine(parry_duration));
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
            player.rb.velocity = movement_vector.normalized * player.speed * 1 / 3f;
        }
    }
    

    public override void update(PlayerController player)
    {
        parry_timer += Time.deltaTime;
        if (parry_timer >= parry_duration)
        {
            player.transitionState(player.idle_state);
        }

        player.animator.Play("walk " + player.facing_direction.ToString().ToLower());
    }

    
}
