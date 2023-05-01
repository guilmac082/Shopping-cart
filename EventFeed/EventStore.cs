namespace ShoppingCart.EventFeed
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEvents(
        long firstEventSequenceNumber, long lastEventSequenceNumber);
        void Raise(string eventName, object content);
    }
    public class EventStore : IEventStore
    {
        private static readonly List<Event> database = new List<Event>();

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            return database.Where(e => e.SequenceNumber >= firstEventSequenceNumber && e.SequenceNumber <= lastEventSequenceNumber)
                           .OrderBy(e => e.SequenceNumber);
        }

        public void Raise(string eventName, object content)
        {
            var seqNumber = database.Count();
            database.Add(new Event(seqNumber, DateTimeOffset.UtcNow, eventName, content));
        }

    }
}
