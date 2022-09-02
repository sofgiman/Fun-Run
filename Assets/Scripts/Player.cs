﻿using UnityEngine;
using System.Collections;
using Mirror;


// The movement script of the player 
[RequireComponent(typeof(Controller2D))]
public class Player : NetworkBehaviour
{
	
	private bool gameStarted = false;
	public float jumpHeight = 4;  // the jump height units
	public float timeToJumpApex = .4f;  // time to get to jump's apex
	public float wallSlidingSpeedMax = 3;  // maximum speed when sliding on the wall

	public Vector2 wallJumpClimb;  // velocity for wall jumping
	float accelerationTimeAirborne = .2f;  // the smooth time of velocity.x on air
	float accelerationTimeGrounded = .1f;  // the smooth time of velocity.x on ground

	private Vector2 input;  // the input from the user. x: 1 - right arrow button pressed, -1 - left arrown y: 1 - up, -1 - down  
	float moveSpeed = 6;  // the regular speed
	public int speedBurst = 10;  // speed on ice
	float gravity;
	float jumpVelocity;  // jump velocity from ground (initialized on start)

	float landFromJumpingVelocity = -15;  // sliding down velocity
	bool isJumping;  // check if player is jumping
	bool isLandingFromSliding= false;  // if going down from the air
	Vector3 velocity; // player's velocity 
	float velocityXSmoothing;
	Controller2D controller;  // refernce to the controller
	[SerializeField] private Animator animator ;

	
	private bool isDead = false;
	private bool isDeadCooldown = false;

	private const float DEATH_TIME = 1f;
	private const float DEATH_COOLDOWN_TIME = 1f;


	private SpriteRenderer sr;
	private Material matDefault;
	[SerializeField] private Material matWhite;
	
	void Start()
	{
		if (isLocalPlayer) // if this object is the the client's own local player
		{
			sr = GetComponent<SpriteRenderer>();
			matDefault = sr.material;

			controller = GetComponent<Controller2D>();
			gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);  // calculating gravity to adjust jump height and time to jump apex
			jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);

			//isFacingRight = true;
			isJumping = false;
		}
		
}



void Update(){

if (isLocalPlayer)  // only the local player can change his movement
	{	
	if(!isDead){
		if(!gameStarted){
				velocity.y += gravity * Time.deltaTime;
				if (Above() || Below())
				{
					velocity.y = 0;
				}
				
				controller.Move(velocity * Time.deltaTime);
			}		
		else{

			input = new Vector2(1f, 0f);
			int wallDirX = (controller.collisions.left) ? -1 : 1;  // are we facing a wall and what direction
			bool wallSliding = false;  // suppossing that we aren't wall sliding

			float targetVelocityX = input.x * moveSpeed;  // this is the target velocity
			if (controller.collisions.belowBackgroundSpeed)  // if player on ice then adjust the velocity
				targetVelocityX = input.x * speedBurst;

			 // if player on ice then adjust the velocity	
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (Below()) ? accelerationTimeGrounded : accelerationTimeAirborne);
			
			if ((controller.collisions.left || controller.collisions.right) && !Below() && velocity.y < 0)  // if on wall
			{
				if (!isLandingFromSliding)
				{
					if (input.x == wallDirX)
					{
						wallSliding = true;
						isJumping = false;
						animator.SetBool("Jump", isJumping);  // not allowing jump animation

						if (velocity.y < -wallSlidingSpeedMax)
						{
							velocity.y = -wallSlidingSpeedMax;
						}
					}
				}
			}
			animator.SetBool("IsSlidingOnWall", wallSliding);

			if (Above() || Below())
			{
				velocity.y = 0;
			}
			

			if (Input.GetButtonDown("Jump"))  // if player wants to jump
			{	
				if (wallSliding )
				{
					isJumping = true;
					if (wallDirX == input.x)
					{
						velocity.x = -wallDirX * wallJumpClimb.x;
						velocity.y = wallJumpClimb.y;
					}
					else if (input.x == 0)  // don't jump if player isn't attached to wall
					{
						velocity.x = 0;
					}
				}
				if (Below())
				{
					isJumping = true;
					velocity.y = jumpVelocity;
				}
				animator.SetBool("Jump", isJumping);
			}
			else if (isJumping == true && Below())  // if player is back on the ground -> he isn't jumping
			{
				isJumping = false;
				animator.SetBool("Jump", isJumping);
			}


			if (Input.GetButtonDown("Slide") && IsOnAir())  // if player on air and pressed the down arrow button
			{
				isLandingFromSliding = true;
				isJumping = false;  // make flag false in case player jumped
				wallSliding = false;  // make flag false in case player on a wall
				velocity.y = landFromJumpingVelocity;
				animator.SetBool("Jump", isJumping);
				animator.SetBool("IsSlidingOnWall", wallSliding);
				if (input.x != 0)
				{
					velocity.x = velocity.x * 0f;
				}

			}
			if (isLandingFromSliding && Below())
			{
				isLandingFromSliding = false;
			}

			velocity.y += gravity * Time.deltaTime;
			animator.SetFloat("Speed", Mathf.Abs(input.x));
			controller.Move(velocity * Time.deltaTime);
		}
	}
	}
		
}

	

	bool IsOnAir()
    {
		if(controller.collisions.below || controller.collisions.belowBackgroundSpeed)
        {
			return false;
        }
		return true;
    }

	bool Below()
    {
		return controller.collisions.below || controller.collisions.belowBackgroundSpeed;

    }
	bool Above()
    {
		return controller.collisions.above || controller.collisions.aboveBackgroundSpeed;
	}

	// Func to set the isGameStarted variable (this variable is set when the game started -> allowing player to move)
	public void SetGameStarted(bool isGameStarted){
		gameStarted = isGameStarted;
	}

	// Handle the spikes collisions
	private void OnTriggerEnter2D(Collider2D other)
    {
		if(isLocalPlayer){
			// if collider is a spike and the dead cooldown isn't flaged
			if(other.gameObject.layer == LayerMask.NameToLayer("Spike") && !isDeadCooldown)
			{
				isDead = true;
				isDeadCooldown = true;
				animator.SetBool("Dead", true);
				ResetAllVaribles();
				StartCoroutine(EndDeath());  // stop the death 
				StartCoroutine(DeathFlickering());  // make player flicker
			}
		}
	}
	
	// Coroutine that handles the death of a player
	private IEnumerator EndDeath(){
		yield return new WaitForSeconds(1f);  // player is dead for one second
		animator.SetBool("Dead", false);
		isDead = false;
		yield return new WaitForSeconds(1f);  // player can't die for another one second
		isDeadCooldown = false;
		yield break;
	}

	// Coroutine to make player flicker during its death cooldown -> allowing user to know when he can't step on spikes
	private IEnumerator DeathFlickering(){

		// loop takes 4 * 0.5 = 2 seconds, same as player's death duration
		for(int i=0; i<4 ;i++){
			yield return new WaitForSeconds(0.25f);
			sr.material = matWhite;  // making material white for 0.25 sec
			yield return new WaitForSeconds(0.25f);
			sr.material = matDefault;  // goes back to default material
		}
		yield break;
	}

	// Func to reset all variables on death
	private void ResetAllVaribles(){
		// stopping animations (if active)
		animator.SetFloat("Speed",0);
		animator.SetBool("Jump", false);
		animator.SetBool("IsSlidingOnWall", false);
		isJumping = false;
		velocity.x = 0;
		velocity.y = 0;
	}
}