using System;

namespace Framework.Essentials.CursorManagement
{
	public readonly struct CursorLockStateHandler : IEquatable<CursorLockStateHandler>
	{
		public readonly string ID;

		public CursorLockStateHandler(string id)
		{
			ID = id;
		}

		public bool Equals(CursorLockStateHandler other)
		{
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			return obj is CursorLockStateHandler other && Equals(other);
		}

		public override int GetHashCode()
		{
			return ID != null ? ID.GetHashCode() : 0;
		}
	}
}
