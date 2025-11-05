using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphereApi.Data;
using StudySphereApi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudySphereApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private const int DEMO_USER_ID = 1; 

        public GroupsController(ApiDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var demoUserMembershipIds = await _context.UserGroups
                .Where(ug => ug.UserId == DEMO_USER_ID)
                .Select(ug => ug.GroupId)
                .ToHashSetAsync();

            var groups = await _context.Groups
                .Select(group => new
                {
                    group.Id,
                    group.CourseName,
                    group.GroupName,
                    IsDemoUserMember = demoUserMembershipIds.Contains(group.Id)
                })
                .ToListAsync();

            return Ok(groups);
        }


        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] Group newGroup)
        {
            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();
            return Ok(newGroup);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var groupToDelete = await _context.Groups.FindAsync(id);
            if (groupToDelete == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(groupToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] Group updatedGroup)
        {
            if (id != updatedGroup.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingGroup = await _context.Groups.FindAsync(id);
            if (existingGroup == null)
            {
                return NotFound();
            }

            existingGroup.CourseName = updatedGroup.CourseName;
            existingGroup.GroupName = updatedGroup.GroupName;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}