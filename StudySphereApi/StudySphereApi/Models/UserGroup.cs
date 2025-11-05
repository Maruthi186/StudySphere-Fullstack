namespace StudySphereApi.Models
{
    public class UserGroup
    {
        // Foreign Key for the User
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property back to the User

        // Foreign Key for the Group
        public int GroupId { get; set; }
        public Group Group { get; set; } // Navigation property back to the Group
    }
}