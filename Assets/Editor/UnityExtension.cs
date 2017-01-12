#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>Unity extension stuff</summary>
public static class UnityExtension
{
	[MenuItem("Scenes/Shop")]
	public static void OpenSceneShop() {
		Utility.ExtensionUNITY.OpenScene("Scenes/Shop");
	}
	[MenuItem("Scenes/Main")]
	public static void OpenSceneMain() {
		Utility.ExtensionUNITY.OpenScene("Scenes/Level 01");
	}
	[MenuItem("Scenes/SurvivalShooter")]
	public static void OpenSceneCompleted() {
		Utility.ExtensionUNITY.OpenScene("_CompletedAssets/Scenes/Level 01 5.x");
	}

	/// <summary>Alt+C to take a screenshot of game window</summary>
	[MenuItem("Window/Capture Screenshot &c")]
	public static void TakeScreenshot() {
		Utility.ExtensionUNITY.Screenshot.Take();
	}
}
#endif
