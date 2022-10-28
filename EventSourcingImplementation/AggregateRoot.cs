using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingImplementation
{
    public abstract class AggregateRoot
    {
        private readonly IList<IEvent> uncommittedEvents = new List<IEvent>();

        public abstract void Load(IEnumerable<IEvent> events);
        
        public IList<IEvent> GetUncommittedEvents()
        {
            return uncommittedEvents;
        }

        public void ClearUncommittedEvents()
        {
            uncommittedEvents.Clear();
        }

        public void Add(IEvent evnt)
        {
            uncommittedEvents.Add(evnt);
        }
    }
}
