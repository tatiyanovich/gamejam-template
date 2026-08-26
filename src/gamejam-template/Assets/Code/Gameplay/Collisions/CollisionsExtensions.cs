namespace Code.Gameplay.Collisions
{
    public static class CollisionsExtensions
    {
        public static GameEntity RemoveCollisionComponents(this GameEntity entity)
        {
            entity.isCollision = false;
            entity.isTriggerEnter = false;
            entity.isTriggerExit = false;
            entity.isTriggerStay = false;
            entity.isCollisionEnter = false;
            entity.isCollisionExit = false;
            entity.isCollisionStay = false;

            if (entity.hasCollidedId)
                entity.RemoveCollidedId();

            if (entity.hasColliderOwnerId)
                entity.RemoveColliderOwnerId();
            
            return entity;
        }
    }
}