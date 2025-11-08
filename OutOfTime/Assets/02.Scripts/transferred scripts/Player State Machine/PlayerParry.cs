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
    }

    
}
