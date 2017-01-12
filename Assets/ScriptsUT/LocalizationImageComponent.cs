using Utility;
using Utility.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

namespace Utility {
	[RequireComponent(typeof(Image)), DisallowMultipleComponent]
	public class LocalizationImageComponent : MonoBehaviour {
		public string key;
		/// <summary>NOTE: Unity event</summary>
		public void Awake() {
			Assert.IsNotNull(key);
			if (key != null) {
				bool hasMultires = GetComponent<ImageMultiresComponent>() != null;
				if (!hasMultires) {
#if LOG_VERBOSE
						// LOG.Write("ImageLocalizationComponent.Awake: changing sprite");
#endif
					Texture2D tex2d = null; //(Texture2D)SmartLocalization.LanguageManager.Instance.GetTexture(key);
					Assert.IsNotNull(tex2d);
					GetComponent<Image>().sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0.5f, 0.5f));
				} else {
					#if LOG_VERBOSE
						// LOG.Write("ImageLocalizationComponent.Awake: skipping, has multires");
					#endif
				}
			}
		}
	}
}