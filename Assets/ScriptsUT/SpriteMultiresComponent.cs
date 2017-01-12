using Utility;
using Utility.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Utility {
	[RequireComponent(typeof(SpriteRenderer)), DisallowMultipleComponent]
	public class SpriteMultiresComponent : MonoBehaviour {
		/// <summary>NOTE: Unity event</summary>
		public void Awake() {
			if (!Multires.IsDefault) {
				GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite.name.LoadMultiresSprite();
			}
		}
	}
}