using UnityEngine;
using System.Collections;

namespace FenceShooter {
	public class CameraFollow : MonoBehaviour {
		public Transform target;            // The position that that camera will be following.
		public float smoothing = 5f;        // The speed with which the camera will be following.
		//[Tooltip("AudioListener.volume"), Range(0f, 1f)]
		//public float soundVolume = 1f;

		Vector3 offset;                     // The initial offset from the target.


		void Start() {
			// Calculate the initial offset.
			offset = transform.position - target.position;
		}


		void FixedUpdate() {
			// Set AudioListener's volume
#if UNITY_EDITOR
			//if (AudioListener.volume != soundVolume) {
			//	AudioListener.volume = soundVolume;
			//}
#endif
			// Create a postion the camera is aiming for based on the offset from the target.
			Vector3 targetCamPos = target.position + offset;

			// Smoothly interpolate between the camera's current position and it's target position.
			transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
		}
	}
}