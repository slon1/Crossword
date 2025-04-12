using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class LevelService : MonoBehaviour {
	private const string CURRENT_LEVEL_KEY = "CurrentLevel";
	private const string DEFAULT_LEVEL_PATH = "Levels/default_level";
	private int currentLevel;

	private void Awake() {
		currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1); // Начинаем с уровня 1
	}

	private async void Start() {
		await InitializeServices();
	}

	private async Task InitializeServices() {
		await UnityServices.InitializeAsync();
		if (!AuthenticationService.Instance.IsSignedIn) {
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}

	public async UniTask<LevelData> LoadLevel(int levelNumber) {
		currentLevel = levelNumber;

		if (levelNumber == 1) {
			// Загружаем локальный уровень по умолчанию
			TextAsset levelJson = Resources.Load<TextAsset>(DEFAULT_LEVEL_PATH);
			if (levelJson == null) {
				Debug.LogError($"Failed to load default level from {DEFAULT_LEVEL_PATH}");
				return null;
			}
			return new LevelData(levelJson.text);
		}
		else {
			// Загружаем уровень через Remote Config
			var tcs = new UniTaskCompletionSource<LevelData>();

			void OnFetch(ConfigResponse configResponse) {
				RemoteConfigService.Instance.FetchCompleted -= OnFetch;
				if (configResponse.requestOrigin == ConfigOrigin.Remote) {
					string levelKey = $"Level{currentLevel}";
					//string jsonData = RemoteConfigService.Instance.appConfig.GetJson(levelKey);
					string jsonData = RemoteConfigService.Instance.appConfig.GetJson("TestLevelData");
					if (string.IsNullOrEmpty(jsonData)) {
						Debug.LogError($"Failed to load level data for {levelKey}");
						tcs.TrySetResult(null);
						return;
					}
					Debug.Log($"Loaded level data for {levelKey}: {jsonData}");
					LevelData levelData = new LevelData(jsonData);
					tcs.TrySetResult(levelData);
				}
				else {
					Debug.LogWarning("Failed to fetch remote config. Using cached or default data.");
					tcs.TrySetResult(null);
				}
			}

			RemoteConfigService.Instance.FetchCompleted += OnFetch;
			RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());

			return await tcs.Task;
		}
	}

	public void CompleteLevel() {
		currentLevel++;
		PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, currentLevel);
		PlayerPrefs.Save();
	}

	public int GetCurrentLevel() {
		return currentLevel;
	}

	private struct userAttributes { }
	private struct appAttributes { }
}