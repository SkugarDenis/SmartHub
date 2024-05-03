using SmartHub.DeviceHubMiddleware.Entity;
using System.Net.WebSockets;
using System.Text;

namespace SmartHub.Extensions
{
    public static class NetworkExtensions
    {
        public static async Task<WebSocketMessageResult> ReadMessage(this WebSocket webSocket, int BuffeSize)
        {
            byte[] receiveBuffer = new byte[BuffeSize];

            WebSocketReceiveResult receiveResult = default;

            StringBuilder TmpStr = new StringBuilder();

            do
            {
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                TmpStr.Append((Encoding.UTF8.GetString(receiveBuffer, 0, receiveBuffer.Length)).Trim().Replace("\0", ""));
                receiveBuffer = new byte[BuffeSize];

            } while (!receiveResult.EndOfMessage);

            string str = TmpStr.ToString();

            return new WebSocketMessageResult()
            {
                Message = str,
                webScoketResult = receiveResult
            };
        }
    }
}
