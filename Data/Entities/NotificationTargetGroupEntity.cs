using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationTargetGroupEntity
{
    [Key]
    public int Id { get; set; }

    public string TargetGroup { get; set; } = null!;

    public ICollection<NotificationEntity> Notifications { get; set; } = [];


}
