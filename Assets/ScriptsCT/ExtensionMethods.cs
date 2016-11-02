using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Reflection;

namespace CT {
	public static class ExtensionMethodsForBounds {
		public static void ChunkifyX(this Bounds piece, Bounds anvil, float[] chunkWidths) {
			#if LOG_VERBOSE
				// LOG.Write("Bounds.ChunkifyX : anvil: " + anvil.ToStringExt(3) + " , piece: " + piece.ToStringExt(3));
			#endif
			Assert.IsTrue(chunkWidths.Length == 3);
			float anvilXMin = anvil.min.x, anvilXMax = anvil.max.x;
			float pieceXMin = piece.min.x, pieceXMax = piece.max.x;
			Assert.IsTrue(anvilXMin < anvilXMax);
			Assert.IsTrue(pieceXMin < pieceXMax);
			if (pieceXMax <= anvilXMin) {
				// "right of piece" to the left of "left of anvil" : chunk left PRESENT ONLY
				chunkWidths[0] = pieceXMax - pieceXMin;
				chunkWidths[1] = 0f;
				chunkWidths[2] = 0f;
				// END
			} else {
				// new "left of piece" after creating new chunk
				float tmpPieceXMin;
				// "right of piece" to the right of "left of anvil" : outcome depends on "left of piece" 
				if (pieceXMin < anvilXMin) {
					// "left of piece" to the left of "left of anvil" : chunk left PRESENT surely, others? possibly
					chunkWidths[0] = anvilXMin - pieceXMin;
					tmpPieceXMin = anvilXMin;
				} else {
					// "left of piece" to the right of "left of anvil" : chunk left ABSENT surely, others? possibly
					chunkWidths[0] = 0f;
					tmpPieceXMin = pieceXMin;
				}
				if (pieceXMax < anvilXMax) {
					// "right of piece" to the left of "right of anvil" : chunk middle PRESENT surely, chunk right ABSENT surely
					chunkWidths[1] = pieceXMax - tmpPieceXMin;
					chunkWidths[2] = 0f;
					// END
				} else {
					// "right of piece" to the right of "right of anvil" : chunk middle depends on "left of piece", chunk right PRESENT surely
					if (tmpPieceXMin < anvilXMax) {
						// "left of piece" to the left of "right of anvil" : chunk middle PRESENT surely
						chunkWidths[1] = anvilXMax - tmpPieceXMin;
						tmpPieceXMin = anvilXMax; // new "left of piece" after creating new chunk
					} else {
						// "left of piece" to the right of "right of anvil" : chunk middle ABSENT surely
						chunkWidths[1] = 0f;
					}
					chunkWidths[2] = pieceXMax - tmpPieceXMin;
					// END
				}
			}
			#if LOG_VERBOSE
				// LOG.Write("Bounds.ChunkifyX : widths: " + chunkWidths[0] + ", " + chunkWidths[1] + ", " + chunkWidths[2]);
			#endif
		}
		public static string ToStringExt(this Bounds b, int digits = 3) {
			#if DEBUG
			{
				return "{center:" + b.center.ToStringExt(digits) + ",size:" + b.size.ToStringExt(digits) + ",min:" + b.min.ToStringExt(digits) + ",max:" + b.max.ToStringExt(digits) + "}";
			}
			#else
			{
				return "";
			}
			#endif
		}
	}

	public static class ExtensionMethodsForSpriteRenderer {
		public static void SetColorAlpha(this SpriteRenderer sr, float alpha) {
			Color color = sr.color;
			color.a = alpha;
			sr.color = color;
		}
		/// <summary>Use spriteRenderer.sprite.rect as basis for own rect</summary>
		public static void CropSprite(this SpriteRenderer sr, Rect rectPx) {
			//var previousSprite = sr.sprite;
			sr.sprite = sr.sprite.Crop(rectPx);
			// TODO REWRITE make possible to GameObject.Destroy(previousSprite); if it was generated and is not asset
			// GameObject.Destroy(previousSprite);
		}
	}
	
	public static class ExtensionMethodsForGraphic {
		public static void SetColorAlpha(this Graphic text, float alpha) {
			Color color = text.color;
			color.a = alpha;
			text.color = color;
		}
	}

	public static class ExtensionMethodsForAudioSource {
		/// <summary>Plays this audio source in loop "nicely". When there is a need for looped sound to be stopped
		/// not abruptly but after finishing whole loop, use <seealso cref="ExtensionMethodsForAudioSource.PlayLoopNicely(AudioSource)"/> and <seealso cref="ExtensionMethodsForAudioSource.StopLoopNicely(AudioSource)"/>.</summary>
		public static void PlayLoopNicely(this AudioSource audioSource) {
			audioSource.loop = true;
			audioSource.Play();
		}
		/// <summary>Stops playing but allows for currently playing clip to end. For stopping looped sounds nicely.
		/// <para>Always use <seealso cref="ExtensionMethodsForAudioSource.PlayLoopNicely(AudioSource)"/> when using this method</para></summary>
		public static void StopLoopNicely(this AudioSource audioSource) {
			// just setting "loop" to false will cause AudioSource to stop playing after finishing currently played clip
			audioSource.loop = false;
		}
	}

	public static class ExtensionMethodsForBoxCollider2D {
		public static Bounds GetLocalBounds(this BoxCollider2D bc2d) {
			return new Bounds(bc2d.transform.localPosition + (Vector3)bc2d.offset, bc2d.size);
		}
		public static void SetLocalSize(this BoxCollider2D bc2d, float? x, float? y) {
			var size = bc2d.size;
			if (x != null) {
				size.x = (float)x;
			}
			if (y != null) {
				size.y = (float)y;
			}
			bc2d.size = size;
		}
		public static void SetLocalSizeX(this BoxCollider2D bc2d, float x) {
			var size = bc2d.size;
			size.x = x;
			bc2d.size = size;
		}
		public static string ToStringExt(this BoxCollider2D bc2d, int digits = 3) {
			#if DEBUG
			{
				return "{offset:" + bc2d.offset.ToStringExt(digits) + ",size:" + bc2d.size.ToStringExt(digits) + ",bounds:" + bc2d.bounds.ToStringExt(digits) + "}";
			}
			#else
			{
				return "";
			}
			#endif
		}
	}
	
	public static class ExtensionMethodsForRectTransform {
		public static Bounds GetLocalBounds(this RectTransform trans) {
			
			return new Bounds(trans.localPosition, trans.rect.size);
		}
		public static void SetDefaultScale(this RectTransform trans) {
			trans.localScale = Vector3.one;
		}
		
		public static Vector2 GetSize(this RectTransform trans) {
			return trans.rect.size;
		}
		/// <summary>Pivot respecting size change</summary>
		public static void SetSize(this RectTransform trans, Vector2 newSize) {
			Vector2 oldSize = trans.rect.size;
			Vector2 deltaSize = newSize - oldSize;
			trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
			trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
		}
		public static void SetLocalSizeX(this RectTransform trans, float sizeX) {
			trans.SetSize(new Vector2(sizeX, trans.GetHeight()));
		}

		public static float GetWidth(this RectTransform trans) {
			return trans.rect.width;
		}
		public static float GetHeight(this RectTransform trans) {
			return trans.rect.height;
		}
		public static void SetWidth(this RectTransform trans, float newSize) {
			SetSize(trans, new Vector2(newSize, trans.rect.size.y));
		}
		public static void SetHeight(this RectTransform trans, float newSize) {
			SetSize(trans, new Vector2(trans.rect.size.x, newSize));
		}
		
		public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
		}
		public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}
		public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}
		public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}
		public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}
		
		public static string ToStringExt(this RectTransform trans, int digits = 3) {
			#if DEBUG
			{
				return "{pivot:" + trans.pivot.ToStringExt(digits) + ",transform:" + ((Transform)trans).ToStringExt(digits) + ",anchPos:" + trans.anchoredPosition.ToStringExt(digits) + "}";
			}
			#else
			{
				return "";
			}
			#endif
		}
	}
	
	public static class ExtensionMethodsForTransform {
		public static void SetLocalScaleX(this Transform trans, float x) {
			var locscale = trans.localScale;
			locscale.x = x;
			trans.localScale = locscale;
		}
		public static void SetLocalScaleY(this Transform trans, float y) {
			var locscale = trans.localScale;
			locscale.y = y;
			trans.localScale = locscale;
		}
		public static void SetLocalPosition(this Transform trans, float? x, float? y, float? z) {
			var locpos = trans.localPosition;
			if (x != null) {
				locpos.x = (float)x;
			}
			if (y != null) {
				locpos.y = (float)y;
			}
			if (z != null) {
				locpos.z = (float)z;
			}
			trans.localPosition = locpos;
		}
		public static void SetLocalPositionXY(this Transform trans, Vector2 xy) {
			var locpos = trans.localPosition;
			locpos.x = xy.x;
			locpos.y = xy.y;
			trans.localPosition = locpos;
		}
		public static void SetLocalPositionX(this Transform trans, float x) {
			var locpos = trans.localPosition;
			locpos.x = x;
			trans.localPosition = locpos;
		}
		public static void SetLocalPositionY(this Transform trans, float y) {
			var locpos = trans.localPosition;
			locpos.y = y;
			trans.localPosition = locpos;
		}
		public static void SetLocalPositionZ(this Transform trans, float z) {
			var locpos = trans.localPosition;
			locpos.z = z;
			trans.localPosition = locpos;
		}
		public static void AddToLocalPosition(this Transform trans, float? x, float? y, float? z) {
			var locpos = trans.localPosition;
			if (x != null) {
				locpos.x += (float)x;
			}
			if (y != null) {
				locpos.y += (float)y;
			}
			if (z != null) {
				locpos.z += (float)z;
			}
			trans.localPosition = locpos;
		}
		public static void AddToLocalPositionX(this Transform trans, float x) {
			var locpos = trans.localPosition;
			locpos.x += x;
			trans.localPosition = locpos;
		}
		public static void AddToLocalPositionY(this Transform trans, float y) {
			var locpos = trans.localPosition;
			locpos.y += y;
			trans.localPosition = locpos;
		}
		public static void AddToLocalPositionZ(this Transform trans, float z) {
			var locpos = trans.localPosition;
			locpos.z += z;
			trans.localPosition = locpos;
		}
		public static IEnumerator SmoothMove(this Transform trans, float duration, float? xSrc, float? xDst, float? ySrc, float? yDst, float? zSrc, float? zDst) {
			float t = 0f;
			while (t <= 1f) {
				t += Time.deltaTime / duration;
				trans.SetLocalPosition(
					xSrc == null ? (float?)null : Mathf.Lerp((float)xSrc, (float)xDst, Mathf.SmoothStep(0f, 1f, t)),
					ySrc == null ? (float?)null : Mathf.Lerp((float)ySrc, (float)yDst, Mathf.SmoothStep(0f, 1f, t)),
					zSrc == null ? (float?)null : Mathf.Lerp((float)zSrc, (float)zDst, Mathf.SmoothStep(0f, 1f, t))
				);
				yield return null;
			}
		}
		public static IEnumerator SmoothMoveX(this Transform trans, float duration, float xSrc, float xDst) {
			float t = 0f;
			while (t <= 1f) {
				t += Time.deltaTime / duration;
				trans.SetLocalPositionX(Mathf.Lerp(xSrc, xDst, Mathf.SmoothStep(0f, 1f, t)));
				yield return null;
			}
		}
		public static IEnumerator SmoothMoveY(this Transform trans, float duration, float ySrc, float yDst) {
			float t = 0f;
			while (t <= 1f) {
				t += Time.deltaTime / duration;
				trans.SetLocalPositionY(Mathf.Lerp(ySrc, yDst, Mathf.SmoothStep(0f, 1f, t)));
				yield return null;
			}
		}
		public static IEnumerator SmoothMoveZ(this Transform trans, float duration, float zSrc, float zDst) {
			float t = 0f;
			while (t <= 1f) {
				t += Time.deltaTime / duration;
				trans.SetLocalPositionZ(Mathf.Lerp(zSrc, zDst, Mathf.SmoothStep(0f, 1f, t)));
				yield return null;
			}
		}
		public static Vector2 ScreenEdge(this Transform trans, Camera camera, float viewportX, float viewportY) {
			return trans.InverseTransformPoint(camera.ViewportToWorldPoint(new Vector3(viewportX, viewportY, trans.position.z - camera.transform.position.z)));
		}
		public static float ScreenEdgeHorizontalNegative(this Transform trans, Camera camera) { return trans.ScreenEdge(camera, 0f, 0.5f).x; }
		public static float ScreenEdgeHorizontalPositive(this Transform trans, Camera camera) { return trans.ScreenEdge(camera, 1f, 0.5f).x; }
		public static float ScreenEdgeVerticalNegative(this Transform trans, Camera camera) { return trans.ScreenEdge(camera, 0.5f, 0f).y; }
		public static float ScreenEdgeVerticalPositive(this Transform trans, Camera camera) { return trans.ScreenEdge(camera, 0.5f, 1f).y; }
		public static void DestroyChildren(this Transform trans, int startIdx = 0) {
			for (int l = trans.childCount, i = startIdx; i < l; i++) {
				Object.Destroy(trans.GetChild(i).gameObject);
			}
		}
		#if UNITY_EDITOR
		public static void DestroyChildrenInEditMode(this Transform trans, int startIdx = 0) {
			while (trans.childCount > startIdx) {
				// we must use DestroyImmediate in edit mode
				Object.DestroyImmediate(trans.GetChild(startIdx).gameObject);
			}
		}
		#endif
		/// <summary>Destroy childer that works also in edit mode</summary>
		public static void DestroyChildrenInBothModes(this Transform trans, int startIdx = 0) {
			#if UNITY_EDITOR
			{
				trans.DestroyChildrenInEditMode(startIdx);
			}
			#else
			{
				trans.DestroyChildren(startIdx);
			}
			#endif
		}
		public static string ToStringExt(this Transform trans, int digits = 3) {
			#if DEBUG
			{
				return "{locPos:" + trans.localPosition.ToStringExt(digits) + ",locScale:" + trans.localScale.ToStringExt(digits) + ",pos:" + trans.position.ToStringExt(digits) + "}";
			}
			#else
			{
				return "";
			}
			#endif
		}
	}

	public static class ExtensionMethodsForVector3 {
		public static string ToStringExt(this Vector3 v3, int digits = -1) {
			#if DEBUG
			{
				if (digits < 0) {
					return "(" + v3.x + "," + v3.y + "," + v3.z + ")";
				} else {
					string digitTag = "F" + digits.ToString();
					return "(" + v3.x.ToString(digitTag) + ", " + v3.y.ToString(digitTag) + ", " + v3.z.ToString(digitTag) + ")";
				}
			}
			#else
			{
				return "";
			}
			#endif
		}
	}

	public static class ExtensionMethodsForVector4 {
		public static Vector4 Normalize(this Vector4 v4) {
			v4.Normalize();
			return v4;
		}
		public static string ToStringExt(this Vector4 v4, int digits = -1) {
			#if DEBUG
			{
				if (digits < 0) {
					return "(" + v4.x + "," + v4.y + "," + v4.z + "," + v4.w + ")";
				} else {
					string digitTag = "F" + digits.ToString();
					return "(" + v4.x.ToString(digitTag) + ", " + v4.y.ToString(digitTag) + ", " + v4.z.ToString(digitTag) + ", " + v4.w.ToString(digitTag) + ")";
				}
			}
			#else
			{
				return "";
			}
			#endif
		}
	}

	public static class ExtensionMethodsForRect {
		public static string ToStringExt(this Rect r, int digits = -1) {
			#if DEBUG
			{
				if (digits < 0) {
					return "(" + r.xMin + ".." + r.xMax + "," + r.yMin + ".." + r.yMax + "|" + "w=" + r.width + ",h=" + r.height + ")";
				} else {
					string digitTag = "F" + digits.ToString();
					return "(" + r.xMin.ToString(digitTag) + ".." + r.xMax.ToString(digitTag) + "," + r.yMin.ToString(digitTag) + ".." + r.yMax.ToString(digitTag) + "|" + "w=" + r.width.ToString(digitTag) + ",h=" + r.height.ToString(digitTag) + ")";
				}
			}
			#else
			{
				return "";
			}
			#endif
		}
	}
	
	public static class ExtensionMethodsForVector2 {
		public static Vector2 halfUnit = new Vector2(0.5f, 0.5f);
		public static Vector3 ToVector3(this Vector2 v2) {
			return new Vector3(v2.x, v2.y, 0f);
		}
		public static string ToStringExt(this Vector2 v2, int digits = -1) {
			#if DEBUG
			{
				if (digits >= 0) {
					return "("+v2.x+","+v2.y+")";
				} else {
					string digitTag = "F" + digits.ToString();
					return "("+v2.x.ToString(digitTag)+", "+v2.y.ToString(digitTag)+")";
				}
			}
			#else
			{
				return "";
			}
			#endif
		}
	}
	
	public static class ExtensionMethodsForColor32 {
		public static int ToInt32(this Color32 c32) {
			return (c32.a << 24) | (c32.r << 16) | (c32.g << 8) | (c32.b << 0);
		}
		public static string ToStringHex(this Color32 c32) {
			return c32.ToInt32().ToString("X8");
		}
		public static string ToStringExt(this Color32 c32) {
			#if DEBUG
			{
				return "("+c32.r+","+c32.g+","+c32.b+"|"+c32.a+")";
			}
			#else
			{
				return "";
			}
			#endif
		}
	}
	
	public static class ExtensionMethodsForColor {
		public static Color Dim(this Color c, float newAlpha) {
			c.a = newAlpha;
			return c;
		}
		public static string ToStringHexRGB(this Color c) {
			return string.Format("{0:X2}{1:X2}{2:X2}", (int)Mathf.Clamp01(c.r * 255f), (int)Mathf.Clamp01(c.g * 255f), (int)Mathf.Clamp01(c.b * 255f));
		}
		public static string ToStringHex(this Color c) {
			return string.Format("{0}{1:X2}", c.ToStringHexRGB(), (int)Mathf.Clamp01(c.a * 255f));
		}
		public static string ToStringExt(this Color c) {
			#if DEBUG
			{
				return "("+c.r.ToString("F4")+","+c.g.ToString("F4") + ","+c.b.ToString("F4") + "|"+c.a.ToString("F4") + ")";
			}
			#else
			{
				return "";
			}
			#endif
		}
	}
	
	public static class ExtensionMethodsForInt {
		public static bool IsEven(this int val) { return (val & 1) == 0; }
		public static bool IsOdd(this int val) { return (val & 1) == 1; }
		public static Color32 ToColor32(this int val) {
			return new Color32((byte)((val >> 16) & 0xff), (byte)((val >> 8) & 0xff), (byte)((val >> 0) & 0xff), (byte)((val >> 24) & 0xff));
		}
	}

	public static class ExtensionMethodsForImage {
		public static IEnumerator CoroutineAlphaByCurve(this Image image, float duration, AnimationCurve curve) {
			float t = 0f;
			while (t <= 1f) {
				t += Time.deltaTime / duration;
				image.SetColorAlpha(curve.Evaluate(t));
				yield return null;
			}
		}
	}

	public static class ExtensionMethodsForTexture2D {
		public static Sprite ToSprite(this Texture2D texture) {
			return texture.ToSprite(ExtensionMethodsForVector2.halfUnit);
		}
		public static Sprite ToSprite(this Texture2D texture, Vector2 pivot) {
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot);
			sprite.name = texture.name;
			return sprite;
		}
	}
	
	public static class ExtensionMethodsForSprite {
		public static Sprite Crop(this Sprite sprite, Rect rectPx) {
			Sprite result = Sprite.Create(sprite.texture, rectPx, ExtensionMethodsForVector2.halfUnit, sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect); // force full rect mesh to prevent possible time waste on sprite analysis (for tight sprite meshes)
			result.name = sprite.name;
			return result;
		}
	}

	//public static class ExtensionMethodsForLanguageManager {
	//	public static bool IsDefault(this SmartLocalization.LanguageManager languageManager) {
	//		SmartLocalization.SmartCultureInfo systemLanguage = SmartLocalization.LanguageManager.Instance.CurrentlyLoadedCulture;
	//		Assert.IsNotNull(systemLanguage);
	//		return systemLanguage != null ? systemLanguage.languageCode.Substring(0, 2).Equals("en", System.StringComparison.OrdinalIgnoreCase) : false;
	//	}
	//}

	//public static class ExtensionMethodsForSmartCultureInfo {
	//	public static string LanguageCodeShortOrFallback(this SmartLocalization.SmartCultureInfo languageManager, string fallback = "en") {
	//		SmartLocalization.SmartCultureInfo systemLanguage = SmartLocalization.LanguageManager.Instance.CurrentlyLoadedCulture;
	//		return systemLanguage != null ? systemLanguage.languageCode.Substring(0, 2) : fallback;
	//	}
	//}

	public static class ExtensionMethodsForString {
		/// <summary>Localize this string using SmartLocalization plugin.</summary>
		public static string L(this string key) {
			return key;// SmartLocalization.LanguageManager.Instance.GetTextValue(key);
		}

		/// <summary>Localize this string using SmartLocalization plugin and use it in string.Format call with function arguments.</summary>
		public static string L(this string key, params object[] args) {
			return key;// string.Format(SmartLocalization.LanguageManager.Instance.GetTextValue(key), args);
		}

		public static Sprite LoadMultiresSprite(this string str) {
			return str.LoadMultires<Sprite>();
		}
		public static Sprite LoadResSprite(this string str) {
			return str.LoadRes<Sprite>();
		}
		
		public static T LoadMultires<T>(this string str) where T : UnityEngine.Object {
			return Resources.Load<T>(Multires.Folder.ToString() + "/" + str);
		}
		public static T LoadRes<T>(this string str) where T : UnityEngine.Object {
			return Resources.Load<T>(str);
		}
		
		public static T[] LoadAllMultires<T>(this string str) where T : UnityEngine.Object {
			return Resources.LoadAll<T>(Multires.Folder.ToString() + "/" + str);
		}
		public static T[] LoadAllRes<T>(this string str) where T : UnityEngine.Object {
			return Resources.LoadAll<T>(str);
		}
	}

	public static class ExtensionMethodsForFloat {
		public static float Scatter(this float f, float radius) {
			return f + (radius == 0f ? 0f : UnityEngine.Random.Range(-1f, 1f) * radius);
		}
		public static bool IsZeroNearly(this float f) {
			return Mathf.Abs(f) < 1e-6;
		}
	}

	public static class ExtensionMethodsForArrays {
		public static void SetEvery<T>(this T[] arr, System.Func<T> setter) {
			for (int l = arr.Length, i = 0; i < l; i++) {
				arr[i] = setter();
			}
		}

		public static void SetEvery<T>(this T[] arr, System.Func<int, T> setter) {
			for (int l = arr.Length, i = 0; i < l; i++) {
				arr[i] = setter(i);
			}
		}
	}

	public static class ExtensionMethodsForDelegate {
		public static string MethodFullName(this System.Delegate method) {
			ParameterInfo[] methodParams = method.Method.GetParameters();
			string paramsString = "(";
			for (int i = 0; i < methodParams.Length - 1; i++) {
				paramsString = System.String.Concat(paramsString, methodParams[i].ParameterType, ",");
			}
			paramsString = System.String.Concat(paramsString, methodParams[methodParams.Length - 1].ParameterType, ")");
			string methodName = System.String.Concat(method.Method.ReflectedType.Namespace, ".",
												method.Method.ReflectedType.Name, ".",
												method.Method.Name,
												paramsString);
			return methodName;
		}
	}



}
