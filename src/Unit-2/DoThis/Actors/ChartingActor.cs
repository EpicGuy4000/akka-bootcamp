using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using ChartApp.Actors.ChartingMessages;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; private set; }
        }
        
        public class AddSeries
        {
            public AddSeries(Series series)
            {
                Series = series;
            }

            public Series Series { get; }
        }
        
        public class RemoveSeries
        {
            public RemoveSeries(string seriesNameName)
            {
                SeriesName = seriesNameName;
            }

            public string SeriesName { get; }
        }

        #endregion

        public const int MaxPoints = 250;
        private int _xPosCounter = 0;

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;

        public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
        {
        }

        private ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;

            Receive<InitializeChart>(HandleInitialize);
            Receive<AddSeries>(HandleAddSeries);
            Receive<RemoveSeries>(HandleRemoveSeries);
            Receive<Metric>(HandleMetrics);
        }

        private void SetChartBoundaries()
        {
            var allPoints = _seriesIndex.Values.SelectMany(series => series.Points).ToList();
            var yValues = allPoints.SelectMany(point => point.YValues).ToList();
            double maxAxisX = _xPosCounter;
            double minAxisX = _xPosCounter - MaxPoints;
            var maxAxisY = yValues.Count > 0 ? Math.Ceiling(yValues.Max()) : 1.0d;
            var minAxisY = yValues.Count > 0 ? Math.Floor(yValues.Min()) : 0.0d;
            
            if (allPoints.Count <= 2) return;
            
            var area = _chart.ChartAreas[0];
            area.AxisX.Minimum = minAxisX;
            area.AxisX.Maximum = maxAxisX;
            area.AxisY.Minimum = minAxisY;
            area.AxisY.Maximum = maxAxisY;
        }

        #region Individual Message Type Handlers

        private void HandleAddSeries(AddSeries series)
        {
            if (string.IsNullOrEmpty(series.Series.Name) || _seriesIndex.ContainsKey(series.Series.Name)) return;
            
            _seriesIndex.Add(series.Series.Name, series.Series);
            _chart.Series.Add(series.Series);
            SetChartBoundaries();
        }

        private void HandleRemoveSeries(RemoveSeries series)
        {
            if (string.IsNullOrEmpty(series.SeriesName) || !_seriesIndex.ContainsKey(series.SeriesName)) return;
            
            var seriesToRemove = _seriesIndex[series.SeriesName];
            _seriesIndex.Remove(series.SeriesName);
            _chart.Series.Remove(seriesToRemove);
            SetChartBoundaries();
        }

        private void HandleMetrics(Metric metric)
        {
            if (string.IsNullOrEmpty(metric.Series) || !_seriesIndex.ContainsKey(metric.Series)) return;
            
            var series = _seriesIndex[metric.Series];
            series.Points.AddXY(_xPosCounter++, metric.CounterValue);
                
            while (series.Points.Count > MaxPoints)
            {
                series.Points.RemoveAt(0);
            }
                
            SetChartBoundaries();
        }

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            _chart.Series.Clear();
            
            // set the axes up
            var area = _chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;

            //attempt to render the initial chart
            if (_seriesIndex.Any())
            {
                foreach (var series in _seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    _chart.Series.Add(series.Value);
                }
            }
            
            SetChartBoundaries();
        }

        #endregion
    }
}
