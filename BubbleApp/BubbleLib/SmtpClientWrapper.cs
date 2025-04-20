using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace BubbleLib;

/// Defines the contract for an SMTP client wrapper.
public interface ISmtpClientWrapper : IDisposable
{
    /// Establishes a connection to the SMTP server.
    Task ConnectAsync(string host, int port, bool useSsl); 
    
    /// Authenticates with the SMTP server.
    Task AuthenticateAsync(string username, string password);
    
    /// Sends an email message.
    Task SendAsync(MimeMessage message);
    
    /// Disconnects from the SMTP server.
    Task DisconnectAsync(bool quit);
}

/// Concrete implementation of ISmtpClientWrapper using MailKit.
public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _client = new();

    /// Connects to the specified SMTP server.
    public Task ConnectAsync(string host, int port, bool useSsl) => 
        _client.ConnectAsync(host, port, useSsl);

    /// Authenticates with the provided credentials.
    public Task AuthenticateAsync(string username, string password) => 
        _client.AuthenticateAsync(username, password);

    /// Sends the given email message through the SMTP connection.
    public Task SendAsync(MimeMessage message) =>
        _client.SendAsync(message);

    /// Disconnects the current SMTP session.
    public Task DisconnectAsync(bool quit) =>
        _client.DisconnectAsync(quit);

    /// Disposes the SMTP client instance.
    public void Dispose() => 
        _client.Dispose();
}