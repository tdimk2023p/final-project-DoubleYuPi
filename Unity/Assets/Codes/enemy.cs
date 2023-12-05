using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(touchingDrirections), typeof(damageAble))]
public class enemy : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public detectionZone attackZone;

    Rigidbody2D rb;
    touchingDrirections touchingDrirections;
    Animator animator;
    damageAble damageable;

    public enum WalkableDirection { Left, Right }

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    
    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set { 
            if(_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                
                if(value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }else if(value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            
            
            _walkDirection = value; }
    }
    public bool _hasTarget = false;

    public bool HasTarget { get { return _hasTarget; } private set {
            _hasTarget = value;
            animator.SetBool(animationStrings.hasTarget, value);
        } 
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(animationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDrirections = GetComponent<touchingDrirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<damageAble>();
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (touchingDrirections.IsOnWall && touchingDrirections.IsGrounded)
        {
            FlipDirection();
        }
        if (!damageable.LockVelocity)
        {
            if (CanMove)
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        if(WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }else if(WalkDirection == WalkableDirection.Left){
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Current walkable direction is not set up to left or right");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
