using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData {
	public List<string> ExpectedWords { get; private set; }
	public List<string> Clusters { get; private set; }

	// Вложенные классы для десериализации JSON
	[System.Serializable]
	private class LevelDataWrapper {
		public BoardData Board;
		public List<ClusterData> Clusters;
	}

	[System.Serializable]
	private class BoardData {
		public List<string> Words;
	}

	[System.Serializable]
	private class ClusterData {
		public string Letters;
	}

	public LevelData(string json) {
		try {
			// Десериализуем JSON в промежуточный объект
			var wrapper = JsonUtility.FromJson<LevelDataWrapper>(json);

			// Проверяем, что данные не null
			if (wrapper == null || wrapper.Board == null || wrapper.Clusters == null) {
				throw new Exception("Invalid level data format");
			}

			// Инициализируем свойства
			ExpectedWords = wrapper.Board.Words ?? new List<string>();
			Clusters = wrapper.Clusters.ConvertAll(cluster => cluster.Letters) ?? new List<string>();

			// Проверяем, что списки не пустые, иначе задаём значения по умолчанию
			if (ExpectedWords.Count == 0 || Clusters.Count == 0) {
				throw new Exception("Level data is empty");
			}
		}
		catch (Exception e) {
			Debug.LogError($"Failed to parse level data: {e.Message}. Using default values.");
			
		}
	}
}