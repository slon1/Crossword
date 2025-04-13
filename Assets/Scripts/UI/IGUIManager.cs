using System.Collections.Generic;

public interface IGUIManager {	
	void Initialize();
	void ShowPanel(PanelId panelId);
	void ShowPanelModal(PanelId panelId, bool show);
	void Back();
	void Execute<T>(PanelId  panelId, PageActionId action, T param );
	void Execute(PanelId panelId, PageActionId action);
	
};
