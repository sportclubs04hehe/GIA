using Core.Entities.IdentityBase;
using System;

namespace Infrastructure.Data.Utilities
{
    public static class EntityMetadataUtility
    {
        public static void SetCreateMetadata<T>(T entity) where T : BaseIdentity
        {
            entity.IsDelete = false;
            entity.CreatedDate = DateTime.UtcNow;
        }

        public static void SetUpdateMetadata<T>(T entity) where T : BaseIdentity
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.IsDelete = false;
        }

        public static void SetDeleteMetadata<T>(T entity) where T : BaseIdentity
        {
            entity.IsDelete = true;
            entity.ModifiedDate = DateTime.UtcNow;
        }
    }

    // Thêm lớp non-static để GenericRepository sử dụng
    public class EntityMetadataHandler<T> where T : BaseIdentity
    {
        public void SetCreateMetadata(T entity)
        {
            EntityMetadataUtility.SetCreateMetadata(entity);
        }

        public void SetUpdateMetadata(T entity)
        {
            EntityMetadataUtility.SetUpdateMetadata(entity);
        }

        public void SetDeleteMetadata(T entity)
        {
            EntityMetadataUtility.SetDeleteMetadata(entity);
        }
    }
}