﻿namespace gatherly.server.Entities.Meetings;

public class MeetingDTOCreate
{
    public virtual string MeetingName { get; set; }
    public virtual string Description { get; set; }
    public virtual string PlaceName { get; set; }
    public virtual double? Lon { get; set; }
    public virtual double? Lat { get; set; }
    public virtual DateTime StartOfTheMeeting { get; set; }
    public virtual DateTime EndOfTheMeeting { get; set; }
    public virtual bool IsMeetingTimePlanned { get; set; } 
    public virtual string TimeZone { get; set; }
}