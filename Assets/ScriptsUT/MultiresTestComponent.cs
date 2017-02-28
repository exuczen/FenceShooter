using Utility;
using Utility.Debug;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Utility {
	public class MultiresTestComponent : MonoBehaviour {
		public bool enableResolutionTest = false;
		public MultiresKind resolution = MultiresKind.Res256;

		public static void TestSetupIfAny() {
			MultiresTestComponent testMultires = GameObject.FindObjectOfType<MultiresTestComponent>();
			Assert.IsTrue(GameObject.FindObjectsOfType<MultiresTestComponent>().Length <= 1);
			if (testMultires != null) {
				// set test multires
				if (testMultires.enableResolutionTest) {
					Assert.IsTrue(testMultires.resolution != MultiresKind.Uninitialized);
					if (testMultires.resolution != MultiresKind.Uninitialized) {
						Multires.Folder = testMultires.resolution;
						#if LOG_VERBOSE_MULTIRES
							LOG.Write("App.AwakeOwn : Multires : using TEST MULTIRES folder=" + Multires.Folder);
						#endif
					}
				} else {
					#if LOG_VERBOSE_MULTIRES
						LOG.Write("App.AwakeOwn : Multires : TEST MULTIRES DISABLED");
					#endif
				}
			} else {
				#if LOG_VERBOSE_MULTIRES
					LOG.Write("App.AwakeOwn : Multires : TEST MULTIRES NOT FOUND");
				#endif
			}
		}
	}
}