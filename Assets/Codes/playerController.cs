using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows.Speech;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D), typeof(touchingDrirections), typeof(damageAble))]
public class playerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpImpulse = 10f;
    Vector2 moveInput;
    touchingDrirections touchingDrirections;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> voiceActions = new Dictionary<string, Action>();
    damageAble damageable;

    public float CurrentMoveSpeed {  get 
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDrirections.IsOnWall)
                {
                    if(touchingDrirections.IsGrounded)
                    {
                        return moveSpeed;
                    }
                    else
                    {
                        return jumpImpulse;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(animationStrings.isMoving, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight { get
        {
            return _isFacingRight;
        }private set { 
            if (_isFacingRight != value)
            {
                //flip local scale to make the player opposite direction
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove { get
        {
            return animator.GetBool(animationStrings.canMove);
        } }

    private bool IsAlive
    {
        get
        {
            return animator.GetBool(animationStrings.isAlive);
        }
    }

    

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDrirections = GetComponent<touchingDrirections>();
        damageable = GetComponent<damageAble>();

        // Initialize voice commands
        voiceActions.Add("right", MoveForward);
        voiceActions.Add("left", MoveBack);
        voiceActions.Add("jump", PerformJump);
        voiceActions.Add("slash", PerformAttack);
        voiceActions.Add("stop", StopMovement);

        // Start keyword recognizer
        keywordRecognizer = new KeywordRecognizer(voiceActions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        string spokenPhrase = speech.text.ToLower(); // Convert to lowercase for matching

        if (voiceActions.ContainsKey(spokenPhrase))
        {
            voiceActions[spokenPhrase].Invoke();
        }
    }

    // Voice command actions
    private void MoveForward()
    {
        moveInput = new Vector2(1, 0); // Set moveInput to move right
        IsMoving = true; // Set movement flag to true

        setDirection(moveInput); // Update facing direction

        // If the player can move and is not on a wall, move in the forward direction
        if (CanMove && !touchingDrirections.IsOnWall)
        {
            // Update the velocity in FixedUpdate
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }

    }

    private void MoveBack()
    {
        moveInput = new Vector2(-1, 0); // Set moveInput to move left
        IsMoving = true; // Set movement flag to true

        setDirection(moveInput); // Update facing direction

        // If the player can move and is not on a wall, move in the backward direction
        if (CanMove && !touchingDrirections.IsOnWall)
        {
            // Update the velocity in FixedUpdate
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }
    }

    private void StopMovement()
    {
        moveInput = new Vector2(0, 0);

        IsMoving = false; // Set the movement flag to false
    }

    private void PerformJump()
    {
        // Implement jump logic here
        if (touchingDrirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(animationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    private void PerformAttack()
    {
        // Implement jump logic here
        if (touchingDrirections.IsGrounded)
        {
            animator.SetTrigger(animationStrings.attack);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(!damageable.LockVelocity)
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

        animator.SetFloat(animationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            setDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }

        
    }

    private void setDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight)
        {
            //face the right
            IsFacingRight = true;
        }else if(moveInput.x < 0 && IsFacingRight)
        {
            //face the left
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Check if alive as well
        if(context.started && touchingDrirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(animationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnAttack (InputAction.CallbackContext context)
    {
        if (context.started && touchingDrirections.IsGrounded)
        {
            animator.SetTrigger(animationStrings.attack);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
