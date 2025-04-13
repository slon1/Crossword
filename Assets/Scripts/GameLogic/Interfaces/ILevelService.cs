using Cysharp.Threading.Tasks;

namespace GameLogic {
	public interface ILevelService {
		UniTask Initialize();
		UniTask<LevelData> LoadLevel(int levelNumber);
		void CompleteLevel();
		int GetCurrentLevel();
	}
}