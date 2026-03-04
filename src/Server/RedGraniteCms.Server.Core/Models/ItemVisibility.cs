namespace RedGraniteCms.Server.Core.Models;

public enum ItemVisibility
{
    /// <summary>Visible to everyone, including anonymous users.</summary>
    Public,

    /// <summary>Visible to any authenticated user.</summary>
    Authenticated,

    /// <summary>Visible only to explicitly granted users/roles (ACL-controlled).</summary>
    Restricted,

    /// <summary>Visible only to the owner.</summary>
    Private
}
