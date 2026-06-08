// =============================================================================
// Author: Nicolai
// =============================================================================

using System.Net.Http;
using System.Net.Http.Json;

using client.Application.Interfaces;
using client.Domain.Models;

namespace client.Application.Services;

public class MessageService : IMessageService
{
    private readonly HttpClient _httpClient;

    // Author: Nicolai
    public MessageService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5189")
        };
    }

    // Author: Nicolai
    public async Task<List<Message>> GetMessagesAsync(int id)
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<MessageDto>>($"api/channels/{id}/messages") ?? [];

        return [.. dtos.Select(dto => new Message(
                    dto.Id,
                    dto.Text,
                    dto.Time,
                    dto.ChannelId,
                    dto.SenderUserId,
                    dto.SenderUsername
                    ))];
    }

    // Author: Nicolai
    public async Task<Message> SendMessageAsync(int id, string text, int userId)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/channels/{id}/messages", new MessageCreateDto
        {
            Text = text,
            UserId = userId
        });
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<MessageDto>()
            ?? throw new InvalidOperationException("API failed to return created message");

        return new Message(
                dto.Id,
                dto.Text,
                dto.Time,
                dto.ChannelId,
                dto.SenderUserId,
                dto.SenderUsername
                );
    }

    private sealed class MessageDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public int ChannelId { get; set; }
        public int SenderUserId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
    }

    private sealed class MessageCreateDto
    {
        public string Text { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
