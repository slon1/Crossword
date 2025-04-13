using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using GameUI;
namespace GameLogic {
	public class GameController : MonoBehaviour {
		private LevelData levelData;
		private Model model;
		private GridView gridView;
		private BankView bankView;
		private ClusterView.Factory clusterViewFactory;
		private ILevelService levelService;
		private List<ClusterView> clusters;
		private ClusterPositionManager positionManager;
		private IGUIManager guiManager;

		[SerializeField]
		private Transform root;
		[SerializeField] private Button validateButton;

		[Inject]
		public void Construct(GridView gridView, BankView bankView, ClusterView.Factory clusterViewFactory, ILevelService levelService, IGUIManager guiManager) {
			this.gridView = gridView;
			this.bankView = bankView;
			this.clusterViewFactory = clusterViewFactory;
			this.levelService = levelService;
			this.guiManager = guiManager;
		}

		private async void Start() {
			await levelService.Initialize();
			EventBus.Bus.AddListener(EventId.Game, StartGame);
			EventBus.Bus.AddListener<ClusterView>(EventId.ClusterDragBegin, OnClusterDragBegin);
			EventBus.Bus.AddListener<ClusterView>(EventId.ClusterDragEnd, OnClusterDragEnd);
			if (validateButton != null) {
				validateButton.onClick.AddListener(Validate);
			}
			else {
				Debug.LogError("ValidateButton is not assigned in the Inspector!");
			}
		}

		private async void StartGame() {

			await LoadCurrentLevel();
		}

		private async UniTask LoadCurrentLevel() {
			try {
				int currentLevel = levelService.GetCurrentLevel();
				LevelData loadedLevelData = await levelService.LoadLevel(currentLevel);
				if (loadedLevelData == null) {
					Debug.LogError("Failed to load level data!");
					return;
				}

				OnLevelLoaded(loadedLevelData);
			}
			catch (Exception e) {
				Debug.LogError($"Failed to load level: {e.Message}");
			}
		}

		private void OnLevelLoaded(LevelData loadedLevelData) {

			levelData = loadedLevelData;
			model = new Model(4, 6);
			gridView.Initialize(4, 6);

			positionManager = new ClusterPositionManager(model, gridView);

			clusters = new List<ClusterView>();
			foreach (var clusterLetters in levelData.Clusters) {
				var cluster = clusterViewFactory.Create();
				cluster.Initialize(clusterLetters, 100);
				clusters.Add(cluster);
				positionManager.InitializeCluster(cluster);
			}

			bankView.SetClusters(clusters);


		}

		private void Reset() {
			if (clusters != null) {
				foreach (var cluster in clusters) {
					Destroy(cluster.gameObject);
				}
				clusters.Clear();
			}
			model = null;
			gridView.Clear();
			bankView.Clear();
			positionManager = null;
		}

		private void OnClusterDragBegin(ClusterView cluster) {
			positionManager.ClearPosition(cluster);
			cluster.transform.SetParent(root, true);
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
			EventBus.Bus.Invoke(EventId.OnClick);
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
				levelService.CompleteLevel();
				Reset(); 
				guiManager.ShowPanel(PanelId.Win);
				guiManager.Execute<string[]>(PanelId.Win, PageActionId.Win, words);
				//await LoadCurrentLevel(); 
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
		private void OnDestroy() {
			EventBus.Bus.RemoveListener(EventId.Game, StartGame);
			EventBus.Bus.RemoveListener<ClusterView>(EventId.ClusterDragBegin, OnClusterDragBegin);
			EventBus.Bus.RemoveListener<ClusterView>(EventId.ClusterDragEnd, OnClusterDragEnd);

		}
	}
}