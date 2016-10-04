using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CT {
	/// <summary>Add this as behaviour to animation controller (all states, by clicking on blank animator canvas in editor).
	/// This behaviour adds memory: easy access to pair (previous state, current state).
	/// The pair is stored in AppAnimCtrlMemoryComponent that must exist in Animator component parent game object.</summary>
	public class AnimCtrlMemoryBehaviour : StateMachineBehaviour {
		private AnimCtrlMemoryComponent memory;

		/// <summary>Registers state being entered as current.</summary>
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			#if LOG_VERBOSE
				// LOG.Write("AppAnimCtrlMemoryBehaviour.OnStateEnter : name = " + animator.name + ", state = " + CT.DEBUG.Namehash.DBGet(stateInfo.fullPathHash));
			#endif
			if (memory == null) {
				memory = animator.GetComponent<AnimCtrlMemoryComponent>();
			}
			Assert.IsNotNull(memory);
			Assert.IsFalse(memory.Is(stateInfo.fullPathHash));
			memory.StateNotifyCurrent(stateInfo.fullPathHash);
		}
	}
}
