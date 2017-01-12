using Utility;
using Utility.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace Utility {
	/// <summary>Suicidal behaviour added to state causes parent game object to be destroyed on state enter</summary>
	public class ParenticidalBehaviour : StateMachineBehaviour {
		private bool done;
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			#if LOG_VERBOSE
				// LOG.Write(animator.gameObject.name + " : ParenticidalBehaviour.OnStateUpdate"/* + ": " + animator.name + ", state: " + stateInfo*/);
			#endif
			if (!done) {
				App.StartCoroutine(KillCoroutine(animator.transform.parent.gameObject));
			}
		}
		private IEnumerator KillCoroutine(GameObject obj) {
			yield return new WaitForEndOfFrame();
			Destroy(obj);
			done = true;
		}
	}
}
