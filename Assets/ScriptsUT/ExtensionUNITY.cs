#if UNITY_EDITOR
using Utility;
using Utility.DEBUG;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Utility {
	/// <summary>Quick access to scenes under Assets/Scenes folder: add method below to a script in project
	/// Example: [MenuItem("&Scenes/&Main")]public static void OpenSceneMain() {CT.ExtensionUNITY.OpenScene("Main");}
	/// adds top menu "scenes" with alt-s shortcut, with menu item "main" with "m" shortcut.
	/// % (ctrl on Windows, cmd on OS X), # (shift), & (alt), _ (underscore, no key modifiers).</summary>
	public static class ExtensionUNITY {
		/// <summary>Opens the scene if not opened already. If current scene is modified,
		/// user is asked to save before opening. We assume all scenes are under Assets/Scenes folder.</summary>
		/// <param name="name">Name of the scene (without extension and path)</param>
		public static void OpenScene(string name) {
			var path = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
			if (path[path.Length - 1].Equals(name + ".unity")) {
				return; // do not open if current scene is the same as being opened
			}
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
				EditorSceneManager.OpenScene("Assets/" + name + ".unity");
			}
		}

		public static class Screenshot {

			private const string FileNamePrefix = "Screen";
			private const string FolderName = "Screenshots";
			private static string ScreenshotsPath { get { return Application.dataPath + "/../" + FolderName; } }
			private static string DateWithTimeString { get { return System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"); } }
			private static string FileName(int width, int height) { return string.Format("{0}-{1}-{2}x{3}.png", FileNamePrefix, DateWithTimeString, width, height); }
			private static string FilePath(int width, int height) { return ScreenshotsPath + "/" + FileName(width, height); }

			public static void Take() {
				// try to use private methods to get game view size, sometimes Screen.width/height have wrong values
				// get type of "game" window
				Type gameViewType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView");
				Assert.IsNotNull(gameViewType);
				if (gameViewType == null) {
					#if LOG_ERROR
						LOG.Write("ExtensionUNITY.Screenshot.Take: unable to get UnityEditor.GameView type");
					#endif
					return;
				}
				// focus "game" window before trying to get view size
				EditorWindow.GetWindow(gameViewType).Focus();
				Vector2 gameViewSize;
				MethodInfo getSizeUsingPrivateMethod = gameViewType.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
				if (getSizeUsingPrivateMethod != null) {
					gameViewSize = (Vector2)getSizeUsingPrivateMethod.Invoke(null, null);
				} else {
					#if LOG_ERROR
						LOG.Write("ExtensionUNITY.Screenshot.Take: unable to get GetSizeOfMainGameView method, fallback to GetMainGameViewTargetSize method");
					#endif
					getSizeUsingPrivateMethod = gameViewType.GetMethod("GetMainGameViewTargetSize", BindingFlags.NonPublic | BindingFlags.Static);
					if (getSizeUsingPrivateMethod != null) {
						gameViewSize = (Vector2)getSizeUsingPrivateMethod.Invoke(null, null);
					} else {
						#if LOG_ERROR
							LOG.Write("ExtensionUNITY.Screenshot.Take: unable to get GetMainGameViewTargetSize method, fallback to Screen.width and Screen.height");
						#endif
						gameViewSize = new Vector2(Screen.width, Screen.height);
					}
				}
				#if LOG_VERBOSE
					LOG.Write("ExtensionUNITY.Screenshot.Take: gameViewSize=" + gameViewSize.x + "," + gameViewSize.y + " Screen.width,height=" + Screen.width + "," + Screen.height);
				#endif
				// create actual screenshot
				CaptureScreenDefault(gameViewSize);
			}

			/// <summary>Take screenshot using Unity API. Uses fileNameResolution only for fileName, screenshot is taken from Game's view with it's current resolution, regardless of fileNameResolution.</summary>
			private static void CaptureScreenDefault(Vector2 fileNameResolution) {
				try {
					CreateScreenshotsFolderInNotExists();
					string filePath = FilePath((int)fileNameResolution.x, (int)fileNameResolution.y);
					Application.CaptureScreenshot(filePath, 1);
					#if LOG_VERBOSE
						LOG.Write("ExtensionUNITY.Screenshot.CaptureScreenDefault: screenshot saved in " + filePath);
					#endif
				} catch (Exception exception) {
#if LOG_ERROR
						LOG.Write("ExtensionUNITY.Screenshot.CaptureScreenDefault: " + exception.ToString());
#endif
					LOG.Write(exception.ToString());
					throw;
				}
			}

			/// <summary>Take screenshot using RenderTexture and Camera. Uses size for both fileName and texture resolution.</summary>
			public static void CaptureScreenWithRenderingToTexture(Vector2 size) {
				int width = (int)size.x;
				int height = (int)size.y;
				RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
				// TODO: Customize anti-aliasing value. Anti-aliasing value must be one of (1, 2, 4 or 8), indicating the number of samples per pixel.
				renderTexture.antiAliasing = 4;
				RenderTexture activePreviously = RenderTexture.active;
				RenderTexture.active = renderTexture;
				Camera cam = Camera.main;
				RenderTexture target = cam.targetTexture;
				cam.targetTexture = renderTexture;
				cam.Render();
				Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
				// Read screen contents into the texture
				tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
				tex.Apply();
				// Encode texture into PNG
				var bytes = tex.EncodeToPNG();
				try {
					CreateScreenshotsFolderInNotExists();
					string filePath = FilePath(width, height);
					File.WriteAllBytes(filePath, bytes);
					#if LOG_VERBOSE
						LOG.Write("ExtensionUNITY.Screenshot.CaptureScreenWithRenderingToTexture: screenshot saved in " + filePath);
					#endif
				} catch (System.Exception exception) {
#if LOG_ERROR
						LOG.Write("ExtensionUNITY.Screenshot.CaptureScreenWithRenderingToTexture: " + exception.ToString());
#endif
					LOG.Write(exception.ToString());
					throw;
				}
				RenderTexture.active = activePreviously;
				cam.targetTexture = target;
				UnityEngine.Object.DestroyImmediate(tex);
				UnityEngine.Object.DestroyImmediate(renderTexture);
			}

			private static void CreateScreenshotsFolderInNotExists() {
				string screenshotsPath = ScreenshotsPath;
				if (!Directory.Exists(screenshotsPath)) {
					Directory.CreateDirectory(screenshotsPath);
				}
			}

		}
	}
}
#endif
