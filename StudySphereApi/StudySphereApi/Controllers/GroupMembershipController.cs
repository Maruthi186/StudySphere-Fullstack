using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudySphereApi.Data;
using StudySphereApi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudySphereApi.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupMembershipController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private const int DEMO_USER_ID = 1; // Our hardcoded user

        public GroupMembershipController(ApiDbContext context)
        {
            _context = context;
        }

        // --- "JOIN" ENDPOINT ---
        [HttpPost("{groupId}/join")]
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists)
            {
                return NotFound("Group not found.");
            }

            var alreadyMember = await _context.UserGroups
                .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == DEMO_USER_ID);

            if (alreadyMember)
            {
                return Conflict("User is already a member of this group.");
            }

            var userGroupLink = new UserGroup
            {
                UserId = DEMO_USER_ID,
                GroupId = groupId
            };

            _context.UserGroups.Add(userGroupLink);
            await _context.SaveChangesAsync();

            return Ok("Successfully joined group.");
        }

        // --- "GET MEMBERS" ENDPOINT ---
        [HttpGet("{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(int groupId)
        {
            var members = await _context.UserGroups
                .Where(ug => ug.GroupId == groupId)
                .Select(ug => ug.User)
                .ToListAsync();

            if (members == null)
            {
                return NotFound("Group not found.");
            }

            return Ok(members);
        }

        // --- "LEAVE" ENDPOINT ---
        [HttpDelete("{groupId}/leave")]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            var userGroupLink = await _context.UserGroups
                .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == DEMO_USER_ID);

            if (userGroupLink == null)
            {
                return NotFound("User is not a member of this group.");
            }

            _context.UserGroups.Remove(userGroupLink);
            await _context.SaveChangesAsync();

            return Ok("Successfully left group.");
        }
    }
}