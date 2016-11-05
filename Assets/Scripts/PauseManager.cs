using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SurvivalShooter {
	public class PauseManager : MonoBehaviour {

		public AudioMixerSnapshot paused;
		public AudioMixerSnapshot unpaused;
		public EventTrigger touchTrigger;

		Canvas canvas;
		bool gamePaused;

		void Start() {
			canvas = GetComponent<Canvas>();
			gamePaused = false;
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				canvas.enabled = !canvas.enabled;
				Pause();
			}
		}

		public void Pause() {
			gamePaused = !gamePaused;
			if (touchTrigger)
				touchTrigger.enabled = !gamePaused;
			Time.timeScale = gamePaused ? 0 : 1;
			if (gamePaused) {
				paused.TransitionTo(.01f);
			} else {
				unpaused.TransitionTo(.01f);
			}
		}

		public void Quit() {
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
		}
	}
}