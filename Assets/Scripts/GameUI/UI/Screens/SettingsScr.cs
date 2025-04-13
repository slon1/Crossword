using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameUI {
	public class SettingsScr : ScrAbs {


		public override void Execute(PageActionId action) {
			base.Execute(action);

			if (action == PageActionId.SoundOn) {
				GetButton(ButtonId.SoundOff).gameObject.SetActive(false);
				GetButton(ButtonId.SoundOn).gameObject.SetActive(true);
				EventBus.Bus.Invoke(EventId.Sound, true);

			}
			if (action == PageActionId.SoundOff) {
				GetButton(ButtonId.SoundOff).gameObject.SetActive(true);
				GetButton(ButtonId.SoundOn).gameObject.SetActive(false);
				EventBus.Bus.Invoke(EventId.Sound, false);
			}
		}
	}

}