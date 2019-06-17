using System;

namespace AccessControl.Architects.Entities
{
    public class RoleUserEntity
    {
        public Guid RoleId { get; set; }

        public Guid UserId { get; set; }
    }
}
