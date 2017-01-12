using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace SurvivalShooter {
	public class GameOverManager : MonoBehaviour {
		Animator anim;                          // Reference to the animator component.

		void Awake() {
			// Set up the reference.
			anim = GetComponent<Animator>();
		}

		public void RestartLevel() {
			SceneManager.LoadScene(0);
		}

	}
}