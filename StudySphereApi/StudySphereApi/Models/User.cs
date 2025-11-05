using System.Collections.Generic; // Required for ICollection

namespace StudySphereApi.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string Email { get; set; }

        // Navigation property: Tells EF Core that a User can be in many groups.
        public ICollection<UserGroup> UserGroups { get; set; }
    }
}