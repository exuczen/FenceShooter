using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CT {
	public class DEBUGAnimStateBehaviour : StateMachineBehaviour {
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			#if LOG_VERBOSE
				LOG.Write("DEBUGAnimStateBehaviour.OnStateEnter: " + animator.name + ", state: " + CT.DEBUG.Namehash.DBGet(stateInfo.fullPathHash));
			#endif
		}
		
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			#if LOG_VERBOSE
				LOG.Write("DEBUGAnimStateBehaviour.OnStateExit: " + animator.name + ", state: " + CT.DEBUG.Namehash.DBGet(stateInfo.fullPathHash));
			#endif
		}
	}
}
