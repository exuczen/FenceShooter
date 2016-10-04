using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;

namespace CT {
	[RequireComponent(typeof(Image)), DisallowMultipleComponent]
	public class ImageMultiresComponent : MonoBehaviour {
		/// <summary>NOTE: Unity event</summary>
		public void Awake() {
			//LocalizationImageComponent localizationComponent = GetComponent<LocalizationImageComponent>();
			//if (!Multires.IsDefault || (!SmartLocalization.LanguageManager.Instance.IsDefault() && localizationComponent != null)) {
			//	string spriteName;
			//	var image = GetComponent<Image>();
			//	if (localizationComponent != null) {
			//		Assert.IsNotNull(localizationComponent.key);
			//		spriteName = localizationComponent.key + "_";// + SmartLocalization.LanguageManager.Instance.CurrentlyLoadedCulture.LanguageCodeShortOrFallback("en");
			//	} else {
			//		spriteName = image.sprite.name;
			//	}
			//	#if LOG_VERBOSE_MULTIRES
			//		LOG.Write("MultiresImageComponent.Awake: changing sprite, spriteName=" + spriteName);
			//	#endif
			//	image.sprite = spriteName.LoadMultiresSprite();
			//} else {
			//	#if LOG_VERBOSE_MULTIRES
			//		LOG.Write("MultiresImageComponent.Awake: skipping, multires.default=" + Multires.IsDefault + ", l10n.default=" + SmartLocalization.LanguageManager.Instance.IsDefault() + ", hasL10NComponent=" + (localizationComponent != null));
			//	#endif
			//}
		}
	}
}