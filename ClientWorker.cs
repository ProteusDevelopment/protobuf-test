using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Protobuf;

namespace ProtobufTest;

/// <summary>
/// Client service.
/// </summary>
public class ClientWorker : BackgroundService
{
    private readonly IPEndPoint _remotePoint;
    
    private readonly ILogger<ClientWorker> _logger;

    private readonly UdpClient _socket;

    /// <summary>
    /// Client service constructor.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="remotePoint">Remote point for send frames.</param>
    public ClientWorker(ILogger<ClientWorker> logger, IPEndPoint remotePoint)
    {
        _logger = logger;
        _remotePoint = remotePoint;

        _socket = new UdpClient();
    }

    /// <summary>
    /// Sends frame information every second.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var frame = new Frame
            {
                Created = Timestamp.FromDateTime(DateTime.UtcNow),
                FrameInfo = "254546",
                Number = 254546
            };
            // Transform frame to datagram for send.
            var datagram = new ReadOnlyMemory<byte>(frame.ToByteArray());
            
            _logger.LogInformation("Frame {FrameCreated} sended", frame.Created);
            
            await _socket.SendAsync(datagram, _remotePoint, stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}