using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using CT;
using CT.DEBUG;
using System.Reflection;

namespace FenceShooter {

	using ActionEvent = UnityAction<BaseEventData>;
	using ActionEventDictionary = Dictionary<System.Delegate, UnityAction<BaseEventData>>;
	using ActionEventTriggerDictionary = Dictionary<EventTriggerType, Dictionary<System.Delegate, UnityAction<BaseEventData>>>;

	public class PointerEventTrigger : EventTrigger {
		public delegate void PointerEventDelegate(PointerEventData data);
		private ActionEventTriggerDictionary dict;

		//string MethodName {
		//	get {
		//		MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
		//		string methodName = method.Name;
		//		string className = method.ReflectedType.Name;
		//		return className + "." + methodName;
		//	}
		//}
		void Awake() {
			Utils.Log("EventTriggerList.Awake " + GetType());
			dict = new ActionEventTriggerDictionary();
			//VoidDelegate voidDelegate = Awake.;

		}

		void Start() {
			//AddEvent(EventTriggerType.PointerDown, PassRaycast);
		}

		void PassRaycast(PointerEventData data) {
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(camRay, out hit, 100f)) {
				LOG.Write(hit.collider.ToString());
			}
		}

		public void RemoveAllListeners() {
			foreach (var item in triggers) {
				item.callback.RemoveAllListeners();
			}
			triggers.Clear();
			foreach (var item in dict) {
				item.Value.Clear();
			}
			dict.Clear();
		}


		public void RemoveEvent(EventTriggerType type, PointerEventDelegate action) {
			foreach (var item in triggers) {
				if (item.eventID == type) {
					ActionEventDictionary typeEvents;
					if (dict.TryGetValue(type, out typeEvents)) {
						ActionEvent actionListener;
						string methodName = action.MethodFullName();
						if (typeEvents.TryGetValue(action, out actionListener)) {
							item.callback.RemoveListener(actionListener);
							typeEvents.Remove(action);
							Utils.Log("successfully removed " + methodName + " from typeEvents");
						}
					}
				}
			}
		}
		public void AddEvent(EventTriggerType type, PointerEventDelegate action) {
			//type = EventTriggerType.BeginDrag;
			//LOG.Write("AddEvent1 " + action.ToString());
			//LOG.Write("AddEvent2 " + action.Method.ReflectedType.Name);
			//LOG.Write("AddEvent3 " + action.Method.Name);
			//LOG.Write("AddEvent4 " + action.Method.ToString());
			//LOG.Write("AddEvent5 " + action.Method.ReflectedType.Namespace);
			//LOG.Write("AddEvent6 " + action.Method.ReturnType.Name);
			//VoidDelegate del = AddEvent;

			string methodName = action.MethodFullName();
			LOG.Write("AddEvent " + methodName);
			//LOG.Write("AddEvent action.Method.ToString()=" + action.Method.ToString());
			ActionEventDictionary typeEvents;
			if (!dict.ContainsKey(type)) {
				typeEvents = new ActionEventDictionary();
				dict.Add(type, typeEvents);
			} else if (dict.TryGetValue(type, out typeEvents)) {
				if (typeEvents != null && typeEvents.ContainsKey(action)) {
					if (typeEvents[action] != null) {
						Utils.Log("typeEvents already contain " + methodName + " for " + type);
						return;
					} else {
						typeEvents.Remove(action);
					}
				}
			}

			ActionEvent actionListener = new ActionEvent((data) => { action((PointerEventData)data); });
			foreach (var item in triggers) {
				if (item.eventID == type) {
					item.callback.AddListener(actionListener);
					typeEvents.Add(action, actionListener);
					return;
				}
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = type;
			//entry.callback.AddListener((data) => { action((PointerEventData)data); });
			entry.callback.AddListener(actionListener);
			typeEvents.Add(action, actionListener);
			triggers.Add(entry);
		}

	}
}
