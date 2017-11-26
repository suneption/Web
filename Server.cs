using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class Server : IDisposable
{
    private readonly IPAddress _defaultIpAddress;
    private readonly int _defaultPort;
    public static long _countOfRequests = 0;

    private readonly Socket _rootSocket;

    public Server()
    {
        _defaultIpAddress = IPAddress.Any;
        _defaultPort = 10001;

        _rootSocket = Initialize();

        var timer = new System.Timers.Timer(1000);
        timer.Start();
        timer.Elapsed += (sender, e) => {
            var countOfRequests = Interlocked.Exchange(ref _countOfRequests, 0);
            Console.WriteLine(countOfRequests);
        };
    }

    public Socket Initialize() 
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(_defaultIpAddress, _defaultPort));
        socket.Listen(10);

        return socket;
    }

    public async Task Listen() 
    {
        while(true) 
        {                    
            var clientSocket = await AcceptAsync(_rootSocket);
            Task.Run(() => HandleClient(clientSocket));
        }
    }

    public async Task HandleClient(Socket clientSocket) 
    {
        try
        {
            var requestBuffer = new byte[1024];
            var requestSize = await ReceiveAsync(clientSocket, requestBuffer);
            // var message = Encoding.UTF8.GetString(inputByteBuffer.Take(requestSize).ToArray());
            // var inputMessage = message.Trim();
            // var outputMessageBytes = Encoding.UTF8.GetBytes(inputMessage.ToUpper());
            Thread.Sleep(1000);
            var outputMessageBytes = new byte[] {};
            clientSocket.Send(outputMessageBytes);

            Interlocked.Increment(ref _countOfRequests);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            if (clientSocket != null)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }

    public Task<int> ReceiveAsync(Socket clientSocket, byte[] requestBuffer) 
    {
        var result = Task.Factory
            .FromAsync(
                clientSocket.BeginReceive(requestBuffer, 0, requestBuffer.Length, SocketFlags.None, null, clientSocket), 
                clientSocket.EndReceive);
        result.ConfigureAwait(false);

        return result;
    }

    public Task<Socket> AcceptAsync(Socket clientSocket)
    {
        var result = Task.Factory
            .FromAsync(clientSocket.BeginAccept, clientSocket.EndAccept, null);
        result.ConfigureAwait(false);
        return result;
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                _rootSocket.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~Server() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}