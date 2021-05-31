﻿using BlockchainAuthIoT.DataProvider.Models.Realtime;
using LiteNetLib;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainAuthIoT.DataProvider.Services
{
    public class RealtimeServer : IDisposable
    {
        private readonly ITokenVerificationService _tokenVerification;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly EventBasedNetListener eventListener;
        private readonly NetManager server;
        private readonly Dictionary<(string ip, int port), RealtimePeer> peers = new();

        public IEnumerable<RealtimePeer> Peers => peers.Values;

        public RealtimeServer(IConfiguration config, ITokenVerificationService tokenVerification)
        {
            _tokenVerification = tokenVerification;

            var port = int.Parse(config.GetSection("Realtime")["Port"]);

            // Start the server on the port written in the configuration
            eventListener = new EventBasedNetListener();
            server = new NetManager(eventListener);
            server.Start(IPAddress.Any, IPAddress.IPv6Any, port);

            // On connection request, verify the token and accept
            eventListener.ConnectionRequestEvent += async request =>
            {
                var token = request.Data.GetString();

                try
                {
                    var ip = request.RemoteEndPoint.Address.ToString();
                    var port = request.RemoteEndPoint.Port;
                    var contractAddress = await _tokenVerification.VerifyToken(token);
                    peers[(ip, port)] = new(null, contractAddress);
                    request.Accept();
                }
                catch
                {
                    Console.WriteLine($"{request.RemoteEndPoint} failed to connect. Invalid token.");
                    request.Reject();
                }
            };

            // When a peer connects, log it to console
            eventListener.PeerConnectedEvent += peer =>
            {
                var ip = peer.EndPoint.Address.ToString();
                var port = peer.EndPoint.Port;
                peers[(ip, port)].NetPeer = peer;
                Console.WriteLine($"{peer.EndPoint} connected");
            };

            // Periodically refresh
            _ = Task.Run(async () =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        server.PollEvents();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to poll events from the realtime server: {ex.Message}");
                    }
                    
                    await Task.Delay(15);
                }
            });
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            server.Stop();
        }
    }
}
