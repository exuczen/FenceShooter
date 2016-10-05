using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CT
{
	[RequireComponent(typeof(Animator), typeof(AnimCtrlMemoryComponent)), DisallowMultipleComponent]
	public class AppSingleton : MonoBehaviour
	{
		public GameObject world;

		/// <summary>NOTE: Unity event</summary>
		private void Awake()
		{
#if LOG_VERBOSE
			LOG.PrintStartupInfo();
#endif
			App.AwakeOwn(this);
			// uncomment to disable "editor only" objects on run (useful if sometimes those objs interfere visually with level and make designing harder)
			//#if UNITY_EDITOR
			//{
			//	var editorOnlyGOs = GameObject.FindGameObjectsWithTag("EditorOnly");
			//	foreach (var go in editorOnlyGOs) {
			//		go.SetActive(false);
			//	}
			//}
			//#endif
		}

		/// <summary>NOTE: Unity event</summary>
		private void OnApplicationFocus(bool focusStatus)
		{
#if LOG_VERBOSE
			LOG.Write("App.OnApplicationFocusOwn : focusStatus = " + focusStatus);
#endif
			App.OnApplicationFocusOwn(focusStatus);
		}
		/// <summary>NOTE: Unity event</summary>
		private void OnApplicationPause(bool pauseStatus)
		{
#if LOG_VERBOSE
			LOG.Write("App.OnApplicationPause : pauseStatus = " + pauseStatus);
#endif
		}
		/// <summary>NOTE: Unity event</summary>
		private void OnApplicationQuit()
		{
#if LOG_VERBOSE
			LOG.Write("App.OnApplicationQuit");
#endif
		}

	}

	/// <summary>App state is state of animator attached to main game object (camera, player avatar).
	/// Aside: why static class? Because Unity itself uses those for "global" tasks like for example Input.
	/// We also will use this pattern for global tasks. Its equivalent to singleton but easier to use.
	/// There are no drawbacks other than lack of flexibility (unit tests would be harder), but we're
	/// writing small game and we need easier usage more than flexibility.</summary>
	public static class App
	{
		public static AppSingleton obj;
		public static Transform Transform { get { return obj.transform; } }
		public static Animator Animator { get; private set; }
		public static AnimCtrlMemoryComponent State { get; private set; }

		public static bool WasInputNeutral { get; set; }
		public static bool WaitForInputNeutral { get; set; }

		public static bool IsHelloTowerDialogToBeShown { get; set; }

		public static bool IsLandingLevel { get; set; }
		public static bool IsHeadstart { get; set; }
		public static bool IsTutorial { get; set; }

		public static GameObject World { get { return obj.world; } }

		/// <summary>DEBUG ONLY</summary>

		/// <summary>Use in "yield return" to wait until "awaiting signal" state is entered. Exits immediately if already in "awaiting signal" state.</summary>
		public static CustomYieldInstruction WaitForStateAwaitingSignalEntry { get; private set; }
		/// <summary>Use in "yield return" to wait until "awaiting signal" state is exited. Exits immediately if already in state other than "awaiting signal".</summary>
		public static CustomYieldInstruction WaitForStateAwaitingSignalExit { get; private set; }

		public static void AwakeOwn(AppSingleton appComponent)
		{
		}

		/// <summary>When app loses focus and situation allows (not prep/intro), we save app state (map/tower)</summary>
		public static void OnApplicationFocusOwn(bool isFocused)
		{
			if (App.State == null)
			{
				return; // it's possible this is called before Awake
			}
			bool lostFocus = !isFocused;
			bool allowedAppState = !(App.State.Is(App.Namehash.StatePrep) || App.State.Is(App.Namehash.StateIntro));
			if (lostFocus && allowedAppState)
			{
				//save app state
			}
		}



		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return App.obj.StartCoroutine(coroutine);
		}
		public static void StopCoroutine(Coroutine coroutine)
		{
			App.obj.StopCoroutine(coroutine);
		}


		/// <summary>Application "save" module with saving/restoring related stuff</summary>

		/// <summary>Cached namehashes of animator states and triggers. NOTE: states need full name including layer.</summary>
		public static class Namehash
		{
			public static int TriggerPrepEnded = Animator.StringToHash("PrepEnded");
			public static int TriggerIntroEndedLetsSeeTower = Animator.StringToHash("IntroEndedLetsSeeTower");
			public static int TriggerIntroEndedLetsSeeMap = Animator.StringToHash("IntroEndedLetsSeeMap");
			public static int TriggerIntroEndedDoFailShortcut = Animator.StringToHash("IntroEndedDoFailShortcut");
			public static int TriggerSignalReceived = Animator.StringToHash("SignalReceived");
			public static int TriggerNewLevelFail = Animator.StringToHash("NewLevelFail");
			public static int TriggerReadyForNextLevel = Animator.StringToHash("ReadyForNextLevel");
			public static int TriggerEnterLevelDestruction = Animator.StringToHash("EnterLevelDestruction");
			public static int TriggerStartCameraMovement = Animator.StringToHash("StartCameraMovement");
			public static int TriggerGameOver = Animator.StringToHash("GameOver");
			public static int StatePrep = Animator.StringToHash("Base Layer.Prep");
			public static int StateIntro = Animator.StringToHash("Base Layer.Intro");
			public static int StateAwaitingSignal = Animator.StringToHash("Base Layer.AwaitingSignal");
			public static int StateLandingLevel = Animator.StringToHash("Base Layer.LandingLevel");
			public static int StateDestroyingLevels = Animator.StringToHash("Base Layer.DestroyingLevels");
			public static int StateFlyingCamera = Animator.StringToHash("Base Layer.FlyingCamera");
			public static int StateFail = Animator.StringToHash("Base Layer.Fail");
			public static int StateMap = Animator.StringToHash("Base Layer.Map");
			static Namehash()
			{
				CT.DEBUG.Namehash.DBAdd(typeof(Namehash));
			}
		}

		public static class NamehashStandard
		{
			public static int TriggerShow = Animator.StringToHash("Show");
			public static int TriggerHide = Animator.StringToHash("Hide");
			public static int StateHidden = Animator.StringToHash("Base Layer.Hidden");
			public static int StateShowing = Animator.StringToHash("Base Layer.Showing");
			public static int StateVisible = Animator.StringToHash("Base Layer.Visible");
			public static int StateHiding = Animator.StringToHash("Base Layer.Hiding");
		}
	}
}