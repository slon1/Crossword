using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Services.RemoteConfig;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

[System.Serializable]
public class BoardData {
	public Board Board;
	public Cluster[] Clusters;
}

[System.Serializable]
public class Board {
	public string[] Words;
}

[System.Serializable]
public class Cluster {
	public string Letters;
}

public class WordValidatorEditor : EditorWindow {
	private TextAsset dictionaryAsset;
	private TextAsset jsonAsset;
	private string remoteConfigKey = "your_config_key"; 
	private string resultMessage = "";
	private Vector2 scrollPosition;
	private bool isFetchingConfig = false;

	[MenuItem("Tools/Word Validator")]
	public static void ShowWindow() {
		GetWindow<WordValidatorEditor>("Word Validator");
	}

	private void OnGUI() {
		GUILayout.Label("Word Validator Tool", EditorStyles.boldLabel);

		
		GUILayout.Label("Dictionary File (.txt):", EditorStyles.label);
		dictionaryAsset = (TextAsset)EditorGUILayout.ObjectField(dictionaryAsset, typeof(TextAsset), false);

		
		GUILayout.Label("JSON File (.json):", EditorStyles.label);
		jsonAsset = (TextAsset)EditorGUILayout.ObjectField(jsonAsset, typeof(TextAsset), false);

		
		EditorGUI.BeginDisabledGroup(dictionaryAsset == null || jsonAsset == null);
		if (GUILayout.Button("Validate Local Files")) {
			ValidateLocal();
		}
		EditorGUI.EndDisabledGroup();

		
		GUILayout.Label("Remote Config Key:", EditorStyles.label);
		remoteConfigKey = EditorGUILayout.TextField(remoteConfigKey);

		
		EditorGUI.BeginDisabledGroup(isFetchingConfig || dictionaryAsset == null || string.IsNullOrWhiteSpace(remoteConfigKey));
		if (GUILayout.Button("Fetch and Validate from Remote Config")) {
			FetchAndValidateRemoteConfig();
		}
		EditorGUI.EndDisabledGroup();

		
		if (!string.IsNullOrEmpty(resultMessage)) {
			GUILayout.Label("Result:", EditorStyles.boldLabel);
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
			EditorGUILayout.TextArea(resultMessage, EditorStyles.wordWrappedLabel);
			EditorGUILayout.EndScrollView();
		}
	}

	private void ValidateLocal() {
		resultMessage = "";
		if (dictionaryAsset == null || jsonAsset == null) {
			resultMessage = "Error: Please assign both Dictionary and JSON files!";
			return;
		}

		WordValidator validator = new WordValidator(dictionaryAsset.text);
		bool isValid = validator.ValidateJson(jsonAsset.text, out string validationDetails);

		resultMessage = isValid
			? "Success: Only the target words can be formed from the clusters!"
			: $"Validation Failed:\n{validationDetails}";
	}

	private async void FetchAndValidateRemoteConfig() {
		if (dictionaryAsset == null) {
			resultMessage = "Error: Please assign Dictionary file!";
			return;
		}
		if (string.IsNullOrWhiteSpace(remoteConfigKey)) {
			resultMessage = "Error: Please specify a Remote Config key!";
			return;
		}

		isFetchingConfig = true;
		resultMessage = "Fetching Remote Config...";
		Repaint();

		try {
			await UnityServices.InitializeAsync();
			if (!AuthenticationService.Instance.IsSignedIn) {
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
			}

			TaskCompletionSource<ConfigResponse> tcs = new TaskCompletionSource<ConfigResponse>();
			RemoteConfigService.Instance.FetchCompleted += (response) => tcs.SetResult(response);
			RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());

			ConfigResponse response = await tcs.Task;

			if (response.requestOrigin == ConfigOrigin.Remote) {
				string jsonData = RemoteConfigService.Instance.appConfig.GetJson(remoteConfigKey);
				if (string.IsNullOrEmpty(jsonData)) {
					resultMessage = $"Error: No data found for key '{remoteConfigKey}' in Remote Config!";
					isFetchingConfig = false;
					Repaint();
					return;
				}

				WordValidator validator = new WordValidator(dictionaryAsset.text);
				bool isValid = validator.ValidateJson(jsonData, out string validationDetails);

				resultMessage = isValid
					? $"Success: Only the target words can be formed from the clusters (Remote Config, key: {remoteConfigKey})!"
					: $"Validation Failed (Remote Config, key: {remoteConfigKey}):\n{validationDetails}";
			}
			else {
				resultMessage = "Error: Could not fetch Remote Config data!";
			}
		}
		catch (Exception e) {
			resultMessage = $"Error fetching Remote Config: {e.Message}";
		}
		finally {
			isFetchingConfig = false;
			Repaint();
		}
	}

	struct userAttributes { }
	struct appAttributes { }
}

public class WordValidator {
	private List<string> dictionary;

	public WordValidator(string dictionaryText) {
		dictionary = dictionaryText.Split('\n')
			.Select(word => word.Trim().ToLower())
			.Where(word => !string.IsNullOrEmpty(word))
			.ToList();

		if (dictionary.Count == 0) {
			Debug.LogError("Dictionary is empty or invalid!");
		}
	}

	public bool ValidateJson(string jsonData, out string resultMessage) {
		resultMessage = "";
		try {
			BoardData data = JsonUtility.FromJson<BoardData>(jsonData);
			if (data == null || data.Board == null || data.Clusters == null) {
				resultMessage = "Invalid JSON structure!";
				return false;
			}

			List<string> targetWords = data.Board.Words.Select(word => word.ToLower()).ToList();
			List<string> clusters = data.Clusters.Select(cluster => cluster.Letters.ToLower()).ToList();

			foreach (string word in targetWords) {
				if (!CanFormWord(word, clusters)) {
					resultMessage = $"Cannot form target word: {word}";
					return false;
				}
			}

			if (dictionary.Count == 0) {
				resultMessage = "Dictionary is empty!";
				return false;
			}

			foreach (string word in dictionary) {
				if (!targetWords.Contains(word) && CanFormWord(word, clusters)) {
					resultMessage = $"Undesired word can be formed: {word}";
					return false;
				}
			}

			return true;
		}
		catch (Exception e) {
			resultMessage = $"Error processing JSON: {e.Message}";
			return false;
		}
	}

	private bool CanFormWord(string word, List<string> availableClusters) {
		if (word.Length != 6) return false;

		List<string> tempClusters = new List<string>(availableClusters);
		string remainingWord = word;

		while (remainingWord.Length > 0 && tempClusters.Count > 0) {
			bool found = false;
			for (int i = 0; i < tempClusters.Count; i++) {
				string cluster = tempClusters[i];
				if (remainingWord.StartsWith(cluster)) {
					remainingWord = remainingWord.Substring(cluster.Length);
					tempClusters.RemoveAt(i);
					found = true;
					break;
				}
			}
			if (!found) return false;
		}

		return remainingWord.Length == 0;
	}
}