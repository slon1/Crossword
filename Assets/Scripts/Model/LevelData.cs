using System.Collections.Generic;

public class LevelData {
	public List<string> ExpectedWords { get; }
	public List<string> Clusters { get; }

	public LevelData(string json) {
		
		ExpectedWords = new List<string> { "������", "������", "������", "������" };
		Clusters = new List<string> { "��", "����", "���", "���", "���", "�", "��", "�", "��", "���" };
	}
}