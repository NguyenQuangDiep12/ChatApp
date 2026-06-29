namespace ChatApp.Core
{
    /// <summary>
    /// Well-known stable GUIDs for seeded entities.
    /// These match the values inserted in EF Core migrations.
    /// </summary>
    public static class WellKnownIds
    {
        // System Role
        public static readonly Guid SystemRole_User = new("00000000-0000-0000-0000-000000000001");

        // Room Permissions (must match migration 20260628200000_SeedPermissions)
        public static readonly Guid Perm_SendMessage       = new("00000000-0000-0000-0001-000000000001");
        public static readonly Guid Perm_DeleteOwnMessage  = new("00000000-0000-0000-0001-000000000002");
        public static readonly Guid Perm_DeleteAnyMessage  = new("00000000-0000-0000-0001-000000000003");
        public static readonly Guid Perm_EditOwnMessage    = new("00000000-0000-0000-0001-000000000004");
        public static readonly Guid Perm_KickMember        = new("00000000-0000-0000-0001-000000000005");
        public static readonly Guid Perm_BanMember         = new("00000000-0000-0000-0001-000000000006");
        public static readonly Guid Perm_ManageRoles       = new("00000000-0000-0000-0001-000000000007");
        public static readonly Guid Perm_ManageRoom        = new("00000000-0000-0000-0001-000000000008");
        public static readonly Guid Perm_InviteMember      = new("00000000-0000-0000-0001-000000000009");

        // Permission code strings (must match PermissionCode column values)
        public static class PermissionCode
        {
            public const string SendMessage      = "SEND_MESSAGE";
            public const string DeleteOwnMessage = "DELETE_OWN_MESSAGE";
            public const string DeleteAnyMessage = "DELETE_ANY_MESSAGE";
            public const string EditOwnMessage   = "EDIT_OWN_MESSAGE";
            public const string KickMember       = "KICK_MEMBER";
            public const string BanMember        = "BAN_MEMBER";
            public const string ManageRoles      = "MANAGE_ROLES";
            public const string ManageRoom       = "MANAGE_ROOM";
            public const string InviteMember     = "INVITE_MEMBER";
        }
    }
}
