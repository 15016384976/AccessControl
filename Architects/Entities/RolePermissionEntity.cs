using System;

namespace AccessControl.Architects.Entities
{
    public class RolePermissionEntity
    {
        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }
    }
}
