namespace Code.Infrastructure.EntityComponentSystem.SceneEntities.Snapshots
{
	public class SceneEntitySnapshot
	{
		public string SceneEntityGuid;
		public bool IsEnabled;

		public SceneEntitySnapshot(string sceneEntityGuid, bool isEnabled)
		{
			SceneEntityGuid = sceneEntityGuid;
			IsEnabled = isEnabled;
		}
	}
}
