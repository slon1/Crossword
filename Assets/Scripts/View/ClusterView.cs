using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ClusterView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private string letters;
	private List<Cell> cells;
	private CanvasGroup canvasGroup;
	private RectTransform rectTransform;
	private Cell.Factory cellFactory;
	private Transform border;
	private Vector2 initialMousePosition;
	private Vector2 initialClusterPosition;

	public delegate void DragHandler(ClusterView cluster);
	public event DragHandler OnDragBegin;
	public event DragHandler OnDragEnd;

	public string Letters => letters;
	public List<Cell> Cells => cells;

	[Inject]
	public void Construct(Cell.Factory cellFactory) {
		this.cellFactory = cellFactory;
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Initialize(string letters, float width) {
		this.letters = letters;
		cells = new List<Cell>();
		border = transform.GetChild(0);

		for (int i = 0; i < letters.Length; i++) {
			var cell = cellFactory.Create();
			cell.transform.SetParent(transform, false);
			cell.InitializeAsClusterCell(letters[i].ToString());
			float offset = (letters.Length - 1) * width * 0.5f;
			cell.transform.localPosition = new Vector3(i * width - offset, 0, 0);
			cells.Add(cell);
		}

		rectTransform.sizeDelta = RectTransformUtility.CalculateRelativeRectTransformBounds(transform).size + new Vector3(10, 10);
		border.SetAsLastSibling();
	}

	public Vector2 GetPosition() {
		return rectTransform.position;

	}

	public void SetPosition(Vector2 position) {
		rectTransform.localScale = Vector3.one; 
		rectTransform.DOScale(1.2f, 0.1f) 
			.SetEase(Ease.OutQuad) 
			.OnComplete(() => {
				rectTransform.DOScale(1f, 0.1f) 
					.SetEase(Ease.InQuad); 
			});
		rectTransform.position = position;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = false;
		initialMousePosition = eventData.position;
		initialClusterPosition = rectTransform.position;
		EventBus.Bus.Invoke(EventId.ClusterDragBegin, this);

	}

	public void OnDrag(PointerEventData eventData) {
		Vector2 mouseDelta = eventData.position - initialMousePosition;
		rectTransform.position = initialClusterPosition + mouseDelta;
		
		
	}

	public void OnEndDrag(PointerEventData eventData) {
		canvasGroup.blocksRaycasts = true;
		EventBus.Bus.Invoke(EventId.ClusterDragEnd, this);

	}

	public class Factory : PlaceholderFactory<ClusterView> { }
}