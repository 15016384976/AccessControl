using System;

namespace AccessControl.Architects.Entities
{
    public class PermissionEntity : IEntity
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public int Type { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool Available { get; set; }

        public string Description { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime CreateTime { get; set; }

        public Guid? UpdateBy { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
