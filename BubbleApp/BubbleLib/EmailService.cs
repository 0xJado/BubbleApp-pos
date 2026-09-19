using System;
using System.Collections.Generic;
using MimeKit;
using System.Text;
using System.Threading.Tasks;


namespace BubbleLib;

/// <summary>
/// Interface for sending formatted email receipts to customers.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a formatted email receipt to the specified email address.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="order">The completed order containing summary and transaction details.</param>
    /// <param name="items">List of ordered waffle items to include in the receipt.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    Task SendReceiptAsync(string toEmail, CompletedOrder order, List<BubbleWaffle> items);
}

/// <summary>
/// Concrete implementation of <see cref="IEmailService"/> using an SMTP wrapper to send order receipts.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ISmtpClientWrapper _smtpClientWrapper;
    private readonly string? _smtpUsername;
    private readonly string? _smtpPassword;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="smtpClientWrapper">The injected SMTP client wrapper used to send emails.</param>
    public EmailService(ISmtpClientWrapper smtpClientWrapper, string? smtpUsername = null, string? smtpPassword = null)
    {
        _smtpClientWrapper = smtpClientWrapper;
        _smtpUsername = smtpUsername ?? Environment.GetEnvironmentVariable("BUBBLEAPP_SMTP_USERNAME");
        _smtpPassword = smtpPassword ?? Environment.GetEnvironmentVariable("BUBBLEAPP_SMTP_PASSWORD");
    }

    /// <summary>
    /// Composes and sends a detailed email receipt to the specified address. The email includes order metadata,
    /// item breakdown, total cost (including VAT), and wallet information.
    /// </summary>
    /// <param name="toEmail">The email address to which the receipt will be sent.</param>
    /// <param name="order">The completed order object containing metadata for the receipt.</param>
    /// <param name="items">A list of <see cref="BubbleWaffle"/> objects representing the customer's order.</param>
    public async Task SendReceiptAsync(string toEmail, CompletedOrder order, List<BubbleWaffle> items)
    {
        if (string.IsNullOrWhiteSpace(_smtpUsername) || string.IsNullOrWhiteSpace(_smtpPassword))
        {
            throw new InvalidOperationException("Configure BUBBLEAPP_SMTP_USERNAME and BUBBLEAPP_SMTP_PASSWORD before sending receipts.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("WaffleMoments", "noreply@wafflemoments.de"));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = $"Quittung für Bestellung {order.OrderId}";

        var sb = new StringBuilder();
        sb.AppendLine("WaffleMoments");
        sb.AppendLine("Rothebuschstraße 37");
        sb.AppendLine("46117 Oberhausen");
        sb.AppendLine("USt-ID: DE323159977");
        sb.AppendLine();
        sb.AppendLine($"Bestell-ID: {order.OrderId}");
        sb.AppendLine($"Datum: {order.Timestamp:dd.MM.yyyy}, {order.Timestamp:HH:mm} Uhr");
        sb.AppendLine();
        sb.AppendLine("Warenübersicht:");

        foreach (var item in items)
        {
            var subtotal = item.Quantity * item.WafflePrice;
            sb.AppendLine($"{item.Quantity}x {item.WaffleName} für {subtotal:F2} €");
        }

        var vat = order.TotalEur / 1.19m * 0.19m;

        sb.AppendLine();
        sb.AppendLine($"Gesamtbetrag (inkl. 19% MwSt): {order.TotalEur:F2} EUR");
        sb.AppendLine($"→ Enthaltene MwSt: {vat:F2} EUR");
        sb.AppendLine($"Betrag in SOL: {order.TotalSol:F3} SOL");
        sb.AppendLine();
        sb.AppendLine("Wallet-Adresse:");
        sb.AppendLine(order.Wallet);
        sb.AppendLine();
        sb.AppendLine("Vielen Dank für Ihre Bestellung bei WaffleMoments!");

        message.Body = new TextPart("plain") { Text = sb.ToString() };

        // Connect to SMTP server and send the email
        await _smtpClientWrapper.ConnectAsync("smtp.gmail.com", 587, false);
        await _smtpClientWrapper.AuthenticateAsync(_smtpUsername, _smtpPassword);
        await _smtpClientWrapper.SendAsync(message);
        await _smtpClientWrapper.DisconnectAsync(true);

        Console.WriteLine($"Receipt sent to {toEmail}");
    }
}