using GameLogic;
using Zenject;

public class GameInstaller : MonoInstaller {
	public override void InstallBindings() {
		Container.Bind<IGUIManager>().FromComponentInHierarchy().AsSingle();
		Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<GridView>().FromComponentInHierarchy().AsSingle();
		Container.Bind<BankView>().FromComponentInHierarchy().AsSingle();
		Container.Bind<ILevelService>().To<LevelService>().AsSingle();
		Container.BindFactory<Cell, Cell.Factory>()
			.FromComponentInNewPrefabResource("Cell")
			.AsSingle();
		Container.BindFactory<ClusterView, ClusterView.Factory>()
			.FromComponentInNewPrefabResource("Cluster")
			.AsSingle();
	}
}