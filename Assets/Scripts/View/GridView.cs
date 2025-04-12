using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;

public class GridView : MonoBehaviour {
	[SerializeField] private GridLayoutGroup gridLayoutGroup;
	private RectTransform[,] cells;
	private RectTransform gridRectTransform;
	private int rows;
	private int cols;
	private Cell.Factory cellFactory;

	[Inject]
	public void Construct(Cell.Factory cellFactory) {
		this.cellFactory = cellFactory;
	}

	public void Initialize(int rows, int cols) {
		this.rows = rows;
		this.cols = cols;
		gridRectTransform = GetComponent<RectTransform>();
		gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayoutGroup.constraintCount = cols;
		gridLayoutGroup.cellSize = new Vector2(100f, 100f);
		gridLayoutGroup.spacing = new Vector2(0f, 0f);

		cells = new RectTransform[rows, cols];
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < cols; c++) {
				Cell cell = cellFactory.Create();
				cell.transform.SetParent(transform, false);
				cell.InitializeAsGridCell();
				cells[r, c] = cell.GetComponent<RectTransform>();
			}
		}
	}

	public List<Vector2Int> GetCellsUnderCluster(ClusterView cluster) {
		List<Vector2Int> cellIndices = new List<Vector2Int>();

		
		Vector2 firstCellPos = cluster.Cells[0].GetPosition();
		Vector2 localPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRectTransform, firstCellPos, null, out localPos);

		float cellWidth = gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x;
		float cellHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;

		float offsetX = localPos.x + (gridRectTransform.rect.width / 2);
		float offsetY = (gridRectTransform.rect.height / 2) - localPos.y;

		int row = Mathf.FloorToInt(offsetY / cellHeight);
		int col = Mathf.FloorToInt(offsetX / cellWidth);

		if (row < 0 || row >= rows || col < 0 || col + cluster.Letters.Length > cols) {
			return cellIndices;
		}

		for (int i = 0; i < cluster.Letters.Length; i++) {
			cellIndices.Add(new Vector2Int(row, col + i));
		}

		return cellIndices;
	}

	private Vector2 GetCellPosition(Vector2Int cellIndex) {
		return cells[cellIndex.x, cellIndex.y].position;
	}

	public Vector2 GetCellsCenter(List<Vector2Int> cellIndices) {
		if (cellIndices == null || cellIndices.Count == 0)
			return Vector2.zero;

		Vector2 center = Vector2.zero;
		foreach (var cellIndex in cellIndices) {
			Vector2 cellPos = GetCellPosition(cellIndex);
			center += cellPos;
		}
		center /= cellIndices.Count;
		return center;
	}
}