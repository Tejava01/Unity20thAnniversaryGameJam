using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    private bool movementLocked = false;




    public void move(Vector2 direction, bool stationary)
    {
        //Debug.Log("movement locked? " + movementLocked + ", stationary? " + stationary);
        if (movementLocked) return;
        if (stationary)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // resolve blockage by trying to go only one direction
        if (blocked(direction))
        {
            StartCoroutine(resolveBlock(direction));
            return;
        }


        rb.velocity = direction.normalized * speed;
    }

    private IEnumerator resolveBlock(Vector2 desired)
    {
        movementLocked = true;



        Vector2 newDirection;
        Vector2 desiredX = new Vector2(desired.x, 0);
        Vector2 desiredY = new Vector2(0, desired.y);

        if (blocked(desiredX))
        {
            if (blocked(desiredY)) newDirection = -desiredX;
            else newDirection = desiredY;
        }
        else if (blocked(desiredY))
            newDirection = desiredX;
        else
        {           // if neither are blocked, choose one that gets closer to player
            if (desiredX.magnitude > desiredY.magnitude) newDirection = desiredX;
            else newDirection = desiredY;
        }

        rb.velocity = newDirection.normalized * speed;
        rb.drag = 0;

        yield return new WaitForSeconds(1);

        rb.drag = 10;

        movementLocked = false;

    }



    private bool blocked(Vector2 direction)
    {
        float checkRadius = 0.2f; // depends on your enemy collider size
        Vector2 targetPos = (Vector2)transform.position + direction.normalized * 0.5f;
        return Physics2D.OverlapCircle(targetPos, checkRadius, LayerMask.GetMask("Environment"));
    }



  
    


    public IEnumerator applyKnockback(Vector2 force)
    {
        if (movementLocked) yield break;
        if (force.magnitude < 0.1) yield break;

        movementLocked = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(force * rb.mass, ForceMode2D.Impulse);
        while (rb.velocity.magnitude > 0.5f)
        {
            yield return null;
        }
        movementLocked = false;
    }

}
