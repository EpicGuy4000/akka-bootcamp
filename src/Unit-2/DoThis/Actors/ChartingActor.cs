using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        
        public class TogglePause { }

        #endregion

        public const int MaxPoints = 250;
        private int _xPosCounter = 0;

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;
        private readonly Button _pauseButton;

        public ChartingActor(Chart chart, Button pauseButton) : this(chart, new Dictionary<string, Series>(), pauseButton)
        {
        }

        private ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex, Button pauseButton)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;
            _pauseButton = pauseButton;

            Charting();
        }

        private void Charting()
        {
            Receive<InitializeChart>(HandleInitialize);
            Receive<AddSeries>(HandleAddSeries);
            Receive<RemoveSeries>(HandleRemoveSeries);
            Receive<Metric>(HandleMetrics);

            Receive<TogglePause>(_ =>
            {
                SetPauseButtonText(true);
                BecomeStacked(Paused);
            });
        }

        private void Paused()
        {
            Receive<Metric>(HandleMetricsPaused);
            Receive<TogglePause>(_ =>
            {
                SetPauseButtonText(false);
                UnbecomeStacked();
            });
        }

        private void SetPauseButtonText(bool isPaused)
        {
            _pauseButton.Text = !isPaused ? "PAUSE ||" : "RESUME ->";
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

        private void AddValueForSeries(string seriesName, double value)
        {
            var series = _seriesIndex[seriesName];
            series.Points.AddXY(_xPosCounter++, value);

            while (series.Points.Count > MaxPoints)
            {
                series.Points.RemoveAt(0);
            }

            SetChartBoundaries();
        }

        #region Individual Message Type Handlers

        private void HandleMetricsPaused(Metric metric)
        {
            if (string.IsNullOrEmpty(metric.Series)
                || !_seriesIndex.ContainsKey(metric.Series)) return;
            
            AddValueForSeries(metric.Series, 0.0d);
        }

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

            AddValueForSeries(metric.Series, metric.CounterValue);
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
