using System.Net;
using System.Text;

using client.Application.Services;

namespace client.tests;

public class NotificationServiceTests
{
    [Fact]
    // Repeated polling must stay quiet until the server returns a different notification set.
    public async Task GetNotificationsAsync_RaisesChangeOnlyWhenNotificationSetChanges()
    {
        // The first two responses are identical; the third introduces notification id 2.
        var handler = new SequenceHttpMessageHandler(
            """
            [
              { "id": 1, "text": "Existing", "isRead": false, "time": "2026-06-09T08:00:00" }
            ]
            """,
            """
            [
              { "id": 1, "text": "Existing", "isRead": false, "time": "2026-06-09T08:00:00" }
            ]
            """,
            """
            [
              { "id": 1, "text": "Existing", "isRead": true, "time": "2026-06-09T08:00:00" },
              { "id": 2, "text": "New", "isRead": false, "time": "2026-06-09T08:01:00" }
            ]
            """
        );
        var service = new NotificationService(new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/"),
        });
        int changeCount = 0;
        service.NotificationsChanged += (_, _) => changeCount++;

        // First poll creates the baseline, second is unchanged, and third raises one event.
        await service.GetNotificationsAsync(1);
        await service.GetNotificationsAsync(1);
        await service.GetNotificationsAsync(1);

        Assert.Equal(1, changeCount);
    }

    private sealed class SequenceHttpMessageHandler(params string[] responses) : HttpMessageHandler
    {
        private int _responseIndex;

        // Returns deterministic JSON responses without starting the API during this unit test.
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            int index = Math.Min(_responseIndex++, responses.Length - 1);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responses[index], Encoding.UTF8, "application/json"),
            };

            return Task.FromResult(response);
        }
    }
}
