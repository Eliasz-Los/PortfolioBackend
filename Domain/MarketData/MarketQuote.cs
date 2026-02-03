namespace Domain.MarketData;

public class MarketQuote
{
    public string Symbol { get; set; }
    public double Current { get; set; }
    public double Change { get; set; }
    public double PercentChange { get; set; }
    public double High  { get; set; }
    public double Low { get; set; }
    public double Open { get; set; }
    public double PreviousClose { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
    
    public MarketQuote(string symbol, double current, double change, double percentChange, double high, double low, double open, double previousClose, DateTimeOffset timestampUtc)
    {
        Symbol = symbol;
        Current = current;
        Change = change;
        PercentChange = percentChange;
        High = high;
        Low = low;
        Open = open;
        PreviousClose = previousClose;
        TimestampUtc = timestampUtc;
    }
    
}