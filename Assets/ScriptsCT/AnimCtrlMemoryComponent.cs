using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CT {
	/// <summary>
	/// Attach to object with animator component. Use only with "AppAnimCtrlMachineBehaviour".
	/// NOTE lack of layer support (one layer per animator supported)
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class AnimCtrlMemoryComponent : MonoBehaviour {
		[HideInInspector]
		private int stateFullNamehashPrevious;
		[HideInInspector]
		private int stateFullNamehashCurrent;
		
		/// <summary>If called on each entered state, will maintain (previous,current) pair of states in this object fields.
		/// WARNING: after first call, previous state is 0, thus its a special hash not to be used</summary>
		public void StateNotifyCurrent(int fullNamehash) {
			stateFullNamehashPrevious = stateFullNamehashCurrent;
			stateFullNamehashCurrent = fullNamehash;
		}
		
		public bool Is(int stateNamehash) { return stateFullNamehashCurrent == stateNamehash; }
		public bool Was(int stateNamehash) { return stateFullNamehashPrevious == stateNamehash; }
		public bool WasDefault { get { return stateFullNamehashPrevious == 0; } }

		/// <summary>NOTE: Unity event</summary>
		public void OnDisable() {
			stateFullNamehashPrevious = 0;
			stateFullNamehashCurrent = 0;
		}

		public string ToStringExt() { return "(" + CT.DEBUG.Namehash.DBGet(stateFullNamehashCurrent) + ",p:" + CT.DEBUG.Namehash.DBGet(stateFullNamehashPrevious) + ")"; }

		public override string ToString() { return CT.DEBUG.Namehash.DBGet(stateFullNamehashCurrent); }
	}
}
