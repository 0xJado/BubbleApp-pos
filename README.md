# Blockchain

A collection of Web3 projects and experiments.

## BubbleApp

[BubbleApp](BubbleApp/) is an apprenticeship project built with .NET 8 and .NET MAUI: a waffle-ordering app with Solana Devnet payment tracking, QR codes, order history, and email receipts.

- Solution: `BubbleApp/BubbleApp.sln`
- MAUI application: `BubbleApp/BubbleApp/`
- Shared services: `BubbleApp/BubbleLib/`
- Unit tests: `BubbleApp/UnitTests/`
- Original German documentation: [Dokumentation](BubbleApp/Dokumentation/) and [project report](BubbleApp/Projektdokumentation.pdf)

### Building

Use a compatible .NET 8 SDK and MAUI workloads for the target platform. Open `BubbleApp/BubbleApp.sln` in an IDE configured for MAUI, or build the Windows target from the repository root:

```powershell
dotnet build BubbleApp/BubbleApp/BubbleApp.csproj -p:TargetFrameworks=net8.0-windows10.0.19041.0
```

This is an archived learning project. The latest local build check stopped during package restore because the available SDK lacked MAUI support; successful compilation has not yet been verified in its new location. Restore also reported a moderate security advisory for MailKit 4.11.0. Review dependencies before using the app beyond a demo.

### Email configuration

No SMTP credentials are included. Configure `BUBBLEAPP_SMTP_USERNAME` and `BUBBLEAPP_SMTP_PASSWORD` in the environment before starting the app, or supply credentials through the `EmailService` constructor. Receipt sending fails with a configuration error when credentials are missing. Never commit real credentials. Unit tests use dummy credentials and a mocked SMTP client.

The Solana RPC client is configured for Devnet. This project is a demonstration, not a production payment system.
