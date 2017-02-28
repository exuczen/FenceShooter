using Utility;
using Utility.Debug;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Utility {
	/// <summary>Multiple resolutions for the same resource stuff</summary>
	public enum MultiresKind { Uninitialized = 0, Res256, Res512, Res1024 }
	/// <summary>Multires is a method of sprite optimization, where every "huge enough" sprite is in 3 kinds, low, mid and high resolution.
	/// In the scene all "multires sprites" are set to low resolution, this allows loading app on memory constrained devices.
	/// During app load, this class determines target resolution for device that app is ran on. Gameobjects that represent multires sprites
	/// must use addequate components that will change sprite according to device multires folder.</summary>
	public static class Multires {
		public const MultiresKind DEFAULT_MULTIRES = MultiresKind.Res256;
		public static MultiresKind Folder { get; set; }
		public static bool IsDefault { get { return Folder == DEFAULT_MULTIRES; } }

		static Multires() {
			int screenWidth = Screen.width;
			// list of common device screen resolutions (WxH):
			// 240x320(A), 240x400(A), 320x480(A), 360x640(A), 480x800/854(A), 540x960(A), 640x960/1136(i), 720x1280(A), 750x1334(i), 768x1024(i), 1080x1920(A/i), 1242x2208(i), 1440x2560(A), 1536x2048(i), 2048x2732(i)
			// multires kind is determined using screen width now, but this depends on app - for landscape apps change this code accordingly
			Assert.IsTrue(screenWidth < Screen.height);
			// use screen "dominant" dimension to determine multires kind
			Folder = screenWidth < 320 ? MultiresKind.Res256 : screenWidth < 540 ? MultiresKind.Res512 : MultiresKind.Res1024;
			#if LOG_VERBOSE_MULTIRES
				LOG.Write("Multires.(static)ctor: MultiresFolder=" + Folder);
			#endif
		}
	}
}
