using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Platformer.Character.Controller2D
{
    public class CharacterController2D : MonoBehaviour
    {
        private static readonly int INT_STATE = Animator.StringToHash("State");
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float jumpForce = 1.0f;
        private float prevGravity;

        private Rigidbody2D rigidBody;
        private Animator charAnimator;
        private SpriteRenderer charRenderer;

        [SerializeField] private SceneManager nextScene;

        private List<Collider2D> colliders2D;
        private ContactFilter2D filter2D;
        [SerializeField] private float filterMinAngle = 89.0f;
        [SerializeField] private float filterMaxAngle = 91.0f;

        [SerializeField] private float wallJumpMultiplier = 2.0f;
        [SerializeField] private float wallJumpTimerRecharger = 0.2f;
        private float wallJumpTimer = 0.0f;

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

        private bool isTouchingWalls
        {
            get
            {
                int countR;
                int countL;
                WallTouchFiltering(out countR, out countL);

                return (countR > 0 || countL > 0);
            }
        }

        private bool isGrabbingWall { get; set; }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            charAnimator = GetComponent<Animator>();
            charRenderer = GetComponent<SpriteRenderer>();

            colliders2D = new List<Collider2D>();
            filter2D.SetNormalAngle(filterMinAngle, filterMaxAngle);
            filter2D.useNormalAngle = true;
            prevGravity = rigidBody.gravityScale;
        }

        private void OnEnable()
        {
            charAnimator.GetBehaviour<CharacterAnimationCallback>().DyingAction = OnDying;
            charAnimator.GetBehaviour<WinAnimationCallback>().WinAction = OnWinning;
        }

        void FixedUpdate()
        {
            if (!State.Equals(CharacterAnimationState.Die))
            {
                if (!State.Equals(CharacterAnimationState.Win))
                {
                    if (wallJumpTimer <= 0)
                    {
                        CharacterMovement();
                    }
                    else
                    {
                        wallJumpTimer -= Time.fixedDeltaTime;
                    }
                }

            }
            //Debug.Log(State);
        }

        private void WallTouchFiltering(out int countR, out int countL)
        {
            List<Collider2D> colliders = new List<Collider2D>();

            ContactFilter2D right = default;
            right.SetNormalAngle(-1.0f, 1.0f);
            right.useNormalAngle = true;
            countR = rigidBody.GetContacts(right, colliders);

            ContactFilter2D left = default;
            left.SetNormalAngle(179.0f, 181.0f);
            left.useNormalAngle = true;
            colliders = new List<Collider2D>();
            countL = rigidBody.GetContacts(left, colliders);
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
                }
            }


            ChangeAnimationStateOnMovement(velocity.x);
            rigidBody.velocity = velocity;
            StickToWall();
        }

        private void StickToWall()
        {
            isGrabbingWall = false;
            if (isTouchingWalls && !IsGrounded)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    isGrabbingWall = true;
                }
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    isGrabbingWall = true;
                }

                rigidBody.gravityScale = 0.0f;
            }

            if (isGrabbingWall)
            {
                rigidBody.velocity = Vector2.zero;

                if (Input.GetButtonDown("Jump"))
                {
                    wallJumpTimer = wallJumpTimerRecharger;

                    rigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * speed * Vector3.right.x, jumpForce * Vector2.up.y);
                    rigidBody.velocity *= wallJumpMultiplier;
                    rigidBody.gravityScale = prevGravity;
                    isGrabbingWall = false;
                }
            }
            else
            {
                rigidBody.gravityScale = prevGravity;
            }

        }

        private void ChangeAnimationStateOnMovement(float horizontalVelocity)
        {
            if (IsGrounded)
            {
                if (!State.Equals(CharacterAnimationState.Jump))
                {
                    State = CharacterAnimationState.Jump;
                }
                if (horizontalVelocity.Equals(0.0f))
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
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.tag.Equals("Obstacle"))
            {
                State = CharacterAnimationState.Die;
            }
            if (collision.transform.tag.Equals("Enemy"))
            {
                State = CharacterAnimationState.Die;
            }
            if (collision.transform.tag.Equals("Collectable"))
            {
                State = CharacterAnimationState.Win;
            }
        }

        internal void OnDying()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        internal void OnWinning()
        {
            if (SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}