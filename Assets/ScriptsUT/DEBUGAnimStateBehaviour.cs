using Utility;
using Utility.Debug;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Utility {
	public class DebugAnimStateBehaviour : StateMachineBehaviour {
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
#if LOG_VERBOSE
			Log.Write("DEBUGAnimStateBehaviour.OnStateEnter: " + animator.name + ", state: " + Namehash.DBGet(stateInfo.fullPathHash));
			#endif
		}
		
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
#if LOG_VERBOSE
			Log.Write("DEBUGAnimStateBehaviour.OnStateExit: " + animator.name + ", state: " + Namehash.DBGet(stateInfo.fullPathHash));
			#endif
		}
	}
}
