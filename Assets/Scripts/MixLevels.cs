using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace SurvivalShooter {
	public class MixLevels : MonoBehaviour {
		public AudioMixer masterMixer;
		public Slider sfxVolSlider;
		public Slider musicVolSlider;

		void Start() {
			if (sfxVolSlider) {
				sfxVolSlider.value = Mathf.Lerp(sfxVolSlider.minValue, sfxVolSlider.maxValue, 0.2f);
			}
			if (musicVolSlider) {
				musicVolSlider.value = Mathf.Lerp(musicVolSlider.minValue, musicVolSlider.maxValue, 0.5f);
			}
		}

		public void SetSfxLvl(float sfxLvl) {
			masterMixer.SetFloat("sfxVol", sfxLvl);
		}

		public void SetMusicLvl(float musicLvl) {
			masterMixer.SetFloat("musicVol", musicLvl);
		}
	}

/*
-	public class VolumeHandler : MonoBehaviour {
-		// Use this for initialization
-		void Start() {
-			if (GameObject.Find("EffectsSlider"))
-				GameObject.Find("EffectsSlider").GetComponent<Slider>().onValueChanged.AddListener(SetVolume);
-		}
-
-		void SetVolume(float volume) {
-			GetComponent<AudioSource>().volume = volume;
-		}
-
-		void OnDestroy() {
-			if (GameObject.Find("EffectsSlider"))
-				GameObject.Find("EffectsSlider").GetComponent<Slider>().onValueChanged.RemoveListener(SetVolume);
-		}
-	}
*/
}