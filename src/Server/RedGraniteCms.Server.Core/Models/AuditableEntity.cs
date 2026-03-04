using System.ComponentModel.DataAnnotations;

namespace RedGraniteCms.Server.Core.Models;

// =============================================================================
// AuditableEntity
// =============================================================================

/// <summary>
/// Base class for all persistent domain entities.
/// Provides a stable identity and a full audit trail.
/// Every entity in the system — content or otherwise — inherits from this.
/// </summary>
public abstract class AuditableEntity
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    // Creation audit — immutable after construction
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; init; }

    // Modification audit — updated via UpdateLastModified()
    public DateTimeOffset LastModifiedAt { get; private set; } = DateTimeOffset.UtcNow;
    public string? LastModifiedBy { get; private set; }

    protected AuditableEntity() { } // For deserialization / ORM

    protected AuditableEntity(string? createdBy)
    {
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Called by subclasses whenever a domain mutation occurs.
    /// Only subclasses should trigger this — not external callers.
    /// </summary>
    protected void UpdateLastModified(string? modifiedBy = null)
    {
        LastModifiedAt = DateTimeOffset.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
