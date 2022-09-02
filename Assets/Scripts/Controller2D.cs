using UnityEngine;
using System.Collections;
using System.ComponentModel;
using Mirror;
	
[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : NetworkBehaviour
{
	
	public LayerMask collisionMask;
	public LayerMask collisionMaskGroundSpeed;
	private Vector3 velocity;	
	
	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	

	float horizontalRaySpacing;
	float verticalRaySpacing;

	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	void Start()
	{
		if (isLocalPlayer)
		{
			collider = GetComponent<BoxCollider2D>();
			CalculateRaySpacing();
			collisions.faceDir = 1;
		}
	}

	// Function to move the player
	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
		UpdateRaycastOrigins();  // raycasts origins always changing because player is moving
		collisions.Reset();
		if (velocity.x != 0)  // if player is moving on the x axis then check for collisions and where he facing
		{
			collisions.faceDir = (int)Mathf.Sign(velocity.x);
				
			HorizontalCollisions(ref velocity);
			
		}
		if (velocity.y != 0)  // if player is moving on the y axis then check for collision
		{
			VerticalCollisions(ref velocity);
		}

		
		
		transform.Translate(velocity);
	}

	// Func to find horizontal collisions
	// velocity - player's velocity
	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = collisions.faceDir;  // player's horizontal direction
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		if(Mathf.Abs( velocity.x) < skinWidth)
        {
			rayLength = 2 * skinWidth;
        }

		for (int i = 0; i < horizontalRayCount; i++)
		{
			// if player is facing left then the raycast origin should be the left one
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit)  // if a raycast hit a ground
			{
				velocity.x = (hit.distance - skinWidth) * directionX;   // adjusting velocity so player won't override ground
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	// Func to find vertical collisions
	int num = 1;
	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);  // player's y direction
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{	
			// if player is facing buttom then the raycast origin should be the buttom one
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hitBackground = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);


			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			
			if (hitBackground)   // if a raycast hit a ground
			{
				velocity.y = (hitBackground.distance - skinWidth) * directionY;   // adjusting velocity so player won't override ground
				rayLength = hitBackground.distance;

				
				if (hitBackground.collider.gameObject.layer == LayerMask.NameToLayer("Ground")){
					collisions.below = directionY == -1;
					collisions.above = directionY == 1;
				}
				else if (hitBackground.collider.gameObject.layer == LayerMask.NameToLayer("SpeedGround"))
				{
					collisions.belowBackgroundSpeed = directionY == -1;
					collisions.aboveBackgroundSpeed = directionY == 1;
				}
				
			}

		}
	}


	// a function to update the raycast origins
	void UpdateRaycastOrigins()
	{
		
			if(collider!= null){

            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);  // shrinking the bounds so it won't touch the player's skin


            raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
			}
        

	}

	// calculate the ray casts space between each ray
	void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);  // shrinking the bounds so it won't touch the player's skin

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		// dividing the width and height equally between the rays origins
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	// a struct to store the raycasts origins
	struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	// a struct to know what collision the player have
	public struct CollisionInfo
    {
		public bool aboveBackgroundSpeed, belowBackgroundSpeed;  // ice above or ice below flags
		public bool above, below;  // regular ground above or below
		public bool left, right;  // ground is left or right to the player
		public int faceDir;  // what direction the player faces (1 - right, -1 - left)

		public void Reset()
        {
			aboveBackgroundSpeed = belowBackgroundSpeed = false;
			above = below = false;
			left = right = false;
        }
    }
	

	
}