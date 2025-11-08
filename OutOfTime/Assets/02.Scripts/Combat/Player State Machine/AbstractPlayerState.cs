using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayerState
{
    public abstract void enter(PlayerController player);
    public virtual void exit(PlayerController player){ }
    public virtual void update(PlayerController player){ }
    public virtual void fixed_update(PlayerController player){ }
}
