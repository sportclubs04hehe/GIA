using Core.Entities.IdentityBase;

namespace Infrastructure.Data.Utilities
{
    public class EntityMetadataHandler<T> where T : BaseIdentity
    {
        public void SetCreateMetadata(T entity)
        {
            entity.IsDelete = false;
            entity.CreatedDate = DateTime.UtcNow;
        }

        public void SetUpdateMetadata(T entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.IsDelete = false;
        }

        public void SetDeleteMetadata(T entity)
        {
            entity.IsDelete = true;
            entity.ModifiedDate = DateTime.UtcNow;
        }
    }
}