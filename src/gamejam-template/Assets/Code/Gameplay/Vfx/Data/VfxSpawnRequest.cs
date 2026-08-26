using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Gameplay.Vfx.Data
{
	public readonly struct VfxSpawnRequest
	{
		public readonly AssetReference Asset;
		public readonly Vector3 Position;
		public readonly Quaternion Rotation;
		public readonly Vector3 Scale;
		public readonly bool OverrideScale;
		public readonly float TargetRadius;
		public readonly bool HasTargetRadius;
		public readonly int FollowId;

		public VfxSpawnRequest(AssetReference asset, Vector3 position)
			: this(asset, position, Quaternion.identity, Vector3.one, false, 0f, 0)
		{
		}

		public VfxSpawnRequest(AssetReference asset, Vector3 position, Quaternion rotation, Vector3 scale)
			: this(asset, position, rotation, scale, true, 0f, 0)
		{
		}

		public VfxSpawnRequest(AssetReference asset, Vector3 position, float targetRadius)
			: this(asset, position, Quaternion.identity, Vector3.one, false, targetRadius, 0)
		{
		}

		public VfxSpawnRequest(AssetReference asset, Vector3 position, float targetRadius, int followId)
			: this(asset, position, Quaternion.identity, Vector3.one, false, targetRadius, followId)
		{
		}

		private VfxSpawnRequest(
			AssetReference asset,
			Vector3 position,
			Quaternion rotation,
			Vector3 scale,
			bool overrideScale,
			float targetRadius,
			int followId)
		{
			Asset = asset;
			Position = position;
			Rotation = rotation;
			Scale = scale;
			OverrideScale = overrideScale;
			TargetRadius = targetRadius;
			HasTargetRadius = targetRadius > 0f;
			FollowId = followId;
		}
	}
}
