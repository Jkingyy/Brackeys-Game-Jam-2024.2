using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

	private Rigidbody2D rb;  
  	private Knockback knockback;
	private HealthBarTimeout healthBarTimeout;
	[SerializeField] float maxHorizontalSpeed;
	[SerializeField] bool isRainingIntensely = false;
	[SerializeField] Damage playerDamage;
	public void ToggleRainIntensity()
	{
		isRainingIntensely = !isRainingIntensely;
	}

	// Start is called before the first frame update
	private void Awake()
	{
		// Get the components attached to the GameObject
		rb = GetComponent<Rigidbody2D>();
		knockback = GetComponent<Knockback>();
		healthBarTimeout = GetComponentInChildren<HealthBarTimeout>();
		playerDamage = GetComponent<Damage>();

		UpdatePlayerStats(0);
	}
	private void OnEnable()
	{
		RainEffect.OnRainIntensityLimitReached += ToggleRainIntensity;
		RainEffect.OnRainIntensityDroppedBelowLimit += ToggleRainIntensity;
		Upgrades.OnUpgrade += UpdatePlayerStats;
	}
	
	private void OnDisable()
	{
		RainEffect.OnRainIntensityLimitReached -= ToggleRainIntensity;
		RainEffect.OnRainIntensityDroppedBelowLimit -= ToggleRainIntensity;
		Upgrades.OnUpgrade -= UpdatePlayerStats;
	}
	
	private void Start()
	{
		originalGravityScale = rb.gravityScale;
		currentStamina = _maxFlyingStamina;
		
	}
	// Update is called once per frame
	private void Update()
	{
		rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed), rb.velocity.y);
		// Handle player movement functions 
		Grounded();
		//Animations();

	if (knockback.IsBeingKnockedBack) return;
		if (isDashing) return;

		WallJumping();
		if (isWallJumping) return;
		Jumping();
		Walking();
		Dashing();
		Flying();
		if (!isFlying)
		{
			Destroy(flyingSoundObject);
			flyingSoundObject = null;
		}
		Falling();
		
	}

	private void UpdatePlayerStats(int upgradeCost)
	{
		_jumpSpeed += PlayerPrefs.GetInt("playerJumpSpeed", 0);
		_wallJumpingPower = new Vector2(_jumpSpeed - 4, 16);
		_wallJumpingDuration += (float)PlayerPrefs.GetInt("playerJumpSpeed", 0) / 20;
		
		_moveSpeed += PlayerPrefs.GetInt("playerSpeed", 0);
		_sprintSpeed += PlayerPrefs.GetInt("playerSpeed", 0);
		_flyingSpeed += PlayerPrefs.GetInt("playerSpeed", 0);
		_dashPower += PlayerPrefs.GetInt("playerSpeed", 0);
		
		maxHorizontalSpeed = _dashPower + 5f;
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
	private float vertical; // vertical input from player
	private bool facingRight = true; // Flag indicating if the player is facing right

	// Handle player walking
	private void Walking()
	{
		horizontal = Input.GetAxisRaw("Horizontal");
		vertical = Input.GetAxisRaw("Vertical");

		if (isGrounded && horizontal == 0 && !hasJumped && !isHurt && !(playerDamage.GetInvincibilityTimeCounter() > 0))
		{
			SetPlayerState(PlayerState.Idle);
		}
		
		// Adjust velocity based on sprint input
		if (Input.GetButton("Sprint"))
		{
			rb.velocity = new Vector2(horizontal * _sprintSpeed, rb.velocity.y);
			if (isGrounded && horizontal != 0) SetPlayerState(PlayerState.Run);
		}
		else
		{
			rb.velocity = new Vector2(horizontal * _moveSpeed, rb.velocity.y);
			if (isGrounded&& horizontal != 0) SetPlayerState(PlayerState.Walk);
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


		//flip health UI
		scale = staminaBarParent.transform.localScale;
		scale.x *= -1;
		staminaBarParent.transform.localScale = scale;
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
	[SerializeField] private bool hasPeaked; // Flag indicating if the player has reached the peak of the jump
	[SerializeField] AudioClip _jumpSoundFX;
  [SerializeField] AudioClip _buzzingFX;



  private int numberOfJumps = 1; // Number of jumps the player can perform
	private int jumpsRemaining; // Number of jumps remaining
	private float jumpBufferCounter; // Counter for jump buffer time
	private float coyoteTimeCounter; // Counter for coyote time

	[SerializeField] private bool hasJumped; // Flag indicating if the player has initiated a jump for animations
	[SerializeField] private bool hasLanded; // Flag indicating if the player has initiated a jump for animations

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
			SoundFXManager.Instance.PlaySoundFXClipAtRandomPitch(_jumpSoundFX, transform, 0.25f,0.25f);
			rb.velocity = new Vector2(rb.velocity.x, _jumpSpeed);
			hasJumped = true;
			hasLanded = false;
		}
		
		if (hasJumped && !hasPeaked && rb.velocity.y <= 0)
		{
			hasPeaked = true;
		}

		if (isGrounded && rb.velocity.y < 0)
		{
			hasJumped = false;
			if (!hasLanded)
			{
				hasLanded = true;
				jumpsRemaining = numberOfJumps;
				_playerCanDoubleJump = true;
			}
			hasPeaked = false;
		}

		if (!isGrounded && hasJumped && !isFalling && !isWallGrabbing && !isWallSliding && !isFlying) SetPlayerState(PlayerState.Jump);

		if (Input.GetButtonUp("Jump"))
		{
			coyoteTimeCounter = 0; // Reset coyote time counter if the player releases the jump button to stop accidental double jumps
		}

		if (!isWallGrabbing)
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
			if (jumpsRemaining == 0) _playerCanDoubleJump = false;
			SetPlayerState(PlayerState.DoubleJump);
		}

	}

	
	
	#endregion

	#region Wall Jumping
	[Header("General Wall Settings")]
	[SerializeField] private bool _playerCanWallJump = true; // Flag indicating if the player can wall jump
	[SerializeField] private LayerMask _wallMask; // The layer mask for wall objects
	[SerializeField] private Transform _wallCheck; // The transform representing the position to check for walls
	private bool isTouchingWall; // Flag indicating if the player is touching a wall

	[Header("Wall Movement Settings")]
	[SerializeField] private float _wallSlideSpeed = 2f; // Speed of sliding down a wall
	private bool isWallSliding; // Flag indicating if the player is sliding down a wall
	private bool isWallClimbing; // Flag indicating if the player is climbing up a wall
	private bool isWallGrabbing; // Flag indicating if the player is holding onto a wall

	[Header("Wall Jump Settings")]
	[SerializeField] private float _wallJumpingTime = 0.2f;
	[SerializeField] private float _wallJumpingDuration = 0.2f;
	[SerializeField] private Vector2 _wallJumpingPower;

	private bool isWallJumping; // Flag indicating if the player is wall jumping
	private float wallJumpingDir;
	private float wallJumpingCounter;

	private void WallJumping()
	{
		if (!_playerCanWallJump) return;

		isTouchingWall = Physics2D.OverlapCircle(_wallCheck.position, _groundCheckRadius, _wallMask);


		if (!isRainingIntensely)
		{ //This swaps the player movement from grabbing and climbing walls to sliding and jumping from them
			WallGrab();
			WallMovement();
		}
		else
		{
			WallSlide();
		}

		WallJump();
	}
	private void WallGrab()
	{

		if (isTouchingWall && !isGrounded && horizontal != 0 && vertical == 0)  // If the player is touching a wall and not grounded and moving horizontally into the wall, hold the wall
		{
			rb.velocity = new Vector2(rb.velocity.x, 0);
			rb.gravityScale = 0;
			isWallGrabbing = true;
			SetPlayerState(PlayerState.WallGrab);
			TurnOnStaminaBar();
		}
		else if (!isTouchingWall || horizontal == 0 || currentStamina <= 0)
		{
			rb.gravityScale = originalGravityScale;
			isWallGrabbing = false;
			isWallClimbing = false;
		}
	}



	private void WallMovement()
	{

		// If the player is touching a wall and not grounded and moving horizontally into the wall, slide down the wall
		if (vertical < 0 && isWallGrabbing)
		{
			rb.velocity = new Vector2(rb.velocity.x, -_wallSlideSpeed);
			DrainStamina();
			isWallClimbing = true;
			SetPlayerState(PlayerState.WallClimb);
		}
		else if (vertical > 0 && isWallGrabbing)
		{
			DrainStamina();
			rb.velocity = new Vector2(rb.velocity.x, _wallSlideSpeed);
			isWallClimbing = true;
			SetPlayerState(PlayerState.WallClimb);
		}
	}

	private void WallSlide()
	{
		// If the player is touching a wall and not grounded and moving horizontally into the wall, slide down the wall
		if (isTouchingWall && !isGrounded && horizontal != 0)
		{
			rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -_wallSlideSpeed, float.MaxValue));
			isWallSliding = true;
			SetPlayerState(PlayerState.WallSlide);
		}
		else
		{
			isWallSliding = false;
		}
	}
	private void WallJump()
	{
		if (isWallGrabbing || isWallSliding)
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
			SetPlayerState(PlayerState.WallJump);
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
	
	public bool GetIsWallJumping()
	{
		return isWallJumping;
	}
	#endregion

	#region Dashing

	[Header("Dash Settings")]
	[SerializeField] private bool _playerHasDashAbility = true; // Flag indicating if the player can dash
	[SerializeField] private float _dashPower = 24f; // Power/speed of the dash
	[SerializeField] private float _dashDuration = 0.2f; // How long the dash lasts
	[SerializeField] private float _dashCooldown = 1f; // Cooldown time between dashes

	private float originalGravityScale;
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
		SetPlayerState(PlayerState.Dash);

		rb.gravityScale = 0; // Set gravity to 0 so the player doesn't fall during the dash
		rb.velocity = new Vector2(transform.localScale.x * _dashPower, 0f); // Apply the dash power to the player
		//ChangeAnimationState(PLAYER_DASH);
		
		yield return new WaitForSeconds(_dashDuration); // Wait for the dash duration to end

		rb.gravityScale = originalGravityScale; // Reset the gravity scale
		isDashing = false;

		yield return new WaitForSeconds(_dashCooldown); // Wait for the dash cooldown to end
		canDash = true; // Allow the player to dash again
	}
	
	public bool GetIsDashing()
	{
		return isDashing;
	}
	#endregion

	#region Flying
	[Header("Flying Settings")]

	[SerializeField] float _flyingSpeed = 14;
	[SerializeField] float _maxFlyingStamina = 100;
	[SerializeField] float _staminaDrainPerSecondFlying = 20;
	[SerializeField] float _staminaRecoveryPerSecond = 40;
	[SerializeField] Image _staminaBar;
	[SerializeField] private GameObject staminaBarParent;
	public float UITimeout = 3;



	float currentStamina;
	public bool canFly;
	[SerializeField] private bool isFlying;
	private GameObject flyingSoundObject;

	void Flying()
	{
		if (isRainingIntensely) return; //MyPing0 - this stops the player flying in the rain


		isFlying = false;

		if (isGrounded)
		{ // if grounded then restore stamina until jump
			RestoreStamina();

			return;
		}
		if (!Input.GetButton("Jump")) return; // if not holding the fly button return
		
		if (currentStamina <= 0 || (hasJumped && !hasPeaked) || isWallClimbing || isWallGrabbing || isWallSliding) // if stamina is empty or the player has not reached the peak of the jump then don't fly
		{
			canFly = false;
		}

		if (hasJumped && hasPeaked && currentStamina > 0 && !isWallClimbing && !isWallGrabbing && !isWallSliding)
		{
			canFly = true;  // Re-enable flying when falling and stamina is available
		}
		
		if (!canFly) return;

		rb.velocity = new Vector2(rb.velocity.x, _flyingSpeed); //add velocity for flying
		isFlying = true;
		SetPlayerState(PlayerState.Fly);
		if (flyingSoundObject == null)
		{
			flyingSoundObject = SoundFXManager.Instance.PlayLoopingSoundFXClip(_buzzingFX, transform, 0.25f);
		}

		TurnOnStaminaBar(); // make stamina bar visible

		DrainStamina();
	}

	void RestoreStamina()
	{
		if (currentStamina == _maxFlyingStamina)
		{
			canFly = true;
			return;
		}


		float restoreAmount = _staminaRecoveryPerSecond / (1 / Time.deltaTime);

		currentStamina += restoreAmount;


		if (currentStamina > _maxFlyingStamina)
		{
			currentStamina = _maxFlyingStamina;
		}

		UpdateStaminaBar();
	}
	void DrainStamina()
	{
		float drainAmount = _staminaDrainPerSecondFlying / (1 / Time.deltaTime);

		currentStamina -= drainAmount;

		UpdateStaminaBar();

	}
	void UpdateStaminaBar()
	{
		_staminaBar.fillAmount = currentStamina / _maxFlyingStamina;
		healthBarTimeout.hasChanged = true;
	}

	public void TurnOffStaminaBar()
	{
		if (staminaBarParent.activeSelf == false) return;

		staminaBarParent.gameObject.SetActive(false);
	}
	public void TurnOnStaminaBar()
	{
		if (staminaBarParent.activeSelf == true) return;

		staminaBarParent.gameObject.SetActive(true);
		healthBarTimeout.hasChanged = true;
	}
	#endregion
	
	#region Falling
	
	[Header("Falling Settings")]
	[SerializeField] private bool isFalling; // Flag indicating if the player is falling
	[SerializeField] private float timeBeforeFall = 0.2f; // Time before the player is considered to be falling
	
	private void Falling()
	{
		if (rb.velocity.y < 0 && !isGrounded && !isWallGrabbing && !isWallJumping && !isWallSliding && !isWallClimbing && !isDashing && !isFlying)
		{
			timeBeforeFall -= Time.deltaTime;
			if (timeBeforeFall <= 0)
			{
				isFalling = true;
				SetPlayerState(PlayerState.Fall);
			}
		}
		else
		{
			isFalling = false;
			timeBeforeFall = 0.2f;
		}
			
	}
	
	#endregion
	

		#region Animation

	//animation states    
	public enum PlayerState
	{
		Idle,
		Walk,
		Run,
		Jump,
		DoubleJump,
		Fall,
		Hurt,
		WallJump,
		WallSlide,
		WallGrab,
		WallClimb,
		Dash,
		Fly
	}

	[SerializeField] PlayerState playerState;

	public void SetPlayerState(PlayerState state)
	{
		playerState = state;
		PlayAnimation(playerState);
	}
	private bool isDead = false;
	public bool isHurt = false;
	[SerializeField] Animator animator;
	private string currentState;
	
	private void PlayAnimation(PlayerState state)
	{
		switch (state)
		{
			case PlayerState.Idle:
				ChangeAnimationState("PlayerIdle");
				break;
			case PlayerState.Walk:
				ChangeAnimationState("PlayerWalk");
				break;
			case PlayerState.Run:
				ChangeAnimationState("PlayerRunning");
				break;
			case PlayerState.Jump:
				ChangeAnimationState("PlayerJump");
				break;
			case PlayerState.DoubleJump:
				ChangeAnimationState("PlayerDoubleJump");
				break;
			case PlayerState.Fall:
				ChangeAnimationState("PlayerFall");
				break;
			case PlayerState.Hurt:
				ChangeAnimationState("TakeDamage");
				break;
			case PlayerState.WallJump:
				ChangeAnimationState("PlayerWallJump");
				break;
			case PlayerState.WallSlide:
				ChangeAnimationState("PlayerWallSlide");
				break;
			case PlayerState.WallGrab:
				ChangeAnimationState("PlayerWallGrab");
				break;
			case PlayerState.WallClimb:
				ChangeAnimationState("PlayerClimb");
				break;
			case PlayerState.Dash:
				ChangeAnimationState("PlayerDash");
				break;
			case PlayerState.Fly:
				ChangeAnimationState("PlayerFly");
				break;
		}
	}

	private void Animations()
	{
		if (isHurt || isDead)
		{
			return;
		}
		GroundAnims();
		WallAnims();
		AirAnims();

	}

	private void GroundAnims()
	{
		if (!isGrounded) return;
		if (isDashing) return;

		if (horizontal != 0)
		{
			if (Input.GetButton("Sprint"))
			{
				//ChangeAnimationState(PLAYER_RUN);

			}
			else
			{
				//ChangeAnimationState(PLAYER_WALK);                
			}
		}
		else
		{
			//ChangeAnimationState(PLAYER_IDLE);
		}
	}

	private void AirAnims()
	{
		if (isGrounded) return;
		if (isTouchingWall) return;
		if (isDashing) return;

		if (rb.velocity.y > 0)
		{
			//ChangeAnimationState(PLAYER_RISE);
		}
		else
		{
			//ChangeAnimationState(PLAYER_FALL);
		}
	}

	private void WallAnims()
	{
		if (!isTouchingWall) return;
		if (isGrounded) return;
		if (isDashing) return;

		if (isWallClimbing)
		{
			if (vertical < 0)
			{
				// play climb down
			}
			else if (vertical > 0)
			{
				// play climb up
			}
			else
			{
				// play wall grab
			}
		}
		else if (isWallSliding)
		{
			//ChangeAnimationState(PLAYER_WALLSLIDE);
		}


	}

	public void PlayHurtAnim()
	{
		//ChangeAnimationState(PLAYER_HURT);
		isHurt = true;
		SetPlayerState(PlayerState.Hurt);
	}
	public void PlayDeathAnim()
	{
		//ChangeAnimationState(PLAYER_DEATH);
		isDead = true;
	}
	private void HurtAnimFinished()
	{
		Debug.Log("Hurt anim finished");
		isHurt = false;
	}
	private void DeathAnimFinished()
	{
		gameObject.SetActive(false);
	}



	void ChangeAnimationState(string newState)
	{
		//stop the same animation from interrupting itself
		if (currentState == newState) return;

		//play the animation
		animator.Play(newState);

		//reassign the current state
		currentState = newState;
	}

	#endregion
}
