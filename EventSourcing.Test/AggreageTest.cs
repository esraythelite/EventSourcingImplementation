using EventSourcingImplementation;
using Shouldly;

namespace EventSourcing.Test
{
    public class AggreageTest<T> where T : AggregateRoot
    {
        private readonly T aggregateRoot;

        public AggreageTest(T aggregateRoot)
        {
            this.aggregateRoot = aggregateRoot;
        }

        protected void Given(params IEvent[] events)
        {
            if (events !=null)
            {
                aggregateRoot.Load(events);
            }
        }

        protected void When(Action<T> command)
        {
            command(aggregateRoot);
        }

        protected void Then<TEvent>(params Action<TEvent>[] conditions)
        {
            var events = aggregateRoot.GetUncommittedEvents();
            events.Count.ShouldBe(1);
            var evnt = events.First();
            evnt.ShouldBeOfType<TEvent>();

            if (conditions != null)
            {
                ((TEvent)evnt).ShouldSatisfyAllConditions(conditions);
            }
        }

        protected void Throws<TException>(Action<T> command, params Action<TException>[] conditions) where TException : Exception
        {
            var exception = Should.Throw<TException>(() => command(aggregateRoot));
            if (conditions != null)
            {
                exception.ShouldSatisfyAllConditions(conditions);
            }
        }

    }
}