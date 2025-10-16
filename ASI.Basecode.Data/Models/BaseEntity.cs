using System;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Base entity class that implements soft delete functionality
    /// </summary>
    public abstract class BaseEntity : ISoftDeletable
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}