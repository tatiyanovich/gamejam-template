namespace Code.Common.Cooldown
{
	public static class CooldownExtensions
	{
		public static void PutOnCooldown(this GameEntity entity, float cooldownDuration)
		{
			entity.isOnCooldown = true;
			entity.ReplaceCooldownTimeLeft(cooldownDuration);
		}
	}
}