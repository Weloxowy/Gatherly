namespace gatherly.server.Entities.Meetings;

public class AvailabilityTimesDTO
{
    public virtual Guid UserId { get; set; }
    public virtual string Availability { get; set; }
    public virtual bool IsOwner { get; set; }
}