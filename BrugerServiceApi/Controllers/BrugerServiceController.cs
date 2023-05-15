using Microsoft.AspNetCore.Mvc;
using BrugerServiceApi.Models;
using BrugerServiceApi.Services;

namespace BrugerServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrugerServiceController : ControllerBase
{
    private readonly ILogger<BrugerServiceController> _logger;

    private readonly UsersService _userService;

    private readonly string _dbPath;
    private readonly string _hostingName;

    public BrugerServiceController(ILogger<BrugerServiceController> logger, IConfiguration config, UsersService userService)
    {
        _logger = logger;
        _dbPath = config["DbPath"];
        _hostingName = config["HostName"];
        _userService = userService;

        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var _ipaddr = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"BrugerService responding from {_ipaddr}");


    }

    // Get User by ID
    [HttpGet]
    public async Task<ActionResult<User>> Get(string id) 
    {
        var user = await _userService.GetAsync(id);

        if (user == null) {
            return NotFound();
        }
        return user;
    }

    // Post User
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        // Log
        _logger.LogInformation($"Method 'PostBruger' called with the values: {newUser.username} {newUser.givenName} {newUser.address} {newUser.email} {newUser.telephone}");

        await _userService.CreateAsync(newUser);
        return CreatedAtAction(nameof(Get), new { userID = newUser.userID}, newUser);
    }
}


