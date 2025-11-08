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
        switch (player.facing_direction)
        {
            case PlayerController.DIRECTION.NORTH:
                player.animator.Play("walk north");
                break;
            case PlayerController.DIRECTION.SOUTH:
                player.animator.Play("walk south");
                break;
            case PlayerController.DIRECTION.WEST:
                player.animator.Play("walk west");
                break;
            case PlayerController.DIRECTION.EAST:
                player.animator.Play("walk east");
                break;
            case PlayerController.DIRECTION.NW:
                player.animator.Play("walk northwest");
                break;
            case PlayerController.DIRECTION.NE:
                player.animator.Play("walk northeast");
                break;
            case PlayerController.DIRECTION.SW:
                player.animator.Play("walk southwest");
                break;
            case PlayerController.DIRECTION.SE:
                player.animator.Play("walk southeast");
                break;
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
