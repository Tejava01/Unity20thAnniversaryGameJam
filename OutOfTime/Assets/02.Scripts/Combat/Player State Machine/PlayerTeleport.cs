using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : AbstractPlayerState
{

    public override void enter(PlayerController player)
    {
        player.teleport();
        player.transitionState(player.idle_state);
    }

}
