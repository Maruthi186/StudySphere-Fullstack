namespace StudySphereApi.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string? CourseName { get; set; } // e.g., "CS 101"
        public string? GroupName { get; set; }  // e.g., "Midterm Prep"
    }
}