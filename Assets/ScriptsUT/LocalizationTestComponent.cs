using Utility;
using Utility.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Utility {
	public class LocalizationTestComponent : MonoBehaviour {
		public bool enableLocalizationTest = false;
		public string languageCode;

		public static bool TestSetupIfAny() {
			//LocalizationTestComponent testLocalization = GameObject.FindObjectOfType<LocalizationTestComponent>();
			//Assert.IsTrue(GameObject.FindObjectsOfType<LocalizationTestComponent>().Length <= 1);
			//bool testEnabled = false;
			//if (testLocalization != null) {
			//	// set test localization
			//	if (testLocalization.enableLocalizationTest) {
			//		string languageCode = testLocalization.languageCode;
			//		if (!string.IsNullOrEmpty(languageCode) && SmartLocalization.LanguageManager.Instance.IsCultureSupported(languageCode)) {
			//			//SmartLocalization.LanguageManager.Instance.ChangeLanguage(languageCode);
			//			testEnabled = true;
			//			#if LOG_VERBOSE_L10N
			//				LOG.Write("L10N : using TEST LOCALIZATION languageCode=" + languageCode);
			//			#endif
			//		} else {
			//			Assert.IsNull("L10N : error : test localization not supported, language code invalid or not in smart localization");
			//		}
			//	}
			//}
			//return testEnabled;
			return false;
		}
	}
}