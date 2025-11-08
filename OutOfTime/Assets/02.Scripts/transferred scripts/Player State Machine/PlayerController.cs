using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    public enum DIRECTION
    {
        NORTH, SOUTH, WEST, EAST, NW, NE, SW, SE
    }

    public Animator animator;


    public Rigidbody2D rb;
    public float speed;
    public HealthManager health_manager;


    public List<KeyCode> pressed_keys = new List<KeyCode>();
    public DIRECTION facing_direction;
    public bool allow_rotation = true;


    // some maps from keys to directions to vectors
    private readonly Dictionary<KeyCode, DIRECTION> direction_map = new Dictionary<KeyCode, DIRECTION>()
    {
        { KeyCode.W, DIRECTION.NORTH },
        { KeyCode.S, DIRECTION.SOUTH },
        { KeyCode.A, DIRECTION.WEST },
        { KeyCode.D, DIRECTION.EAST },
    };
    private readonly Dictionary<(DIRECTION, DIRECTION), DIRECTION> diagonal_direction_map = new Dictionary<(DIRECTION, DIRECTION), DIRECTION>()
    {
        // combinations starting with N/S
        { (DIRECTION.NORTH, DIRECTION.WEST), DIRECTION.NW },
        { (DIRECTION.NORTH, DIRECTION.EAST), DIRECTION.NE },
        { (DIRECTION.SOUTH, DIRECTION.WEST), DIRECTION.SW },
        { (DIRECTION.SOUTH, DIRECTION.EAST), DIRECTION.SE },

        // combinations starting with E/W
        { (DIRECTION.EAST, DIRECTION.SOUTH), DIRECTION.SE },
        { (DIRECTION.EAST, DIRECTION.NORTH), DIRECTION.NE },
        { (DIRECTION.WEST, DIRECTION.SOUTH), DIRECTION.SW },
        { (DIRECTION.WEST, DIRECTION.NORTH), DIRECTION.NW },
    };

    public readonly Dictionary<DIRECTION, Vector2> vector_map = new Dictionary<DIRECTION, Vector2>()
    {
        { DIRECTION.NORTH, Vector2.up },
        { DIRECTION.SOUTH, Vector2.down },
        { DIRECTION.WEST, Vector2.left },
        { DIRECTION.EAST, Vector2.right },
        { DIRECTION.NW, new Vector2(-1, 1) },
        { DIRECTION.NE, new Vector2(1, 1) },
        { DIRECTION.SW, new Vector2(-1, -1) },
        { DIRECTION.SE, new Vector2(1, -1) },
    };

    public static PlayerController instance { get; private set; }
    [SerializeField] private AudioClip hurt_sound;


    // state machine
    private AbstractPlayerState current_state;
    public readonly AbstractPlayerState idle_state = new PlayerIdle();
    public readonly AbstractPlayerState move_state = new PlayerMove();
    public readonly AbstractPlayerState attack_state = new PlayerAttack();
    public AbstractPlayerState parry_state = new PlayerParry();
    public AbstractPlayerState teleport_state = new PlayerTeleport();


    // some serialized prefabs
    [SerializeField] private GameObject afterimage_prefab;
    [SerializeField] private GameObject attack_shockwave_prefab;
    [SerializeField] private GameObject charged_shockwave_prefab;       // much larger and more powerful, after teleport
    [SerializeField] private GameObject powerful_shockwave_prefab;      // a bit more powerful, after parry


    // attack types
    private enum ATTACK_TYPE
    {
        BASIC, POWERFUL, CHARGED
    }

    private ATTACK_TYPE next_attack;





    private void Awake()
    {
        instance = this;
        health_manager.onDamage += onDamage;
        health_manager.onDeath += onDeath;
        transitionState(idle_state);
    }

    private void OnDisable()
    {
        health_manager.onDamage -= onDamage;
        health_manager.onDeath -= onDeath;
    }

    public void transitionState(AbstractPlayerState new_state)
    {
        if (current_state != null)
            current_state.exit(this);
        current_state = new_state;
        Debug.Log("Transitioned to state: " + current_state.GetType().ToString());
        current_state.enter(this);

        TextMeshPro tmp = GetComponentInChildren<TextMeshPro>();
        tmp.text = new_state.GetType().ToString();
    }



    private void Update()
    {
        foreach (KeyCode key in direction_map.Keys)
        {
            if (Input.GetKeyDown(key)) pressed_keys.Add(key);
            if (Input.GetKeyUp(key))
            {
                if (pressed_keys.Contains(key))
                    pressed_keys.Remove(key);
            }
        }
        current_state.update(this);
    }



    private void FixedUpdate()
    {
        current_state.fixed_update(this);
        if (allow_rotation)
            parseUserInput();
    }

    private void parseUserInput()
    {
        if (pressed_keys.Count >= 2)
        {
            (DIRECTION, DIRECTION) dir_pair = (direction_map[pressed_keys[pressed_keys.Count - 1]], direction_map[pressed_keys[pressed_keys.Count - 2]]);

            if (diagonal_direction_map.ContainsKey(dir_pair))
            {
                DIRECTION diag_dir = diagonal_direction_map[dir_pair];
                facing_direction = diag_dir;
            }
            else
            {
                Debug.Log(String.Format("cannot map key pair {0} to diagonal direction", dir_pair));
            }
        }
        else if (pressed_keys.Count == 1)
        {
            facing_direction = direction_map[pressed_keys[pressed_keys.Count - 1]];
        }
    }









    private void onDamage()
    {
        SoundManager.instance.playClip(hurt_sound);
    }

    private void onDeath()
    {
    }


    public void teleport()
    {
        Instantiate(afterimage_prefab, transform.position, transform.rotation);
        Vector2 movement_vector = vector_map[facing_direction];
        float teleport_distance = 3.0f;
        Vector2 new_position = rb.position + movement_vector.normalized * teleport_distance;
        rb.MovePosition(new_position);
    }

    public void attack()
    {
        switch (next_attack)
        {
            case ATTACK_TYPE.BASIC:
                StartCoroutine(attackHelperCoroutine("attack " + facing_direction.ToString().ToLower(), attack_shockwave_prefab));
                break;
            case ATTACK_TYPE.POWERFUL:
                StartCoroutine(attackHelperCoroutine("powerful attack " + facing_direction.ToString().ToLower(), powerful_shockwave_prefab));
                break;
            case ATTACK_TYPE.CHARGED:
                StartCoroutine(attackHelperCoroutine("charged attack " + facing_direction.ToString().ToLower(), charged_shockwave_prefab));
                break;
        }
        next_attack = ATTACK_TYPE.BASIC;    // consume stored attack
    }

    public void parry()
    {
        next_attack = ATTACK_TYPE.POWERFUL;
    }

    private IEnumerator attackHelperCoroutine(string animation_name, GameObject shockwave_prefab)
    {
        animator.Play(animation_name);
        yield return new WaitForSeconds(0.2f);  // wait for animation to reach the attack frame


        Vector3 offset = vector_map[facing_direction].normalized * 0.5f;
        GameObject shockwave = Instantiate(shockwave_prefab, transform.position + offset, transform.rotation);
        transitionState(idle_state);


        yield return new WaitForSeconds(0.5f);
        Destroy(shockwave);
    }
}
