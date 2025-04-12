using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cell : MonoBehaviour {
	private Text letterText;
	private Image frameImage;
	private RectTransform rectTransform;

	private void Awake() {
		rectTransform = GetComponent<RectTransform>();
		letterText = GetComponentInChildren<Text>();
		frameImage = GetComponent<Image>();
	}

	public void InitializeAsGridCell() {
		letterText.text = "";
		rectTransform.sizeDelta = new Vector2(100f, 100f);
	}

	public void InitializeAsClusterCell(string letter) {
		letterText.text = letter;
		rectTransform.sizeDelta = new Vector2(100f, 100f);
		frameImage.enabled = true;
	}

	public Vector2 GetPosition() {
		return rectTransform.position;
	}

	public RectTransform GetRectTransform() {
		return rectTransform;
	}

	public class Factory : PlaceholderFactory<Cell> { }
}