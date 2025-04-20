using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace BubbleLib;
public interface ISmtpClientWrapper : IDisposable
{
    Task ConnectAsync(string host, int port, bool useSsl); 
    Task AuthenticateAsync(string username, string password);
    Task SendAsync(MimeMessage message);
    Task DisconnectAsync(bool quit);
}

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _client = new();

    public Task ConnectAsync(string host, int port, bool useSsl) => 
        _client.ConnectAsync(host, port, useSsl);

    public Task AuthenticateAsync(string username, string password) => 
        _client.AuthenticateAsync(username, password);

    public Task SendAsync(MimeMessage message) =>
        _client.SendAsync(message);

    public Task DisconnectAsync(bool quit) =>
        _client.DisconnectAsync(quit);

    public void Dispose() => 
        _client.Dispose();
}