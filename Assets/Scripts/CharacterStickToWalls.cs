using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStickToWalls : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private List<Collider2D> colliders2D;
    private ContactFilter2D filter2DRight;
    private ContactFilter2D filter2DLeft;
    private bool stick = false;
    private bool isTouchingRightWall
    {
        get
        {
            int count = rigidBody.GetContacts(filter2DRight, colliders2D);

            return count > 0;
        }
    }

    private bool isTouchingLeftWall
    {
        get
        {
            int count = rigidBody.GetContacts(filter2DLeft, colliders2D);
            return count > 0;
        }
    }

    internal bool isStickToWall
    {
        get
        {
            return isTouchingLeftWall || isTouchingRightWall;
        }
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        colliders2D = new List<Collider2D>();
        filter2DRight.SetNormalAngle(-1.0f, 1.0f);
        filter2DRight.useNormalAngle = true;
        filter2DLeft.SetNormalAngle(179.0f, 181.0f);
        filter2DLeft.useNormalAngle = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!GetComponent<CharacterController2D>().IsGrounded)
        {
            Debug.Log(isStickToWall);
        }
    }
}
