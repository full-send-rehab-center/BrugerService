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

    public BrugerServiceController(ILogger<BrugerServiceController> logger, IConfiguration config, UsersService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    // Get Rest API's
    [HttpGet]
    public async Task<List<User>> Get() 
    {
        return await _userService.GetAsync();
    }   

    // Get User by ID
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id) 
    {
        var user = await _userService.GetAsync(id);

        if (user == null) {
            return NotFound();
        }
        return user;
    }

    // Post Rest API's
    // Post User
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        // Log
        _logger.LogInformation($"Method 'PostBruger' called with the values: {newUser.username} {newUser.givenName} {newUser.address} {newUser.email} {newUser.telephone}");

        await _userService.CreateAsync(newUser);
        return CreatedAtAction(nameof(Get), new { userID = newUser.userID}, newUser);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var student = await _userService.GetAsync(id);

        if (student == null) {
            return NotFound();
        }

        await _userService.DeleteAsync(student.userID!);

        return NoContent();
    }
}


