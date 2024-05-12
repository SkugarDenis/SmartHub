using SmartHub.DeviceHubMiddleware.Entity;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using SmartHub.Extensions;
using SmartHub.DeviceHubMiddleware.Requests;
using Newtonsoft.Json;
using SmartHub.DeviceHubMiddleware.Response;
using SmartHub.DataContext.DbModels;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class DeviceManager
{
    private readonly ConcurrentDictionary<string, DeviceClient> _connections = new ConcurrentDictionary<string, DeviceClient>();

    public async Task AddConnectionAsync(string connectionId, DeviceClient client)
    {
        // если такой Extension есть удаляем предыдущий
        var connection = _connections.FirstOrDefault(x => x.Value.ExtensionId.Equals(client.ExtensionId));
        if (!connection.Equals(default(KeyValuePair<string, DeviceClient>)))
        {
            _connections.TryRemove(connection.Key, out _);
        }

        _connections.TryAdd(connectionId, client);
        await Task.CompletedTask;
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
        await Task.CompletedTask;
    }

    public async Task HandleMessageAsync(string connectionId, byte[] buffer)
    {
        if (_connections.TryGetValue(connectionId, out DeviceClient? client))
        {
            await client.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public async Task NotificationDevice(string extentsionIdDevice, DeviceInterfaceItem interfaceItem)
    {
        var client = _connections.Values.FirstOrDefault(x => x.ExtensionId.Equals(extentsionIdDevice));

        if (client != null)
        {
            NotificationInterfecItemChange notificationInterfecItemChange = new NotificationInterfecItemChange()
            {
                dataType = interfaceItem.DataType,
                ItefaceName = interfaceItem.Name,
                Value = interfaceItem.Control
            };

            string json = JsonConvert.SerializeObject(notificationInterfecItemChange);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await client.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            Console.WriteLine($"NotificationDevice is not defide connection to device id: {extentsionIdDevice}");
        }
    }
}

public class DeviceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DeviceManager _webSocketManager;
    private readonly IConfiguration _configuration;
    private readonly int BuffeSize = 2048;
    public DeviceMiddleware(RequestDelegate next, DeviceManager webSocketManager, IConfiguration configuration)
    {
        _next = next;
        _webSocketManager = webSocketManager;
        _configuration = configuration;
    }
    
    public async Task Invoke(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest && context.Request.Path.Equals("/devices")) 
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string connectionId = Guid.NewGuid().ToString();

            DeviceClient deviceClient = new DeviceClient()
            {
                deviceType = SmartHub.DataContext.DbModels.DeviceType.None,
                IsAuth = false,
                webSocket = webSocket,
            };

            await Task.Run(async () => await HandleWebSocket(deviceClient, connectionId));

            return;
        }

        await _next(context);
    }

    private async Task HandleWebSocket(DeviceClient deviceClient, string connectionId)
    {
        while (deviceClient.webSocket.State == WebSocketState.Open)
        {
            var result = await deviceClient.webSocket.ReadMessage(BuffeSize);

            if (result.webScoketResult.MessageType == WebSocketMessageType.Text)
            {
                string message = result.Message;

                try
                {
                    BaseWebSocketRequest<object>? baseWebSocketRequest = JsonConvert.DeserializeObject<BaseWebSocketRequest<object>>(message);
                    if (baseWebSocketRequest != null)
                    {
                        await HandlerRecivedMessage(deviceClient,
                            baseWebSocketRequest,
                            message,
                            connectionId);
                    }
                    else
                    {
                        await CloseConnection(deviceClient.webSocket, connectionId);
                    }
                }
                catch (Exception)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes("Error");
                    await _webSocketManager.HandleMessageAsync(connectionId, buffer);
                    await CloseConnection(deviceClient.webSocket, connectionId);
                }

                
            }
            else if (result.webScoketResult.MessageType == WebSocketMessageType.Close)
            {
                await CloseConnection(deviceClient.webSocket, connectionId);
            }
        }
    }

    private async Task HandlerRecivedMessage(DeviceClient deviceClient,
        BaseWebSocketRequest<object> baseWebSocketRequest,
        string request,
        string connectionId)
    {
        if (baseWebSocketRequest.typeRequest == WeboskcetTypeRequest.Auth)
        {
            BaseWebSocketRequest<AuthWebSocketRequest>? AuthWebSocketMessage =
                    JsonConvert.DeserializeObject<BaseWebSocketRequest<AuthWebSocketRequest>>(request);

            AuthWebSocketResponse authResponse = new AuthWebSocketResponse()
            {
                 ResultAutentificate = false
            };

            if (AuthWebSocketMessage != null)
            {
                var IsSucsess = AunteficateClient(AuthWebSocketMessage);
                authResponse.ResultAutentificate = IsSucsess;
                deviceClient.IsAuth = IsSucsess;
                deviceClient.ExtensionId = AuthWebSocketMessage.ExtensionDeviceId;


                await _webSocketManager.AddConnectionAsync(connectionId, deviceClient);
            }

            string json = JsonConvert.SerializeObject(authResponse);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await _webSocketManager.HandleMessageAsync(connectionId, buffer);

        }
        else if (baseWebSocketRequest.typeRequest == WeboskcetTypeRequest.SendingData)
        {
            if (deviceClient.IsAuth)
            {
                BaseWebSocketRequest<DataItemWebSocketRequest>? DataItemWebSocketMessage =
                    JsonConvert.DeserializeObject<BaseWebSocketRequest<DataItemWebSocketRequest>>(request);

                // обработка поступающих данных с девайсов
            }
            else
            {
                AuthWebSocketResponse authResponse = new AuthWebSocketResponse()
                {
                    ResultAutentificate = false
                };
                string json = JsonConvert.SerializeObject(authResponse);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await _webSocketManager.HandleMessageAsync(connectionId, buffer);
            }
        }
        else if (baseWebSocketRequest.typeRequest == WeboskcetTypeRequest.Ping)
        {
            if (deviceClient.IsAuth)
            {
                PongWebSocketResponse pongWebSocketResponse = new PongWebSocketResponse();
                string json = JsonConvert.SerializeObject(pongWebSocketResponse);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await _webSocketManager.HandleMessageAsync(connectionId, buffer);
            }
            else
            {
                AuthWebSocketResponse authResponse = new AuthWebSocketResponse()
                {
                    ResultAutentificate = false
                };
                string json = JsonConvert.SerializeObject(authResponse);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await _webSocketManager.HandleMessageAsync(connectionId, buffer);
            }
        }
    }

    private bool AunteficateClient(BaseWebSocketRequest<AuthWebSocketRequest> AuthWebSocketMessage)
    {
        var secretKey = _configuration["WebSocketSettings:AuthKeyExtensionDevice"];

        string ExtensionDeviceId = AuthWebSocketMessage.ExtensionDeviceId;
        string requestId = AuthWebSocketMessage.requestId;

        string dataSign = ExtensionDeviceId + requestId;

        var sign = secretKey.GenerateMD5Signature(dataSign);

        if (AuthWebSocketMessage.requestObject.Signature.Equals(sign))
            return true;
        else
            return false;

    }

    private async Task CloseConnection(WebSocket webSocket,
        string connectionId)
    {
        await _webSocketManager.RemoveConnectionAsync(connectionId);
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }
}