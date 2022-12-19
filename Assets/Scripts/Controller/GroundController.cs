using UnityEngine;
using System.Collections;
using CustomPhysics;

public class GroundController : RaycastController {

	private float maxSlopeAngle = 80f;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Initalize() {
		base.Initalize();
		collisions.faceDir = 1;
	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform) {
		Move(moveAmount, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if (moveAmount.y < 0f) {
			DescendSlope(ref moveAmount);
		}

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions(ref moveAmount);
		if (moveAmount.y != 0) {
			VerticalCollisions(ref moveAmount);
		}

		transform.Translate(moveAmount);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	void HorizontalCollisions(ref Vector2 moveAmount) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(moveAmount.x) + _skinWidth;

		if (Mathf.Abs(rayLength) < _skinWidth){
			rayLength = _skinWidth * 2f;
		}

		for (int i = 0; i < _horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX < 0f) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
			RaycastInfo info;

			Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.green);
			if (CollisionManager.GetInstance().Raycast(rayOrigin, Vector2.right * directionX, rayLength, out info)) {
				if (info.distance < Mathf.Epsilon) continue;

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength);
				float slopeAngle = Vector2.Angle(info.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = info.distance - _skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref moveAmount, slopeAngle, info.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
					moveAmount.x = (info.distance - _skinWidth) * directionX;
					rayLength = info.distance;

					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs(moveAmount.y) + _skinWidth;

		for (int i = 0; i < _verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY < 0f) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (_verticalRaySpacing * i + moveAmount.x);

			RaycastInfo info;
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.green);
			if (CollisionManager.GetInstance().Raycast(rayOrigin, Vector2.up * directionY, rayLength, out info)) {
				if (info.collider.Tag != null && info.collider.Tag.Equals("Through")) {
					if (directionY == 1 || info.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform", 0.5f);
						continue;
					}
				}

				moveAmount.y = (info.distance - _skinWidth) * directionY;
				rayLength = info.distance;

				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY < 0f;
				collisions.above = directionY < 0f;
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + _skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastInfo info;
			if (CollisionManager.GetInstance().Raycast(rayOrigin, Vector2.right * directionX, rayLength, out info)) {
				float slopeAngle = Vector2.Angle(info.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					moveAmount.x = (info.distance - _skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = info.normal;
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
	}

	void DescendSlope(ref Vector2 moveAmount) {
		RaycastInfo maxSlopeHitLeft, maxSlopeHitRight;
		bool isLeftHit = CollisionManager.GetInstance().Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + _skinWidth, out maxSlopeHitLeft);
		bool isRightHit = CollisionManager.GetInstance().Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + _skinWidth, out maxSlopeHitRight);
		if (isLeftHit ^ isRightHit) {
			SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
		}

		if (!collisions.slidingDownMaxSlope) {
			float directionX = Mathf.Sign(moveAmount.x);
			Vector2 rayOrigin = (directionX < 0f) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

			RaycastInfo info;
			if (CollisionManager.GetInstance().Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, out info)) {
				float slopeAngle = Vector2.Angle(info.normal, Vector2.up);
				if (slopeAngle != 0f && slopeAngle <= maxSlopeAngle) {
					if (Mathf.Sign(info.normal.x) == directionX) {
						if (info.distance - _skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) {
							float moveDistance = Mathf.Abs(moveAmount.x);
							float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
							moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
							moveAmount.y -= descendmoveAmountY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
							collisions.slopeNormal = info.normal;
						}
					}
				}
			}
		}
	}

	void SlideDownMaxSlope(RaycastInfo info, ref Vector2 moveAmount) {
		if (info.collider == null) return;
		float slopeAngle = Vector2.Angle(info.normal, Vector2.up);
		if (slopeAngle > maxSlopeAngle) {
			moveAmount.x = Mathf.Sign(info.normal.x) * (Mathf.Abs(moveAmount.y) - info.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

			collisions.slopeAngle = slopeAngle;
			collisions.slidingDownMaxSlope = true;
			collisions.slopeNormal = info.normal;
		}
	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			slopeNormal = Vector2.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}