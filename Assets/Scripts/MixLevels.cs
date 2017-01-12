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
				sfxVolSlider.value = Mathf.Lerp(sfxVolSlider.minValue, sfxVolSlider.maxValue, 0.3f);
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
}