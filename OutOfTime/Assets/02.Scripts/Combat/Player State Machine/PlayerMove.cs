using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : AbstractPlayerState
{
    public override void enter(PlayerController player)
    {
    }

    public override void fixed_update(PlayerController player)
    {
        Vector2 movement_vector = player.vector_map[player.facing_direction];
        player.rb.velocity = movement_vector.normalized * player.speed;
        if (player.pressed_keys.Count == 0)
        {
            player.transitionState(player.idle_state);
        }
    }


    public override void update(PlayerController player)
    {
        player.animator.Play("walk " + player.facing_direction.ToString().ToLower());


        if (Input.GetKeyDown(KeyCode.J))
        {
            player.transitionState(player.attack_state);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            player.transitionState(player.parry_state);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            player.transitionState(player.teleport_state);
        }
    }
}
