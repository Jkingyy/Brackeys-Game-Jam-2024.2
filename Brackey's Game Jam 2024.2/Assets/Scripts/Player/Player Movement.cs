using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;



    // Start is called before the first frame update
    private void Awake()
    {
        // Get the components attached to the GameObject
        rb = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        staminaBarParent = _staminaBar.transform.parent.gameObject;
        currentStamina = _maxFlyingStamina;
    }
    // Update is called once per frame
    private void Update()
    {
        // Handle player movement functions 
        Grounded();
        //Animations();


        if (isDashing ) return;

        WallJumping();
        if (isWallJumping) return;
        Jumping();
        Walking();
        Dashing();
        Flying();
    }


    #region Ground Check

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundMask; // The layer mask for ground objects
    [SerializeField] private Transform _groundCheck; // The transform representing the position to check for ground
    [SerializeField] private float _groundCheckRadius = 0.8f; // The radius for ground check
    [SerializeField] private bool isGrounded; // Flag indicating if the player is grounded

    // Check if the player is grounded
    private void Grounded()
    {
        isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundMask);
        if (isGrounded)
        {
            isGrounded = true;
            jumpsRemaining = numberOfJumps;
        }
    }

    #endregion

    #region Walking

    [Header("Movement Speeds")]
    [SerializeField] private float _moveSpeed = 10; // Speed of regular walking
    [SerializeField] private float _sprintSpeed = 13; // Speed of sprinting

    private float horizontal; // Horizontal input from player

    private bool facingRight = true; // Flag indicating if the player is facing right

    // Handle player walking
    private void Walking()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

    
        // Adjust velocity based on sprint input
        if (Input.GetButton("Sprint"))
        {
            rb.velocity = new Vector2(horizontal * _sprintSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontal * _moveSpeed, rb.velocity.y);
        }

        // Flip the player sprite if necessary
        if (horizontal > 0 && !facingRight)
        {
            FlipSprite();
        }
        else if (horizontal < 0 && facingRight)
        {
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    #endregion

    #region Jumping


    [Header("Jump Settings")]
    [SerializeField] private bool _playerCanDoubleJump = true; // Flag indicating if the player can jump
    [SerializeField] private float _coyoteTime = 0.2f; // Duration of grace period for jumping after leaving ground
    [SerializeField] private float _jumpBufferTime = 0.1f; // Duration of buffer time for jumping
    [SerializeField] private float _jumpSpeed = 12; // Height of the jump
    [SerializeField] private float _fallSpeed = 7; // Speed of falling
    [SerializeField] private float _jumpVelocityFalloff = 8; // Rate of decrease in jump velocity


    private int numberOfJumps = 1; // Number of jumps the player can perform
    private int jumpsRemaining; // Number of jumps remaining
    private float jumpBufferCounter; // Counter for jump buffer time
    private float coyoteTimeCounter; // Counter for coyote time

    private bool hasJumped; // Flag indicating if the player has initiated a jump for animations
    private bool hasLanded; // Flag indicating if the player has initiated a jump for animations

    // Handle player jumping
    private void Jumping()
    {

        // Reset coyote time counter if the player is grounded
        if (isGrounded)
        {
            coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Coyote time jump (first jump off the ground)
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && hasJumped == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpSpeed);

            hasJumped = true;
            hasLanded = false;
        }

        if (isGrounded && rb.velocity.y < 0)
        {
            hasJumped = false;
            if (!hasLanded)
            {
                hasLanded = true;
            }
        }


        if (Input.GetButtonUp("Jump"))
        {
            coyoteTimeCounter = 0; // Reset coyote time counter if the player releases the jump button to stop accidental double jumps
        }


        else
        {
            // Apply gravity and falloff to jump velocity
            if (rb.velocity.y < _jumpVelocityFalloff || rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * (_fallSpeed * Physics.gravity.y * Time.deltaTime); //VARIABLE JUMP HEIGHT - gravity is applied to the jump velocity

            }
        }



        if (!_playerCanDoubleJump) return;

        // Mid air jumps - if the player is not grounded and has jumps remaining they can jump again
        if (Input.GetButtonDown("Jump") && coyoteTimeCounter < 0 && jumpsRemaining > 0 && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, _jumpSpeed);
            jumpsRemaining--;
        }

    }

    #endregion

    #region Wall Jumping
    [Header("General Wall Settings")]
    [SerializeField] private bool _playerCanWallJump = true; // Flag indicating if the player can wall jump
    [SerializeField] private LayerMask _wallMask; // The layer mask for wall objects
    [SerializeField] private Transform _wallCheck; // The transform representing the position to check for walls
    private bool isTouchingWall; // Flag indicating if the player is touching a wall

    [Header("Wall Slide Settings")]
    [SerializeField] private float _wallSlideSpeed = 2f; // Speed of sliding down a wall
    private bool isWallSliding; // Flag indicating if the player is sliding down a wall

    [Header("Wall Jump Settings")]
    [SerializeField] private float _wallJumpingTime = 0.2f;
    [SerializeField] private float _wallJumpingDuration = 0.2f;
    [SerializeField] private Vector2 _wallJumpingPower = new Vector2(8f, 16f);

    private bool isWallJumping; // Flag indicating if the player is wall jumping
    private float wallJumpingDir;
    private float wallJumpingCounter;

    private void WallJumping()
    {
        if (!_playerCanWallJump) return;

        isTouchingWall = Physics2D.OverlapCircle(_wallCheck.position, _groundCheckRadius, _wallMask);


        WallSlide();

        WallJump();
    }

    private void WallSlide()
    {
        if(canFly) return;
        // If the player is touching a wall and not grounded and moving horizontally into the wall, slide down the wall
        if (isTouchingWall && !isGrounded && horizontal != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -_wallSlideSpeed, float.MaxValue));
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;

            wallJumpingDir = -transform.localScale.x;
            wallJumpingCounter = _wallJumpingTime; // gives a small buffer time to jump off the wall

            CancelInvoke("StopWallJumping");
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDir * _wallJumpingPower.x, _wallJumpingPower.y);
            wallJumpingCounter = 0;
            if (transform.localScale.x != wallJumpingDir)
            {
                FlipSprite();
            }

            Invoke("StopWallJumping", _wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    #endregion

    #region Dashing

    [Header("Dash Settings")]
    [SerializeField] private bool _playerHasDashAbility = true; // Flag indicating if the player can dash
    [SerializeField] private float _dashPower = 24f; // Power/speed of the dash
    [SerializeField] private float _dashDuration = 0.2f; // How long the dash lasts
    [SerializeField] private float _dashCooldown = 1f; // Cooldown time between dashes

    private bool canDash = true; // Flag indicating if the player can dash
    public bool isDashing = false; // Flag indicating if the player is in the middle of dashing

    private void Dashing()
    {
        if (!_playerHasDashAbility) return;

        // If the player presses the dash button and can dash, start the dash coroutine
        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {

        canDash = false; // Prevent the player from dashing again until the cooldown is over - is set to true in the grounded function
        isDashing = true; // Sets flag that the player is currently dashing
        float originalGravity = rb.gravityScale; // Store the original gravity scale of the player
        rb.gravityScale = 0; // Set gravity to 0 so the player doesn't fall during the dash
        rb.velocity = new Vector2(transform.localScale.x * _dashPower, 0f); // Apply the dash power to the player


        yield return new WaitForSeconds(_dashDuration); // Wait for the dash duration to end

        rb.gravityScale = originalGravity; // Reset the gravity scale
        isDashing = false;

        yield return new WaitForSeconds(_dashCooldown); // Wait for the dash cooldown to end
        canDash = true; // Allow the player to dash again
    }
    #endregion

    #region Flying
    [Header("Flying Settings")]

    [SerializeField] float _flyingSpeed = 14;
    [SerializeField] float _maxFlyingStamina = 100;
    [SerializeField] float _staminaDrainPerSecond = 20;
    [SerializeField] float _staminaRecoveryPerSecond = 40;
    [SerializeField] Image _staminaBar; 

    float currentStamina;
    private GameObject staminaBarParent;
    public bool canFly;

    void Flying(){
        if(isGrounded){ // if grounded then restore stamina until jump
            RestoreStamina();
            return;
        }
        if(!canFly) return; // if cant fly eg. no stamina/player is in the storm return
        if(!Input.GetButton("Jump")) return; // if not holding the fly button return
        if(currentStamina <= 0) {
            canFly = false;
            return; // player is out of stamina
        }
        
        
        rb.velocity = new Vector2(rb.velocity.x, _flyingSpeed); //add velocity for flying

        TurnOnStaminaBar(); // make stamina bar visible
        
        float drainAmount = _staminaDrainPerSecond/(1/Time.deltaTime);

        currentStamina -= drainAmount;
        
        UpdateStaminaBar();

    } 

    void RestoreStamina(){
            if(currentStamina == _maxFlyingStamina) {
                canFly = true;
                return;
            }


            float restoreAmount = _staminaRecoveryPerSecond/(1/Time.deltaTime);

            currentStamina += restoreAmount;


            if(currentStamina > _maxFlyingStamina){
                currentStamina = _maxFlyingStamina;
                Invoke("TurnOffStaminaBar",1); //turns stamina bar off after 1 second
            } 

            UpdateStaminaBar();
    }
    void UpdateStaminaBar(){
        _staminaBar.fillAmount = currentStamina / _maxFlyingStamina;    
    }

    void TurnOffStaminaBar() {
        if(staminaBarParent.activeSelf == false) return;

        staminaBarParent.gameObject.SetActive(false);
    }
    void TurnOnStaminaBar() {
        if(staminaBarParent.activeSelf == true) return;

        staminaBarParent.gameObject.SetActive(true);
    }
    #endregion
}