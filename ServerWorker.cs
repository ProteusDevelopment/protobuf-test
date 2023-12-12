using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Protobuf;

namespace ProtobufTest;

public class ServerWorker : BackgroundService
{
    private readonly ILogger<ServerWorker> _logger;

    private readonly UdpClient _socket;

    public ServerWorker(ILogger<ServerWorker> logger, int srcPort)
    {
        _logger = logger;

        _socket = new UdpClient(srcPort);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await _socket.ReceiveAsync(stoppingToken);

            var frame = Frame.Parser.ParseFrom(result.Buffer);
            
            _logger.LogInformation("Received <<{Frame}>>", frame.ToString());
        }
    }
}