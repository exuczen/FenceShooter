using Utility;
using Utility.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

namespace Utility {
	[RequireComponent(typeof(Text)), DisallowMultipleComponent]
	public class LocalizationStringComponent : MonoBehaviour {
		public string key;
		/// <summary>NOTE: Unity event</summary>
		public void Awake() {
			Assert.IsNotNull(key);
			if (key != null) {
				#if LOG_VERBOSE
					// LOG.Write("LocalizationStringComponent.Awake: changing string, key = " + key);
				#endif
				string translatedText = key.L();
				Assert.IsNotNull(translatedText);
				GetComponent<Text>().text = translatedText;
			}
		}
	}
}