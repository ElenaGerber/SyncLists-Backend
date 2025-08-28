using System.Net.WebSockets;
using synclists_backend.Dtos;

namespace synclists_backend.Endpoints;

public static class WSEndpoints
{
    private static readonly List<WebSocket> _sockets = new();

    private static readonly Dictionary<WebSocket, int> _socketListMap = new();

    public static void MapWebSocketEndpoints(this WebApplication app)
    {
        app.Map("/ws", async context =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(socket);
            _socketListMap[socket] = 0;

            await HandleWebSocketAsync(socket);

            _sockets.Remove(socket);
            _socketListMap.Remove(socket);
        });
    }

    private static async Task HandleWebSocketAsync(WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue)
        {
            var messageJson = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            
            try
            {
                
                var message = System.Text.Json.JsonSerializer.Deserialize<WebSocketMessageDto>(messageJson);
                if (message != null && message.Type == "change_list")
                {
                    Console.WriteLine(socket);
                    Console.WriteLine(message.ListId);
                    _socketListMap[socket] = message.ListId;
                }
            }
            catch
            {
                Console.WriteLine("Invalid message from client: " + messageJson);
            }

            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
    
    public static async Task BroadcastMessageAsync(int listId, string type, object? data = null)
    {
        Console.WriteLine(listId);
        foreach (var socket in _socketListMap)
        {
            Console.WriteLine(socket.Value);
        }
        var payload = new { type, data };
        var json = System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        var buffer = new ArraySegment<byte>(bytes);

        foreach (var socket in _sockets)
        {
            if (socket.State == WebSocketState.Open && _socketListMap.TryGetValue(socket, out int socketListId))
            {
                if (socketListId == listId)
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    private record WebSocketMessage(string Type, int ListId);
}
