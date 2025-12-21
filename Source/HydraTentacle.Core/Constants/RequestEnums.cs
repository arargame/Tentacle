namespace HydraTentacle.Core.Constants
{
    public enum RequestStatus
    {
        Open,
        InProgress,
        Waiting,
        Completed,
        Cancelled
    }

    public enum RequestPriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    public enum RequestSlaLevel
    {
        None,
        Standard,
        Urgent,
        Emergency
    }
}
