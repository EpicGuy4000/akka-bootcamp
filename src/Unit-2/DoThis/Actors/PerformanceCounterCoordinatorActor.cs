using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using ChartApp.Actors.ChartingMessages;

namespace ChartApp.Actors
{
    public class PerformanceCounterCoordinatorActor : ReceiveActor
    {
        #region Message types

        public class Watch
        {
            public Watch(CounterType counter)
            {
                Counter = counter;
            }

            public CounterType Counter { get; }
        }

        public class Unwatch
        {
            public Unwatch(CounterType counter)
            {
                Counter = counter;
            }

            public CounterType Counter { get; }
        }

        #endregion
        
        private static readonly Dictionary<CounterType, Func<PerformanceCounter>>
            CounterGenerators = new Dictionary<CounterType, Func<PerformanceCounter>>()
            {
                {CounterType.Cpu, () => new PerformanceCounter("Processor", 
                    "% Processor Time", "_Total", true)},
                {CounterType.Memory, () => new PerformanceCounter("Memory", 
                    "% Committed Bytes In Use", true)},
                {CounterType.Disk, () => new PerformanceCounter("LogicalDisk",
                    "% Disk Time", "_Total", true)},
            };
        
        private static readonly Dictionary<CounterType, Func<Series>> CounterSeries =
            new Dictionary<CounterType, Func<Series>>()
            {
                {CounterType.Cpu, () =>
                    new Series(CounterType.Cpu.ToString()){ 
                        ChartType = SeriesChartType.SplineArea,
                        Color = Color.DarkGreen}},
                {CounterType.Memory, () =>
                    new Series(CounterType.Memory.ToString()){ 
                        ChartType = SeriesChartType.FastLine,
                        Color = Color.MediumBlue}},
                {CounterType.Disk, () =>
                    new Series(CounterType.Disk.ToString()){ 
                        ChartType = SeriesChartType.SplineArea,
                        Color = Color.DarkRed}},
            };

        private readonly Dictionary<CounterType, IActorRef> _counterActors;
        private readonly IActorRef _chartingActor;

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor)
            : this(chartingActor, new Dictionary<CounterType, IActorRef>())
        {
        }

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor, Dictionary<CounterType, IActorRef> counterActors)
        {
            _chartingActor = chartingActor;
            _counterActors = counterActors;

            Receive<Watch>(Handle);
            Receive<Unwatch>(Handle);
        }

        private void Handle(Watch message)
        {
            if (!_counterActors.ContainsKey(message.Counter))
            {
                var counterActor = Context.ActorOf(Props.Create(() =>
                    new PerformanceCounterActor(message.Counter.ToString(), CounterGenerators[message.Counter])));

                _counterActors[message.Counter] = counterActor;
            }
            
            _chartingActor.Tell(new ChartingActor.AddSeries(CounterSeries[message.Counter]()));
            
            _counterActors[message.Counter].Tell(new SubscribeCounter(message.Counter, _chartingActor));
        }

        private void Handle(Unwatch message)
        {
            if (!_counterActors.ContainsKey(message.Counter))
            {
                return;
            }
            
            _counterActors[message.Counter].Tell(new UnsubscribeCounter(message.Counter, _chartingActor));
            
            _chartingActor.Tell(new ChartingActor.RemoveSeries(message.Counter.ToString()));
        }
    }
}