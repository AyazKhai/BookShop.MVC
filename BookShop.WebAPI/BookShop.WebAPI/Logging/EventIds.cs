namespace BookShop.WebAPI.Logging
{
    public static class EventIds
    {
        public static readonly EventId ControllerInitialized = new EventId(1, "ControllerInitialized");
        public static readonly EventId NotFound = new EventId(2, "NotFound");//404
        public static readonly EventId Created = new EventId(3, "Created");
        public static readonly EventId Updated = new EventId(4, "Updated");
        public static readonly EventId Readed = new EventId (5,"Readed");
        public static readonly EventId Fetched = new EventId(6, "Fetched");
        public static readonly EventId FetchedAll = new EventId(7, "FetchedAll");
        public static readonly EventId Error = new EventId(8, "Error");
        public static readonly EventId Exception = new EventId(9, "Exception");
        public static readonly EventId Deleted = new EventId(10, "Deleted");
    }
}
