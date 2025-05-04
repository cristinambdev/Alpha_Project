namespace Data.Entities;

public class ProjectUserEntity
{
    public string ProjectId { get; set; } = null!; 
    public ProjectEntity Project { get; set; } = null!;

    public string UserId { get; set; } = null!; 
    public UserEntity User { get; set; } = null!;
}
