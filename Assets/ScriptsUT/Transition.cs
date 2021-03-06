// <auto-generated>
//------------------------------------------------------------------------------
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility {
	public enum TransitionProperty {
		Position,
		RotationEulerAngles,
		Scale,
		Alpha
	}

	public class TransitionParams : MonoBehaviour {
		public float startScale;
		public float destScale;
		public float startAlpha;
		public float destAlpha;
		public Vector3 startRotationEulerAngles;
		public Vector3 destRotationEulerAngles;
		public Vector3 startPos;
		public Vector3 destPos;
		public Vector3 shiftRay;
		public float shiftLength;
	}

	public class Transition {

		public TransitionProperty property;
		public TransitionType type;
		public GameObject[] gameObjects;

		int n = 2;
		int m = 2;
		int N = 2;
		int M = 2;

		public int power = 2;

		float inflectionPt0 = 0.5f;
		float inflectionPt1 = 0.5f;

		float xMax, yMax;

		public void SetAsymNormalisedData(int asymPow1, int asymPow2, float inflPoint) {
			this.n = asymPow1;
			this.m = asymPow2;
			this.inflectionPt0 = inflPoint;
		}

		public void SetSymmInflectedData(int symmPow1, int symmPow2, float inflPoint) {
			this.n = symmPow1;
			this.m = symmPow2;
			this.inflectionPt0 = inflPoint;
		}

		public void SetAsymInflectedData(int N, int M, int n, int m,
										 float xMax, float yMax, float inflPtNorm0, float inflPtNorm1) {
			this.N = N;
			this.M = M;
			this.n = n;
			this.m = m;
			this.xMax = xMax;
			this.yMax = yMax;
			this.inflectionPt0 = inflPtNorm0;
			this.inflectionPt1 = inflPtNorm1;
		}

		public void SetAsymPolynom32Data(int asymPow1, int asymPow2) {
			this.n = asymPow1;
			this.m = asymPow2;
		}

		public Transition(TransitionProperty type, bool addTransitionParams, params GameObject[] gObjects) {
			this.property = type;
			gameObjects = new GameObject[gObjects.Length];
			for (int i = 0; i < gObjects.Length; i++) {
				gameObjects[i] = gObjects[i];
			}
			if (addTransitionParams) {
				for (int i = 0; i < gObjects.Length; i++) {
					gameObjects[i].AddComponent<TransitionParams>();
				}
			}
		}

		public Transition(TransitionType type) {
			this.type = type;
		}

		public float GetTransition(float timePassed, float duration) {
			switch (type) {

				case TransitionType.SYMMETRIC_INFLECTED:
				return Maths.GetTransitionSymmInflected(timePassed, duration, inflectionPt0, n, m);
				case TransitionType.ASYMETRIC_INFLECTED:
				return Maths.GetTransitionAsymInflected(timePassed, duration, N, M, n, m,
														xMax, yMax, inflectionPt0, inflectionPt1);
				case TransitionType.ASYMETRIC_NORMALISED:
				return Maths.GetTransitionAsymNormalised(timePassed, duration, inflectionPt0, n, m);
				default:
				return Maths.GetTransition(type, timePassed, duration, power);
			}
		}

		public void Update(float timePassed, float duration) {
			float shift = GetTransition(timePassed, duration);
			TransitionParams tp;
			switch (property) {
				case TransitionProperty.Position:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					go.transform.localPosition = tp.startPos + tp.shiftRay * (tp.shiftLength * shift);
				}
				break;
				case TransitionProperty.RotationEulerAngles:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					Vector3 eulerAngles = Vector3.Lerp(tp.startRotationEulerAngles, tp.destRotationEulerAngles, shift);
					go.transform.localRotation = Quaternion.Euler(eulerAngles);
				}
				break;
				case TransitionProperty.Scale:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					float scale = tp.startScale + (tp.destScale - tp.startScale) * shift;
					go.transform.localScale = new Vector3(scale, scale, 1.0f);
				}
				break;
				case TransitionProperty.Alpha:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					float alpha = tp.startAlpha + (tp.destAlpha - tp.startAlpha) * shift;
					Utils.SetSpriteAlpha(go, alpha);
				}
				break;
				default:
				break;
			}
		}

		public void Finish() {
			TransitionParams tp;
			switch (property) {
				case TransitionProperty.Position:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					go.transform.localPosition = tp.destPos;
				}
				break;
				case TransitionProperty.RotationEulerAngles:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					go.transform.localRotation = Quaternion.Euler(tp.destRotationEulerAngles);
				}
				//Utils.Log("transition finished "+Utils.GetVector3String(destRotationEulerAngles));
				break;
				case TransitionProperty.Scale:
				float scale;
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					if (type == TransitionType.SIN_IN_PI_RANGE)
						scale = tp.startScale;
					else
						scale = tp.destScale;
					go.transform.localScale = new Vector3(scale, scale, 1.0f);
				}
				break;
				case TransitionProperty.Alpha:
				foreach (GameObject go in gameObjects) {
					tp = go.GetComponent<TransitionParams>();
					Utils.SetSpriteAlpha(go, tp.destAlpha);
				}
				break;
				default:
				break;
			}
		}
	}





	public class ObjectTransition {
		public delegate void TransitionFinishedAction();
		public TransitionFinishedAction finishingAction;

		Dictionary<TransitionProperty, Transition> propTransDict;
		Transition[] transitions;

		public float duration;
		float startTime;

		public bool finished;

		float waitAfterFinishTime = 0.0f;
		bool drawPlot = false;
		public bool drawTransitionPlot {
			get {
				return drawPlot;
			}
			set {
				plotPoints = new Vector3[plotPointsMaxNumber];
				plotPointCounter = 0;
				plotPoints[plotPointCounter++] = Vector3.zero;
				drawPlot = value;
			}
		}


		int plotPointCounter;
		const int plotPointsMaxNumber = 200;
		Vector3[] plotPoints;



		public ObjectTransition(params GameObject[] gObject) {

			propTransDict = new Dictionary<TransitionProperty, Transition>();
			var keys = Enum.GetValues(typeof(TransitionProperty));

			for (int i = 0; i < gObject.Length; i++) {
				gObject[i].AddComponent<TransitionParams>();
			}
			transitions = new Transition[keys.Length];
			foreach (TransitionProperty key in keys) {
				transitions[(int)key] = new Transition(key, false, gObject);
			}

		}

		public void ClearPropsDictionary() {
			propTransDict.Clear();
		}


		public void DrawPlot() {
			for (int i = 0; i < plotPointCounter - 1; i++) {
				UnityEngine.Debug.DrawLine(plotPoints[i], plotPoints[i + 1]);
			}
		}

		public void Start(float duration, float waitAfterFinishTime) {
			this.startTime = Time.time;
			this.duration = duration;
			this.waitAfterFinishTime = waitAfterFinishTime;
			finished = false;

			if (drawTransitionPlot) {
				plotPoints = new Vector3[plotPointsMaxNumber];
				plotPointCounter = 0;
				plotPoints[plotPointCounter++] = Vector3.zero;
			}
		}

		public void AddPropertyToDictionary(TransitionProperty property) {
			bool containsKey = propTransDict.ContainsKey(property);
			if (!containsKey) {
				Transition transition = transitions[(int)property];
				propTransDict.Add(property, transition);
			}
		}

		public void SetStartAndDestRotationEulerAngles(GameObject go, Vector3 startEulerAngles, Vector3 destEulerAngles) {
			TransitionProperty property = TransitionProperty.RotationEulerAngles;
			Transition transition = transitions[(int)TransitionProperty.RotationEulerAngles];
			TransitionParams td = go.GetComponent<TransitionParams>();

			td.startRotationEulerAngles = startEulerAngles;
			td.destRotationEulerAngles = destEulerAngles;

			bool containsKey = propTransDict.ContainsKey(property);
			if (startEulerAngles == destEulerAngles) {
				if (containsKey) {
					propTransDict.Remove(property);
				}
			} else {
				if (!containsKey && transition != null) {
					propTransDict.Add(property, transition);
				}
			}
		}

		public void SetStartAndDestPosition(GameObject go, Vector3 start, Vector3 dest) {
			//TransitionProperty property = TransitionProperty.Position;
			Transition transition = transitions[(int)TransitionProperty.Position];
			TransitionParams tp = go.GetComponent<TransitionParams>();

			tp.gameObject.transform.localPosition = start;
			tp.startPos = start;
			tp.destPos = dest;
			tp.shiftRay = tp.destPos - tp.startPos;
			tp.shiftLength = tp.shiftRay.magnitude;

			bool containsKey = propTransDict.ContainsKey(TransitionProperty.Position);
			if (tp.shiftLength > 1E-05f) {
				tp.shiftRay.Normalize();
				if (!containsKey && transition != null) {
					propTransDict.Add(TransitionProperty.Position, transition);
				}
			} else {
				tp.shiftRay = Vector3.zero;
				tp.shiftLength = 0;
				if (containsKey && transition.gameObjects.Length == 1) {
					propTransDict.Remove(TransitionProperty.Position);
				}
			}
		}

		public void SetStartAndDestProperty(GameObject go, TransitionProperty property, float start, float dest) {
			if (property == TransitionProperty.Position ||
				property == TransitionProperty.RotationEulerAngles)
				return;
			Transition transition = null;
			TransitionParams tp = go.GetComponent<TransitionParams>();

			switch (property) {
				case TransitionProperty.Scale:
				transition = transitions[(int)TransitionProperty.Scale];
				tp.startScale = start;
				tp.destScale = dest;
				tp.gameObject.transform.localScale = new Vector3(start, start, 1.0f);
				break;
				case TransitionProperty.Alpha:
				transition = transitions[(int)TransitionProperty.Alpha];
				tp.startAlpha = start;
				tp.destAlpha = dest;
				Utils.SetSpriteAlpha(tp.gameObject, start);
				break;
				default:
				break;
			}
			bool containsKey = propTransDict.ContainsKey(property);
			if (start == dest) {
				if (containsKey && transition.gameObjects.Length == 1) {
					propTransDict.Remove(property);
				}
			} else {
				if (!containsKey && transition != null) {
					propTransDict.Add(property, transition);
				}
			}
		}



		public void SetTransitionPropertyType(TransitionProperty property, TransitionType type, int power) {
			Transition transition = transitions[(int)property];
			if (transition != null) {
				transition.type = type;
				transition.power = power;
			}
		}
		public void SetTransitionPropertyAsymType(TransitionProperty property, TransitionType asymType, params float[] plist) {
			Transition transition = transitions[(int)property];
			if (transition != null) {
				transition.type = asymType;
				switch (asymType) {
					case TransitionType.ASYMETRIC_INFLECTED:
					transition.SetAsymInflectedData((int)plist[0], (int)plist[1], (int)plist[2], (int)plist[3],
														plist[4], plist[5], plist[6], plist[7]);
					break;
					case TransitionType.ASYMETRIC_NORMALISED:
					transition.SetAsymNormalisedData((int)plist[0], (int)plist[1], plist[2]);
					break;
					case TransitionType.SYMMETRIC_INFLECTED:
					transition.SetSymmInflectedData((int)plist[0], (int)plist[1], plist[2]);
					break;

					default:
					break;
				}

			}
		}

		//		IEnumerator PerformFinifhingActionWithDelay(float delay) {
		//			yield return new WaitForSeconds (delay);
		//			finishingAction ();
		//			yield break;
		//		}

		public void Update() {
			float timePassed = Time.time - startTime;
			Update(timePassed, duration);
		}

		public void Update(float timePassed, float duration) {

			float plotShift = 0.0f;

			//			Utils.Log("propTransDict.Count="+propTransDict.Count);
			if (!finished && timePassed < duration) {
				foreach (KeyValuePair<TransitionProperty, Transition> pair in propTransDict) {
					Transition transition = pair.Value;
					transition.Update(timePassed, duration);
				}
			} else {
				if (!finished) {
					foreach (KeyValuePair<TransitionProperty, Transition> pair in propTransDict) {
						Transition transition = pair.Value;
						transition.Finish();
					}
				}
				finished = true;
				plotShift = 1.0f;
				// delegate result method
				if (timePassed - duration >= waitAfterFinishTime && finishingAction != null) {
					finishingAction();
				}
			}

			if (drawTransitionPlot && plotPointCounter < plotPoints.Length) {
				plotPoints[plotPointCounter++] = new Vector3(timePassed, plotShift, 0.0f);
				DrawPlot();
			}

			//			Utils.Log(Utils.GetVector3String(pos));
		}

	}
}

