using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractEnemyState<T> where T : AbstractEnemyController<T>
{
    public abstract void enter(T controller);
    public virtual void exit(T controller) { }
    public virtual void update(T controller) { }
    public virtual void fixedUpdate(T controller) { }
}