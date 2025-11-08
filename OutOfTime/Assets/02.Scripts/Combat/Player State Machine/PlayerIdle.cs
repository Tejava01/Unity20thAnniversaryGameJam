using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : AbstractPlayerState
{
    public override void enter(PlayerController player)
    {
        player.rb.velocity = Vector2.zero;

        Debug.Log("entering idle, setting animation to idle " + player.facing_direction.ToString().ToLower());
        player.animator.Play("idle " + player.facing_direction.ToString().ToLower());
    }

    public override void update(PlayerController player)
    {
        if (player.pressed_keys.Count > 0)
        {
            player.transitionState(player.move_state);
        }

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
