using Microsoft.AspNetCore.Mvc;
using BrugerServiceApi.Models;
using BrugerServiceApi.Services;

namespace BrugerServiceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrugerServiceController : ControllerBase
{
    private readonly ILogger<BrugerServiceController> _logger;

    private readonly IUsersService _userService;
    private ILogger<BrugerServiceController> logger;

    public BrugerServiceController(ILogger<BrugerServiceController> logger, IUsersService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    // Get Rest API's
    // Get all Users
    [HttpGet]
    public async Task<List<User>> Get()
    {
        return await _userService.GetAsync();
    }

    // Get User by ID
    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> Get(string id)
    {
        _logger.LogInformation($"GetUser by ID called with id {id}");
        var user = await _userService.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }
        return CreatedAtAction(nameof(Get), new { id = user.userID }, user);
    }

    // Post Rest API's
    // Post User
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User newUser)
    {
        // Log
        _logger.LogInformation($"Method 'PostBruger' called with the values: {newUser.username} {newUser.givenName} {newUser.address} {newUser.email} {newUser.telephone}");

        await _userService.CreateAsync(newUser);
        return CreatedAtAction(nameof(Get), new { userID = newUser.userID }, newUser);
    }

    // Update Rest API's
    // Update User by ID
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _userService.GetAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        updatedUser.userID = user.userID;

        await _userService.UpdateAsync(id, updatedUser);
        return Ok();
    }

    // Delete Rest API's
    // Delete User by ID
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var student = await _userService.GetAsync(id);

        if (student == null)
        {
            return NotFound();
        }
        else
        {
            await _userService.DeleteAsync(student.userID!);
            return Ok();
        }

    }
}


