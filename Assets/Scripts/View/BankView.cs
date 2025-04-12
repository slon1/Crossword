using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BankView : MonoBehaviour {
	[SerializeField] private RectTransform content;
	private List<ClusterView> clustersInBank;

	private void Awake() {
		clustersInBank = new List<ClusterView>();
	}

	public void SetClusters(List<ClusterView> clusters) {
		
		clusters.Shuffle();
				
		foreach (var cluster in clusters) {
			cluster.transform.SetParent(content, false);
			clustersInBank.Add(cluster);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
		ResetContentPosition();
	}

	public void ReturnCluster(ClusterView cluster) {
		cluster.transform.SetParent(content, false);
		if (!clustersInBank.Contains(cluster)) {
			clustersInBank.Add(cluster);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
		ResetContentPosition();
	}

	public bool IsClusterInBank(Vector2 clusterPos) {
		Vector2 localPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			content,
			clusterPos,
			null,
			out localPos
		);
		return content.rect.Contains(localPos);
	}

	private void ResetContentPosition() {
		content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
		
		// ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
		// if (scrollRect != null)
		// {
		//     scrollRect.verticalNormalizedPosition = 1f;
		// }
	}

	
}
public static class ListExtensions {
	public static void Shuffle<T>(this IList<T> list) {
		for (int i = list.Count - 1; i > 0; i--) {
			int j = Random.Range(0, i + 1);
			(list[i], list[j]) = (list[j], list[i]);
		}
	}
}