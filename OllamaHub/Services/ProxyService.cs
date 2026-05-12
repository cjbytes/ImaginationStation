using System;
using Microsoft.Extensions.Logging;

namespace OllamaHub.Services;

public class ProxyService : IProxyService
{
    private bool _isRunning;
    private readonly ILogger<ProxyService> _logger;

    public ProxyService(ILogger<ProxyService> logger)
    {
        _logger = logger;
    }

    public void StartProxy(string host, int port, string method, string password)
    {
        _logger.LogInformation("Starting proxy at {Host}:{Port}", host, port);
        _isRunning = true;
    }

    public void StopProxy()
    {
        _logger.LogInformation("Stopping proxy");
        _isRunning = false;
    }

    public void ConfigureProxy(ProxyConfig config)
    {
        _logger.LogInformation("Configuring proxy with {Method}", config.Method);
    }

    public bool IsRunning => _isRunning;
}