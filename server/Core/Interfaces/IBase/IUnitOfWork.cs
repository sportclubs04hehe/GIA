using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IBase
{
    /// <summary>
    /// Đại diện cho Unit of Work dùng để quản lý các repository và giao dịch.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Lấy repository cho kiểu thực thể chỉ định.
        /// </summary>
        /// <typeparam name="TEntity">Kiểu thực thể.</typeparam>
        /// <returns>Repository tương ứng với kiểu thực thể.</returns>
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Bắt đầu một giao dịch (transaction).
        /// </summary>
        /// <returns>Task đại diện cho thao tác bất đồng bộ.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Xác nhận giao dịch và lưu tất cả thay đổi trong unit of work hiện tại.
        /// </summary>
        /// <returns>Số lượng bản ghi đã ghi vào cơ sở dữ liệu.</returns>
        Task<int> Complete();

        /// <summary>
        /// Hủy bỏ giao dịch (rollback).
        /// </summary>
        /// <returns>Task đại diện cho thao tác bất đồng bộ.</returns>
        Task RollbackTransactionAsync();
    }

}
