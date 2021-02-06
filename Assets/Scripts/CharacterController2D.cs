using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    private static readonly int INT_STATE = Animator.StringToHash("State");
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 1.0f;

    private Rigidbody2D rigidBody;
    private Animator charAnimator;
    private SpriteRenderer charRenderer;

    private List<Collider2D> colliders2D;
    private ContactFilter2D filter2D;
    [SerializeField] private float filterMinAngle = 89.0f;
    [SerializeField] private float filterMaxAngle = 91.0f;

    private CharacterAnimationState state = default;

    internal CharacterAnimationState State
    {
        get => state;
        set
        {
            state = value;

            charAnimator.SetInteger(INT_STATE, (int)state);
        }
    }

    private bool IsGrounded
    {
        get
        {
            bool value = false;
            int count = rigidBody.GetContacts(filter2D, colliders2D);

            value = count > 0;
            return value;
        }
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        charAnimator = GetComponent<Animator>();
        charRenderer = GetComponent<SpriteRenderer>();

        colliders2D = new List<Collider2D>();
        filter2D.SetNormalAngle(filterMinAngle, filterMaxAngle);
        filter2D.useNormalAngle = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CharacterMovement();
    }

    private void CharacterMovement()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float jumpAxis = Input.GetAxis("Jump");

        var velocity = rigidBody.velocity;
        velocity.x = Vector3.right.x * horizontalAxis * speed;

        if (velocity.x > 0)
        {
            charRenderer.flipX = false;
        }
        else if (velocity.x < 0)
        {
            charRenderer.flipX = true;
        }

        if (IsGrounded)
        {
            if (jumpAxis > 0 && !State.Equals(CharacterAnimationState.Jump))
            {
                velocity.y += Vector2.up.y * jumpAxis * jumpForce;
                State = CharacterAnimationState.Jump;
            }
            else if (velocity.x == 0.0f)
            {
                State = CharacterAnimationState.Idle;
            }
            else
            {
                State = CharacterAnimationState.Run;
            }
        }
        else
        {
            State = CharacterAnimationState.Jump;
        }

        rigidBody.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag.Equals("Obstacle"))
        {
            State = CharacterAnimationState.Die;
        }
    }
}
