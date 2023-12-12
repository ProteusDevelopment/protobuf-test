using System.Net;
using System.Net.Sockets;
using System.Drawing;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Protobuf;
using ProtobufTest;

// Protobuf Test Project.
//   Client sends frames information, server receives. Frame format described on `frame.proto` file.
// Arguments:
//   1. `--src-port <int>`: server port for receive frames;
//   2. `--dest-port <int>`: port for send frames to server.

var argsList = args.ToList();
var srcPortIndex = argsList.IndexOf("--src-port") + 1;
var destPortIndex = argsList.IndexOf("--dest-port") + 1;

int.TryParse(argsList[srcPortIndex], out var srcPort);
int.TryParse(argsList[destPortIndex], out var destPort);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // Set up server service.
        services.AddHostedService<ServerWorker>(provider => 
            new ServerWorker(provider.GetService<ILogger<ServerWorker>>() 
                             ?? throw new ApplicationException("Something wrong"), 
                srcPort));

        // Set up client service.
        services.AddHostedService<ClientWorker>(provider =>
            new ClientWorker(provider.GetService<ILogger<ClientWorker>>() 
                             ?? throw new ApplicationException("Something wrong"),
                new IPEndPoint(IPAddress.Loopback, destPort)));
    })
    .Build();
    
host.Run();