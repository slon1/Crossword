using System;
using System.Collections.Generic;
using UnityEditor;


public class EventBus : IDisposable {

	private static readonly Lazy<EventBus> lazy = new Lazy<EventBus>(() => new EventBus());
	public static EventBus Bus => lazy.Value;


	private readonly Dictionary<EventId, Delegate> eventHandlers = new();


	public void AddListener(EventId eventId, Action handler) {
		if (!eventHandlers.ContainsKey(eventId)) {
			eventHandlers[eventId] = null;
		}
		eventHandlers[eventId] = (Action)eventHandlers[eventId] + handler;
	}

	public void AddListener<T>(EventId eventId, Action<T> handler) {
		if (!eventHandlers.ContainsKey(eventId)) {
			eventHandlers[eventId] = null;
		}
		eventHandlers[eventId] = (Action<T>)eventHandlers[eventId] + handler;
	}


	public void RemoveListener(EventId eventId, Action handler) {
		if (eventHandlers.ContainsKey(eventId)) {
			eventHandlers[eventId] = (Action)eventHandlers[eventId] - handler;
		}
	}


	public void RemoveListener<T>(EventId eventId, Action<T> handler) {
		if (eventHandlers.ContainsKey(eventId)) {
			eventHandlers[eventId] = (Action<T>)eventHandlers[eventId] - handler;
		}
	}


	public void Invoke(EventId eventId) {
		if (eventHandlers.TryGetValue(eventId, out var handler) && handler is Action action) {
			action.Invoke();
		}
	}


	public void Invoke<T>(EventId eventId, T param) {
		if (eventHandlers.TryGetValue(eventId, out var handler) && handler is Action<T> action) {
			action.Invoke(param);
		}
	}


	public void RemoveAllListeners(EventId eventId) {
		if (eventHandlers.ContainsKey(eventId)) {
			eventHandlers[eventId] = null;
		}
	}


	public void Dispose() {
		List<EventId> keys=new List<EventId>(eventHandlers.Keys);
		foreach (var key in keys) {
			eventHandlers[key] = null;
		}
		eventHandlers.Clear();
	}


#if UNITY_EDITOR
	[InitializeOnLoadMethod]
	private static void InitializeEditorSupport() {
		EditorApplication.playModeStateChanged += state => {
			if (state == PlayModeStateChange.ExitingPlayMode) {
				Bus.Dispose();
			}
		};
	}
#endif
}
