using SmartHub.DeviceHubMiddleware.Entity;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using SmartHub.Extensions;
using SmartHub.DeviceHubMiddleware.Requests;
using Newtonsoft.Json;
using SmartHub.DeviceHubMiddleware.Response;

public class WebSocketManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();

    public async Task AddConnectionAsync(string connectionId, WebSocket webSocket)
    {
        _connections.TryAdd(connectionId, webSocket);
        await Task.CompletedTask;
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
        await Task.CompletedTask;
    }

    public async Task HandleMessageAsync(string connectionId, byte[] buffer)
    {
        if (_connections.TryGetValue(connectionId, out WebSocket? webSocket))
        {
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

public class DeviceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketManager _webSocketManager;
    private readonly IConfiguration _configuration;
    private readonly int BuffeSize = 2048;
    public DeviceMiddleware(RequestDelegate next, WebSocketManager webSocketManager, IConfiguration configuration)
    {
        _next = next;
        _webSocketManager = webSocketManager;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            string connectionId = Guid.NewGuid().ToString();
            await _webSocketManager.AddConnectionAsync(connectionId, webSocket);

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

        long timeStamp = AuthWebSocketMessage.timeStamp;
        string ExtensionDeviceId = AuthWebSocketMessage.requestObject.ExtensionDeviceId;
        string requestId = AuthWebSocketMessage.requestId;

        string dataSign = timeStamp.ToString() + ExtensionDeviceId + requestId;

        var sign = secretKey.GenerateSHA256Signature(dataSign);

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