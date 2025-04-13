
// GameManager
using System;
using UnityEngine.UI;
namespace GameUI {

	[Serializable]
	public class EventButton {
		public ButtonId ButtonId;
		public Button Button;
		public void InitEvent() {
			Button.onClick.AddListener(() => EventBus.Bus.Invoke(EventId.Menu, ButtonId));
		}

	}
}