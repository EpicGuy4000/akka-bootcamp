using Akka.Actor;

namespace ChartApp.Actors.ChartingMessages
{
    public class UnsubscribeCounter
    {
        public UnsubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Counter = counter;
            Subscriber = subscriber;
        }

        public CounterType Counter { get; }
        
        public IActorRef Subscriber { get; }
    }
}