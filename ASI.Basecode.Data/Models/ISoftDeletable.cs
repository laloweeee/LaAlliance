using System;

namespace ASI.Basecode.Data.Models
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedDate { get; set; }
        string? DeletedBy { get; set; }
    }
}