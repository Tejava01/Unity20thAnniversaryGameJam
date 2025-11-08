using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Animator animator;

    private bool movementLocked = false;


    private PlayerController.DIRECTION facingDirection = PlayerController.DIRECTION.SOUTH;




    public void move(Vector2 direction, bool stationary)
    {
        if (movementLocked) return;


        facingDirection = mapVector(direction);

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


        facingDirection = mapVector(newDirection);

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



    private PlayerController.DIRECTION mapVector(Vector2 direction)
    {
        PlayerController.DIRECTION dir;
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            if (direction.y > 0) dir = PlayerController.DIRECTION.NORTH;
            else dir = PlayerController.DIRECTION.SOUTH;
        }
        else
        {
            if (direction.x > 0) dir = PlayerController.DIRECTION.EAST;
            else dir = PlayerController.DIRECTION.WEST;
        }
        return dir;
    }

    private void Update()
    {
        animate();
    }

    public IEnumerator applyKnockback(Vector2 force)
    {
        if (movementLocked) yield break;
        if (force.magnitude < 0.1) yield break;

        movementLocked = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(force * rb.mass, ForceMode2D.Impulse);
        facingDirection = mapVector(-force);

        while (rb.velocity.magnitude > 0.5f)
        {
            yield return null;
        }
        movementLocked = false;
    }

    private void animate()
    {
        switch (facingDirection)
        {
            case PlayerController.DIRECTION.NORTH:
                animator.Play("slime up");
                break;
            case PlayerController.DIRECTION.SOUTH:
                animator.Play("slime down");
                break;
            case PlayerController.DIRECTION.WEST:
                animator.Play("slime left");
                break;
            case PlayerController.DIRECTION.EAST:
                animator.Play("slime right");
                break;
        }
    }
}
