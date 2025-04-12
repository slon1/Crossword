using System.Collections.Generic;
using UnityEngine;

public class ClusterPositionManager {
	private readonly Model model;
	private readonly GridView gridView;
	private readonly Dictionary<ClusterView, Vector2Int> clusterPositions;

	public ClusterPositionManager(Model model, GridView gridView) {
		this.model = model;
		this.gridView = gridView;
		clusterPositions = new Dictionary<ClusterView, Vector2Int>();
	}

	
	public void InitializeCluster(ClusterView cluster) {
		clusterPositions[cluster] = new Vector2Int(-1, -1);
	}

	
	public Vector2Int GetPosition(ClusterView cluster) {
		return clusterPositions.TryGetValue(cluster, out Vector2Int position) ? position : new Vector2Int(-1, -1);
	}

	
	public void ClearPosition(ClusterView cluster) {
		if (clusterPositions.TryGetValue(cluster, out Vector2Int position) && position.x != -1) {
			model.ClearCells(position, cluster.Letters.Length);
			clusterPositions[cluster] = new Vector2Int(-1, -1);
		}
	}

	
	public void PlaceCluster(ClusterView cluster, List<Vector2Int> newCells) {
		Vector2 center = gridView.GetCellsCenter(newCells);
		cluster.SetPosition(center);
		model.SetCells(newCells[0], cluster.Letters);
		clusterPositions[cluster] = newCells[0];
	}

	
	public void RevertClusterPosition(ClusterView cluster, Vector2Int oldPosition) {
		List<Vector2Int> oldCells = new List<Vector2Int>();
		for (int i = 0; i < cluster.Letters.Length; i++) {
			oldCells.Add(new Vector2Int(oldPosition.x, oldPosition.y + i));
		}

		Vector2 center = gridView.GetCellsCenter(oldCells);
		cluster.SetPosition(center);
		model.SetCells(oldPosition, cluster.Letters);
	}
}