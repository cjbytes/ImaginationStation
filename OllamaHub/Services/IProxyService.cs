using System;

namespace OllamaHub.Services;

public interface IProxyService
{
    void StartProxy(string host, int port, string method, string password);
    void StopProxy();
    void ConfigureProxy(ProxyConfig config);
    bool IsRunning { get; }
}