using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GUIManager : MonoBehaviour, IGUIManager {
	
	private Dictionary<PanelId, IPage> panels;
	[SerializeField]
	private List<ScrAbs> screens;

	private void Start() {
		Initialize();
	}
	public void Initialize() {
		panels = screens.ToDictionary(panel => panel.PanelID, panel => (IPage)panel);
		EventBus.Bus.AddListener<ButtonId>(EventId.Menu, OnMenuEvent);
	}

	private void OnMenuEvent(ButtonId id) {
		switch (id) {
			case ButtonId.Game:
				ShowPanel(PanelId.Game);
				EventBus.Bus.Invoke(EventId.Game);
				break;			
			
			case ButtonId.Menu:
				ShowPanel(PanelId.Menu);
				
				break;
			case ButtonId.Settings:
				ShowPanel(PanelId.Settings);
				break;

			case ButtonId.SoundOn:
				Execute(PanelId.Settings, PageActionId.SoundOff);
				break;
			case ButtonId.SoundOff:
				Execute(PanelId.Settings, PageActionId.SoundOn);
				break;
			case ButtonId.Next:
				//Execute(PanelId.Settings, PageActionId.SoundOn);
				EventBus.Bus.Invoke(EventId.Game);
				break;
			default:
				break;
		}
	}

	public void Back() {
		//Installer.GetService<IGameManager>().ShowPanel(lastOpen[lastOpen.Count-2]);

	}
	public void ShowPanelModal(PanelId panelId, bool show) {
		if (show) {
			panels[panelId].Show();
		}
		else {
			panels[panelId].Hide();
		}



	}
	public void ShowPanel(PanelId panelId) {
		foreach (var panel in panels.Values) {
			if (panel.IsStatic()) {
				continue;
			}
			if (panel.PanelID == panelId) {
				panel.Show();
			}
			else {
				panel.Hide();
			}
		}

	}

	private void OnDestroy() {
		EventBus.Bus.RemoveListener<ButtonId>(EventId.Menu, OnMenuEvent);
		
		panels = null;		
	}


	public void Execute<T>(PanelId panelId, PageActionId action, T param) {
		panels[panelId].Execute(action, param);
	}


	public void Execute(PanelId panelId, PageActionId action) {
		panels[panelId].Execute(action);
	}

	
}
