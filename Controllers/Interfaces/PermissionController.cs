using System;
using System.Linq;
using AccessControl.Architects.Entities.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Controllers.Interfaces
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("Permissions")]
    public class PermissionController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PermissionController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Select()
        {
            var permissionWithRoleIds = from l in _context.RolePermissionEntities.AsNoTracking()
                                        join r in _context.PermissionEntities.AsNoTracking() on l.PermissionId equals r.Id
                                        join rr in _context.RoleEntities.AsNoTracking() on l.RoleId equals rr.Id
                                        select new
                                        {
                                            r,
                                            rr
                                        };

            return Ok(permissionWithRoleIds.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Select(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            return NoContent();
        }
    }
}
