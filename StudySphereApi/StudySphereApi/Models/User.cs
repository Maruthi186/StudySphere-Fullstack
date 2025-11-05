using System.Collections.Generic;

namespace StudySphereApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
    }
}