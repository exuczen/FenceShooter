using Utility;
using Utility.Debug;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace Utility {
	/// <summary>Use to apply alpha to color (tint) of every SpriteRenderer and UI.Graphic in this object children.
	/// Child SpriteRenderers/Graphics are detected on Awake, if object changes use InitFader method to detect them again.
	/// <para>WARNING: existing alpha values are overridden by this component alpha value which varies [0,1]! Relative alpha values of sprites are not preserved!</para></summary>
	[ExecuteInEditMode, DisallowMultipleComponent]
	public class ObjectTreeFaderComponent : MonoBehaviour {
		[Range(0f, 1f), Tooltip("Animate this value to create fade in/fade out effect")]
		public float alpha;
		private float alphaPrevious;

		private SpriteRenderer[] controlledSpriteRenderers;
		private UnityEngine.UI.Graphic[] controlledUIGraphics;
		
		public void InitFader() {
			var srList = new List<SpriteRenderer>();
			GetComponents(srList);
			GetComponentsInChildren(srList);
			controlledSpriteRenderers = srList.ToArray();
			var grList = new List<Graphic>();
			GetComponents(grList);
			GetComponentsInChildren(grList);
			controlledUIGraphics = grList.ToArray();
			SetAlpha(alpha);
		}

		private void SetAlpha(float newAlpha) {
			// change alpha of all child sprite renderers to alpha value set in this component
			// WARNING code below ignores alpha set in sprite originally, to preserve whole object visual look,
			// we should scale alpha to fit between original value of particular sprite and zero
			for (int l = controlledSpriteRenderers.Length, i = 0; i < l; i++) {
				controlledSpriteRenderers[i].SetColorAlpha(newAlpha);
			}
			for (int l = controlledUIGraphics.Length, i = 0; i < l; i++) {
				controlledUIGraphics[i].SetColorAlpha(newAlpha);
			}
		}

		/// <summary>NOTE: Unity event</summary>
		private void Awake() {
			#if LOG_VERBOSE
				// LOG.Write(name + " : SpriteTreeFaderComponent : Awake");
			#endif
			InitFader();
		}

		/// <summary>NOTE: Unity event</summary>
		private void Update() {
			if (alphaPrevious != alpha) {
				#if LOG_VERBOSE
					// LOG.Write(name + " : SpriteTreeFaderComponent : Update");
				#endif
				alphaPrevious = alpha;
				SetAlpha(alpha);
			}
		}
	}
}
