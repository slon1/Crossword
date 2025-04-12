using System.Collections.Generic;

public class LevelData {
	public List<string> ExpectedWords { get; }
	public List<string> Clusters { get; }

	public LevelData(string json) {
		
		ExpectedWords = new List<string> { "дягиль", "абажур", "ловель", "логово" };
		Clusters = new List<string> { "дя", "гиль", "аба", "жур", "лов", "е", "ль", "л", "ог", "ово" };
	}
}