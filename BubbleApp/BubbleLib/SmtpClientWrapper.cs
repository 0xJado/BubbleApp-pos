using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace BubbleLib;

/// <summary>
/// Defines the contract for an SMTP client wrapper to abstract email sending functionality.
/// </summary>
public interface ISmtpClientWrapper : IDisposable
{
    /// <summary>
    /// Establishes a connection to the SMTP server.
    /// </summary>
    /// <param name="host">The SMTP server host.</param>
    /// <param name="port">The port to use for the connection.</param>
    /// <param name="useSsl">Whether to use SSL for the connection.</param>
    Task ConnectAsync(string host, int port, bool useSsl); 
    
    /// <summary>
    /// Authenticates the client using the provided credentials.
    /// </summary>
    /// <param name="username">The username for SMTP authentication.</param>
    /// <param name="password">The password for SMTP authentication.</param>
    Task AuthenticateAsync(string username, string password);
    
    /// <summary>
    /// Sends an email message using the established SMTP connection.
    /// </summary>
    /// <param name="message">The email message to be sent.</param>
    Task SendAsync(MimeMessage message);
    
    /// <summary>
    /// Disconnects from the SMTP server.
    /// </summary>
    /// <param name="quit">If true, sends a QUIT command to the server before disconnecting.</param>
    Task DisconnectAsync(bool quit);
}

/// <summary>
/// Implementation of the <see cref="ISmtpClientWrapper"/> interface using MailKit for sending emails via SMTP.
/// </summary>
public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _client = new();

    /// <summary>
    /// Connects to the specified SMTP server.
    /// </summary>
    /// <param name="host">The hostname of the SMTP server.</param>
    /// <param name="port">The port on which to connect.</param>
    /// <param name="useSsl">Whether to use SSL encryption.</param>
    public Task ConnectAsync(string host, int port, bool useSsl) => 
        _client.ConnectAsync(host, port, useSsl);

    /// <summary>
    /// Authenticates using the given username and password.
    /// </summary>
    /// <param name="username">SMTP username.</param>
    /// <param name="password">SMTP password.</param>
    public Task AuthenticateAsync(string username, string password) => 
        _client.AuthenticateAsync(username, password);

    /// <summary>
    /// Sends the specified email message via SMTP.
    /// </summary>
    /// <param name="message">The <see cref="MimeMessage"/> to be sent.</param>
    public Task SendAsync(MimeMessage message) =>
        _client.SendAsync(message);

    /// <summary>
    /// Disconnects from the SMTP server.
    /// </summary>
    /// <param name="quit">Indicates if a QUIT command should be sent.</param>
    public Task DisconnectAsync(bool quit) =>
        _client.DisconnectAsync(quit);

    /// <summary>
    /// Releases the resources used by the SMTP client.
    /// </summary>
    public void Dispose() => 
        _client.Dispose();
}