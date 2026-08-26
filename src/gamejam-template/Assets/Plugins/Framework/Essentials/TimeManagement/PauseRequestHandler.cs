using System;

namespace Framework.Essentials.TimeManagement
{
	public readonly struct PauseRequestHandler : IEquatable<PauseRequestHandler>
	{
		public readonly string ID;

		public PauseRequestHandler(string id)
		{
			ID = id;
		}

		public bool Equals(PauseRequestHandler other)
		{
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			return obj is PauseRequestHandler other && Equals(other);
		}

		public override int GetHashCode()
		{
			return ID != null ? ID.GetHashCode() : 0;
		}
	}
}
