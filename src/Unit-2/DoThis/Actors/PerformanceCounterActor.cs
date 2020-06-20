using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using Akka.Actor;
using ChartApp.Actors.ChartingMessages;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor : ReceiveActor
    {
        private readonly string _seriesName;
        private readonly Func<PerformanceCounter> _performanceCounterGenerator;
        private PerformanceCounter _counter;

        private readonly HashSet<IActorRef> _subscriptions;
        private readonly ICancelable _cancelPublishing;

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
            
            _subscriptions = new HashSet<IActorRef>();
            _cancelPublishing = new Cancelable(Context.System.Scheduler);

            Receive<GatherMetrics>(Handle);
            Receive<SubscribeCounter>(Handle);
            Receive<UnsubscribeCounter>(Handle);
        }

        private void Handle(SubscribeCounter message)
            => _subscriptions.Add(message.Subscriber);

        private void Handle(UnsubscribeCounter message)
            => _subscriptions.Remove(message.Subscriber);

        private void Handle(GatherMetrics message)
        {
            var metric = new Metric(_seriesName, _counter.NextValue());
            foreach (var sub in _subscriptions)
            {
                sub.Tell(metric);
            }
        }

        #region Lifecycle methods

        protected override void PreStart()
        {
            _counter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(250), 
                TimeSpan.FromMilliseconds(250),
                Self,
                new GatherMetrics(), 
                Self,
                _cancelPublishing
            );
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel(false);
                _counter.Dispose();
            }
            finally
            {
                base.PostStop();
            }
        }

        #endregion
    }
}