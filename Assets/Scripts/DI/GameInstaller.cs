using Zenject;

public class GameInstaller : MonoInstaller {
	public override void InstallBindings() {
		Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
		Container.Bind<GridView>().FromComponentInHierarchy().AsSingle();
		Container.Bind<BankView>().FromComponentInHierarchy().AsSingle();
		Container.BindFactory<Cell, Cell.Factory>()
			.FromComponentInNewPrefabResource("Cell")
			.AsSingle();
		Container.BindFactory<ClusterView, ClusterView.Factory>()
			.FromComponentInNewPrefabResource("Cluster")
			.AsSingle();
	}
}