using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScr : ScrAbs
{
	[SerializeField]
	private Text text;
	public override void Execute<T>(PageActionId action, T param) {
		base.Execute(action, param);
		if (action== PageActionId.Win && param is string[] words) {
			text.text = string.Join(Environment.NewLine, words);
		}
	}
}
