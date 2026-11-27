# BubbleApp-pos

A .NET MAUI point-of-sale demo for bubble-waffle orders and Solana Devnet payments.

See the [BubbleApp v2 roadmap](ROADMAP.md) for the planned redesign, menu management, and payment options.

## BubbleApp

[BubbleApp](BubbleApp/) is an apprenticeship project now targeting .NET 11 and .NET MAUI 11: a waffle-ordering app with Solana Devnet payment tracking, QR codes, order history, and email receipts.

- Solution: `BubbleApp/BubbleApp.sln`
- MAUI application: `BubbleApp/BubbleApp/`
- Shared services: `BubbleApp/BubbleLib/`
- Unit tests: `BubbleApp/UnitTests/`
- Original German documentation: [Dokumentation](BubbleApp/Dokumentation/) and [project report](BubbleApp/Projektdokumentation.pdf)

### Building

Install the .NET 11 SDK (11.0.100 or a later patch in that feature band). The repository pins MAUI 11.0.10 and workload set 11.0.100.1. From the repository root:

```sh
dotnet workload restore BubbleApp/BubbleApp/BubbleApp.csproj
dotnet build BubbleApp/BubbleApp/BubbleApp.csproj -f net11.0-android
dotnet test BubbleApp/UnitTests/UnitTests.csproj
```

Android builds require JDK 21 and Android SDK API 37. On macOS, the app also targets iOS and Mac Catalyst, which require Xcode 27.2 with the pinned workload. On Windows, use `-f net11.0-windows10.0.19041.0` to build the Windows target. The minimum device versions are Android 7 (API 24), iOS 15, macOS 14 for Mac Catalyst, and Windows 10 version 1809.

To restore and build only the Android target on a host without the other platform workloads, pass `-p:BubbleAppTargetFramework=net11.0-android` to the app build command.

### Email configuration

No SMTP credentials are included. Configure `BUBBLEAPP_SMTP_USERNAME` and `BUBBLEAPP_SMTP_PASSWORD` in the environment before starting the app, or supply credentials through the `EmailService` constructor. Receipt sending fails with a configuration error when credentials are missing. Never commit real credentials. Unit tests use dummy credentials and a mocked SMTP client.

The Solana RPC client is configured for Devnet. This project is a demonstration, not a production payment system.

### Restored project history

The original project history contains 61 commits from 21 March to 14 June 2025. It was extracted from the apprenticeship repository and joined to the existing GitHub import without replacing its commits. Only the project subtree was retained; unrelated training material was excluded.

Original authors, commit dates, and messages were preserved. Historical commit IDs changed because paths were reorganized and excluded files were removed. The original email service and email test revisions were omitted; the current email implementation and tests use configurable or dummy credentials.

Presentations, PDF-generation scripts, and documentation template assets remain local and are excluded from version control. The original German project report and documentation already included in this repository remain available.
