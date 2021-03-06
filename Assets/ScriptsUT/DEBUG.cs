#if LOG_VERBOSE
//#warning Utility.Debug log is ENABLED for VERBOSE
#else
//#warning Utility.Debug log is disabled for VERBOSE
#endif
#if LOG_ERROR
//#warning Utility.Debug log is ENABLED for ERROR
#else
//#warning Utility.Debug log is disabled for ERROR
#endif
#if LOG_VERBOSE || LOG_ERROR
#define LOGGING
#endif
#if LOG_VERBOSE || LOG_ERROR || DEBUG
#define NAMEHASH
#endif

using Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utility.Debug {
	public static class Log {
		public static Text logContentsTextComponent;
		public static int logContentsMaxLength;
		private static System.Text.StringBuilder logContents = new System.Text.StringBuilder(64 * (1 << 10));
		private static System.Text.StringBuilder logEntry = new System.Text.StringBuilder(1 * (1 << 10));
		private static float lastWriteTime = float.NaN;
		public static void Write(string s, bool notMainThread = false) {
			// TODO FUTURE make this method "invisible" with some new Unity feature so that clicking in console would ignore this method in stack trace and navigate to caller
			#if LOGGING
				WriteImpl(s, notMainThread);
			#endif
		}
		[System.Diagnostics.Conditional("LOGGING")]
		private static void WriteImpl(string s, bool notMainThread = false) {
			// TODO FUTURE make this method "invisible" with some new Unity feature so that clicking in console would ignore this method in stack trace and navigate to caller
			const bool prepareLine =
				#if DEBUG
				true
				#else
				false
				#endif
				;
			if (!notMainThread) {
				Log.lastWriteTime = Time.time;
			}
			if (prepareLine /*|| LOG.logContentsTextComponent != null*/) {
				logEntry.Length = 0;
				logEntry.Append("[");
				if (notMainThread) {
					logEntry.Append("?"); // in threads other than main, we show previous Time.time with question mark to indicate that time is not precise (possibly way way off!)
				}
				logEntry.Append(Log.lastWriteTime.ToString("F4"));
				logEntry.Append("] ");
				logEntry.Append(s);
				logEntry.Append("\n");
				s = logEntry.ToString(0, logEntry.Length - 1); // omit trailing newline for "LOG", use it only in own log overlay
			}
			if (!notMainThread && (Log.logContentsTextComponent != null)) {
				logContents.Insert(0, logEntry);
				if (logContents.Length > logContentsMaxLength) {
					logContents.Remove(logContentsMaxLength, logContents.Length - logContentsMaxLength);
				}
				Log.logContentsTextComponent.text = logContents.ToString();
			}
			#if DEBUG
			{
				#if UNITY_EDITOR
				{
					UnityEngine.Debug.Log(s);
				}
				#elif UNITY_ANDROID
				{
					Debug.Log(s);
				}
				#elif UNITY_IOS
				{
					Debug.Log(s);
				}
				#elif UNITY_WP8 || UNITY_WP_8_1
				{
					System.Diagnostics.Debug.WriteLine(s);
					//		Console.WriteLine(s);
					//		print(s);
				}
				#endif
			}
			#endif
		}
		public static void PrintStartupInfo() {
			Log.Write(DevInfo.All);
		}
	}

	public static class DevInfo {
		public static string All {
			get
			{
				return string.Join(", ", new string[] {
					"dev:{ " + Device + " }",
					"os:{ " + OS + " }",
					"cpu:{ " + CPU + " }",
					"sensors:{ " + Sensors + " }",
					"gpu:{ " + GPU + " }",
				});
			}
		}
		public static string Device {
			get {
				return string.Join(", ", new string[] {
					"model:" + SystemInfo.deviceModel,
					"name:" + SystemInfo.deviceName,
					"type:" + SystemInfo.deviceType.ToString(),
					"uid:" + SystemInfo.deviceUniqueIdentifier,
				});
			}
		}
		public static string OS {
			get {
				return string.Join(", ", new string[] {
					"os:" + SystemInfo.operatingSystem,
					"sys-mem-size:" + SystemInfo.systemMemorySize.ToString(),
					".net-ver:" + Environment.Version,
				});
			}
		}
		public static string CPU {
			get {
				return string.Join(", ", new string[] {
					"type:" + SystemInfo.processorType,
					"count:" + SystemInfo.processorCount.ToString(),
					"freq:" + SystemInfo.processorFrequency.ToString(),
				});
			}
		}
		public static string Sensors {
			get {
				return string.Join(", ", new string[] {
					"accel:" + SystemInfo.supportsAccelerometer.ToString(),
					"gyro:" + SystemInfo.supportsGyroscope.ToString(),
					"vibra:" + SystemInfo.supportsVibration.ToString(),
					"location-service:" + SystemInfo.supportsLocationService.ToString(),
				});
			}
		}
		public static string GPU {
			get {
				return string.Join(", ", new string[] {
					"id:" + SystemInfo.graphicsDeviceID.ToString(),
					"name:" + SystemInfo.graphicsDeviceName,
					"type:" + SystemInfo.graphicsDeviceType.ToString(),
					"vendor:" + SystemInfo.graphicsDeviceVendor,
					"vendor-id:" + SystemInfo.graphicsDeviceVendorID.ToString(),
					"version:" + SystemInfo.graphicsDeviceVersion,
					"memory-size:" + SystemInfo.graphicsMemorySize.ToString(),
					"mt:" + SystemInfo.graphicsMultiThreaded.ToString(),
					"shader-level:" + SystemInfo.graphicsShaderLevel.ToString(),
					"max-tex-size:" + SystemInfo.maxTextureSize.ToString(),
					"npot-support:" + SystemInfo.npotSupport.ToString(),
					// "supports-3dtex:" + SystemInfo.supports3DTextures.ToString(),
					// add other supports... stuff as needed
				});
			}
		}
	}

	public static class Namehash {
		/// <summary>Unity Animator state/trigger namehash to name pairs. Useful for debugging when only namehash is available in runtime.</summary>
		private static Dictionary<int, string> namehashDB =
			#if NAMEHASH
			new Dictionary<int, string>()
			#else
			null
			#endif
			;
		/// <summary>Adds pairs from given type to Unity Animator state/trigger "namehash to name" dict.
		/// Given type must contain only "public static int" fields. Call this method in static constructor of
		/// class containing namehash definitions.</summary>
		public static void DBAdd(Type type) {
			#if NAMEHASH
			{
				var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
				foreach (var field in fields) {
					Assert.IsTrue(field.FieldType == typeof(int));
					int value = (int)field.GetValue(null);
					// LOG.Write("AddToNamehashDB: " + field.Name + " = " + value);
					if (namehashDB.ContainsKey(value)) {
						string namehashName = namehashDB[value];
						Assert.IsTrue(namehashName == field.Name);
					} else {
						namehashDB.Add(value, field.Name);
					}
				}
			}
			#endif
		}
		/// <summary>Gets name of Unity Animator state/trigger given that Utility.Debug.Namehash.DBAdd was called before for every type with namehash definitions.
		/// This is needed because Unity uses only namehashes in runtime, and that makes logging/debugging animator states unfeasible.</summary>
		public static string DBGet(int namehash) {
			#if NAMEHASH
			{
				string name = null;
				if (namehashDB.TryGetValue(namehash, out name)) {
					return name;
				} else {
					return namehash.ToString();
				}
			}
			#else
			{
				return namehash.ToString();
			}
			#endif
		}
	}
}
