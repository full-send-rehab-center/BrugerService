using Microsoft.AspNetCore.Mvc;
using BrugerService.DTO;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;


namespace BrugerServiceApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BrugerServiceController : ControllerBase
{
    private readonly ILogger<BrugerServiceController> _logger;

    private readonly string _dbPath;
    private readonly string _hostingName;

    public BrugerServiceController(ILogger<BrugerServiceController> logger, IConfiguration config)
    {
        _logger = logger;
        _dbPath = config["DbPath"];
        _hostingName = config["HostName"];

        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var _ipaddr = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"BrugerService responding from {_ipaddr}");
    }

    // Post bruger
    [HttpPost(Name = "PostBruger")]
    public void Post([FromBody] UserDTO user)
    {
        // Log
        _logger.LogInformation($"Method 'PostBruger' called with the values: {user.username} {user.givenName} {user.address} {user.birthDate} {user.email} {user.telephone}");

        // Create Connection to RabbitMQ Server
        var factory = new ConnectionFactory { HostName = _hostingName };
        using var connection = factory.CreateConnection();

        // Using Connection to create Channel
        using var channel = connection.CreateModel();
        _logger.LogInformation($"Creating Queue using {factory.HostName}");

        // Use channel to declare queue
        channel.ExchangeDeclare(exchange: "BrugerService", type: ExchangeType.Topic);

        // Convert to Json
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));

        // Send Json Object
        channel.BasicPublish(exchange: "BrugerService",
                    routingKey: "KEY",
                    basicProperties: null,
                    body: body);
    }
}


