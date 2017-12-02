using System;
using System.Threading;

public class ServerStatistics : IServerDiagnostic
{
    private int _countOfRequests;

    public ServerStatistics()
    {
        var timer = new System.Timers.Timer(1000);
        timer.Start();
        timer.Elapsed += (sender, e) => {
            var countOfRequests = Interlocked.Exchange(ref _countOfRequests, 0);
            Console.WriteLine(countOfRequests);
        };        
    }

    public void ResponseSended()
    {
        Interlocked.Increment(ref _countOfRequests);
    }
}