namespace ChartApp.Actors.ChartingMessages
{
    public class Metric
    {
        public Metric(string series, float counterValue)
        {
            Series = series;
            CounterValue = counterValue;
        }

        public string Series { get; }
        
        public float CounterValue { get; }
    }
}