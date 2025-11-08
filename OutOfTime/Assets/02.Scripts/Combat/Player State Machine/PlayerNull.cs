using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNull : AbstractPlayerState
{
    // dummy state that replaces abilities the player does not have. it immediately transitions to idle.
    
    public override void enter(PlayerController player)
    {
        player.transitionState(player.idle_state);
    }
}
