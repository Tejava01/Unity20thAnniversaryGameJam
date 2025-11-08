using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : AbstractPlayerState
{
    public override void enter(PlayerController player)
    {
        player.rb.velocity = Vector2.zero;


        switch (player.facing_direction)
        {
            case PlayerController.DIRECTION.NORTH:
                player.animator.Play("idle north");
                break;
            case PlayerController.DIRECTION.SOUTH:
                player.animator.Play("idle south");
                break;
            case PlayerController.DIRECTION.WEST:
                player.animator.Play("idle west");
                break;
            case PlayerController.DIRECTION.EAST:
                player.animator.Play("idle east");
                break;
            case PlayerController.DIRECTION.NW:
                player.animator.Play("idle northwest");
                break;
            case PlayerController.DIRECTION.NE:
                player.animator.Play("idle northeast");
                break;
            case PlayerController.DIRECTION.SW:
                player.animator.Play("idle southwest");
                break;
            case PlayerController.DIRECTION.SE:
                player.animator.Play("idle southeast");
                break;
        }
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
