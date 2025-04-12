using System.Collections.Generic;
using UnityEngine;

public class Model {
	private readonly char[,] grid;
	private readonly int rows;
	private readonly int cols;

	public int Rows => rows;
	public int Cols => cols;

	public Model(int rows, int cols) {
		this.rows = rows;
		this.cols = cols;
		grid = new char[rows, cols];
	}

	
	public void SetCells(Vector2Int start, string letters) {
		for (int i = 0; i < letters.Length; i++) {
			grid[start.x, start.y + i] = letters[i];
		}
	}

	
	public void ClearCells(Vector2Int start, int length) {
		for (int i = 0; i < length; i++) {
			grid[start.x, start.y + i] = '\0';
		}
	}

	
	public bool IsOccupied(Vector2Int pos) {
		return grid[pos.x, pos.y] != '\0';
	}

	
	public string[] GetWords() {
		List<string> words = new List<string>();
		for (int row = 0; row < rows; row++) {
			string word = "";
			for (int col = 0; col < cols; col++) {
				word += grid[row, col];
			}
			word = word.Trim('\0');
			if (!string.IsNullOrEmpty(word)) {
				words.Add(word);
			}
		}
		return words.ToArray();
	}
}