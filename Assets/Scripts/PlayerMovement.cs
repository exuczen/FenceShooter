using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using Utility;

namespace FenceShooter {
	public class PlayerMovement : MonoBehaviour {
		public float speed = 6f;            // The speed that the player will move at.
											//public Transform gun;
		public Transform gunBarrelEnd;
		//public GameObject cannonMark;
		//public Transform cannonTransform;
		public GameObject touchMark;

		Vector3 movement;                   // The vector to store the direction of the player's movement.
		Animator anim;                      // Reference to the animator component.
		private NavMeshAgent navMeshAgent;
		private bool navMeshWalking;
		Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
											//LineRenderer cannonLine;
#if !MOBILE_INPUT
		int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
		int shootableMask;
		int enemyMask;
		float camRayLength = 100f;          // The length of the ray from the camera into the scene.
#endif

		void Awake() {
#if !MOBILE_INPUT
			// Create a layer mask for the floor layer.
			floorMask = LayerMask.GetMask("Floor");
			shootableMask = LayerMask.GetMask("Shootable");
			enemyMask = LayerMask.GetMask("Enemy");
			Utils.Log("floorMask=" + floorMask + " shootableMask=" + shootableMask + " enemyMask=" + enemyMask);
#endif

			// Set up references.
			anim = GetComponent<Animator>();
			playerRigidbody = GetComponent<Rigidbody>();
			navMeshAgent = GetComponent<NavMeshAgent>();
			//cannonLine = cannonTransform.GetComponent<LineRenderer>();
		}

		void Update() {
			if (navMeshAgent.enabled) {
				if (Input.GetButtonDown("Fire2")) {
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, 100)) {
						Utils.Log("navMeshAgent new destination");
						navMeshAgent.destination = hit.point;
						navMeshAgent.Resume();
					} else {
						Utils.Log("raycast missed");
					}
				}
				if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
					if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
						navMeshWalking = false;
				} else {
					navMeshWalking = true;
				}
			}
		}
		void FixedUpdate() {
			// Store the input axes.
			//float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
			//float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");

			// Create a boolean that is true if either of the input axes is non-zero.
			bool axisWalking = h != 0f || v != 0f;
			if (axisWalking && (navMeshWalking || navMeshAgent.hasPath)) {
				navMeshAgent.Stop();
				navMeshAgent.ResetPath();
				navMeshWalking = false;
			}

			// Move the player around the scene.
			Move(h, v);

			// Turn the player to face the mouse cursor.
			Turning();

			// Animate the player.
			// Tell the animator whether or not the player is walking.
			//Utils.Log("axisWalking=" + axisWalking + " navMeshWalking=" + navMeshWalking);
			anim.SetBool("IsWalking", axisWalking || navMeshWalking);
		}

		void Move(float h, float v) {
			// Set the movement vector based on the axis input.
			movement.Set(h, 0f, v);

			// Normalise the movement vector and make it proportional to the speed per second.
			movement = movement.normalized * speed * Time.deltaTime;

			// Move the player to it's current position plus the movement.
			playerRigidbody.MovePosition(transform.position + movement);
		}

		void Turning() {
#if !MOBILE_INPUT
			// Create a ray from the mouse cursor on screen in the direction of the camera.
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Create a RaycastHit variable to store information about what was hit by the ray.
			RaycastHit floorHit;

			// Perform the raycast and if it hits something on the floor layer...
			if (Physics.Raycast(camRay, out floorHit, camRayLength, enemyMask | floorMask | shootableMask)) {
				touchMark.transform.position = floorHit.point;

				// Create a vector from the player to the point on the floor the raycast from the mouse hit.
				//Vector3 playerToMouse = floorHit.point - transform.position;
				Vector3 playerToMouse = floorHit.point - gunBarrelEnd.position;

				// Ensure the vector is entirely along the floor plane.
				//playerToMouse.y = 0f;

				// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
				Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

				// Set the player's rotation to this new rotation.
				playerRigidbody.MoveRotation(newRotation);


				//Vector3 cannonToHit = floorHit.point - cannonTransform.position;
				//Quaternion cannonRotation = Quaternion.LookRotation(cannonToHit);
				//cannonTransform.rotation = cannonRotation;
				//cannonLine.SetPosition(0, cannonTransform.position);
				//cannonLine.SetPosition(1, floorHit.point);
			}
#else

            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
		}

	}
}