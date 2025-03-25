using Microsoft.AspNetCore.Mvc;
using Notifications.Api.Services;

namespace Notifications.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetNotificationsAsync();
        return Ok(notifications);
    }
}