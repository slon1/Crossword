using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameController : MonoBehaviour {
	private LevelData levelData;
	private Model model;
	private GridView gridView;
	private BankView bankView;
	private ClusterView.Factory clusterViewFactory;
	private List<ClusterView> clusters;
	private ClusterPositionManager positionManager;
	[SerializeField]
	private Canvas canvas;
	[SerializeField] private Button validateButton;

	[Inject]
	public void Construct(GridView gridView, BankView bankView, ClusterView.Factory clusterViewFactory) {
		this.gridView = gridView;
		this.bankView = bankView;
		this.clusterViewFactory = clusterViewFactory;
	}

	private void Start() {
		string json = "{\"Board\":{\"Words\":[\"дягиль\",\"абажур\",\"ловель\",\"логово\"]},\"Clusters\":[{\"Letters\":\"дя\"},{\"Letters\":\"гиль\"},{\"Letters\":\"аба\"},{\"Letters\":\"жур\"},{\"Letters\":\"лов\"},{\"Letters\":\"е\"},{\"Letters\":\"ль\"},{\"Letters\":\"л\"},{\"Letters\":\"ог\"},{\"Letters\":\"ово\"}]}";
		levelData = new LevelData(json);
		model = new Model(4, 6);
		gridView.Initialize(4, 6);

		positionManager = new ClusterPositionManager(model, gridView);

		
		clusters = new List<ClusterView>();
		foreach (var clusterLetters in levelData.Clusters) {
			var cluster = clusterViewFactory.Create();
			cluster.Initialize(clusterLetters, 100);
			cluster.OnDragBegin += OnClusterDragBegin;
			cluster.OnDragEnd += OnClusterDragEnd;
			clusters.Add(cluster);
			positionManager.InitializeCluster(cluster);
		}

		bankView.SetClusters(clusters);

		if (validateButton != null) {
			validateButton.onClick.AddListener(Validate);
		}
		else {
			Debug.LogError("ValidateButton is not assigned in the Inspector!");
		}
	}

	private void OnClusterDragBegin(ClusterView cluster) {
		positionManager.ClearPosition(cluster);
		cluster.transform.SetParent(canvas.transform, true);
	}

	private void OnClusterDragEnd(ClusterView cluster) {
		Vector2Int oldPosition = positionManager.GetPosition(cluster);
		List<Vector2Int> newCells = gridView.GetCellsUnderCluster(cluster);

		if (newCells.Count == 0 || bankView.IsClusterInBank(cluster.GetPosition())) {
			ReturnClusterToBank(cluster);
			return;
		}

		if (CanPlaceCluster(newCells)) {
			positionManager.PlaceCluster(cluster, newCells);
		}
		else {
			if (oldPosition.x != -1) {
				positionManager.RevertClusterPosition(cluster, oldPosition);
			}
			else {
				ReturnClusterToBank(cluster);
			}
		}
	}

	private void ReturnClusterToBank(ClusterView cluster) {
		positionManager.ClearPosition(cluster);
		bankView.ReturnCluster(cluster);
	}

	private bool CanPlaceCluster(List<Vector2Int> cells) {
		return !cells.Any(cell => model.IsOccupied(cell));
	}

	public void Validate() {
		string[] words = model.GetWords();
		bool win = words.Length == levelData.ExpectedWords.Count && words.All(w => levelData.ExpectedWords.Contains(w));
		if (win) {
			Debug.Log("Победа!");
		}
		else {
			List<string> expectedWords = new List<string>(levelData.ExpectedWords);
			List<string> foundWords = new List<string>();
			foreach (var word in words) {
				if (expectedWords.Contains(word)) {
					foundWords.Add(word);
					expectedWords.Remove(word);
				}
			}
			string resultMessage = "Собрано: " + (foundWords.Count > 0 ? string.Join(", ", foundWords) : "ничего") + "\n";
			resultMessage += "Осталось: " + (expectedWords.Count > 0 ? string.Join(", ", expectedWords) : "ничего");
			Debug.Log(resultMessage);
		}
	}
}