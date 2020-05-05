# DSharp4Webhook
[![GitHub CI/CD](https://github.com/iRebbok/DSharp4Webhook/workflows/GitHub%20CI/CD/badge.svg)](https://github.com/iRebbok/DSharp4Webhook/actions/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Release](https://img.shields.io/github/v/release/iRebbok/DSharp4Webhook)](https://github.com/iRebbok/DSharp4Webhook/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/DSharp4Webhook)](https://www.nuget.org/packages/DSharp4Webhook/)

Unofficial C# library for interacting with the Discord Webhook API and Mono (Unity) support.

#### If you need Mono support in the Unity environment, then download the library not from NuGet, but from releases on GitHub, this is due to the fact that I can not make a release with the same version / version that does not have proper versioning. In the future I will solve this problem and place the package on NuGet.

## TL;DR
I needed a management library for discord webhooks with support for rate limit and other things, currently the library does not allow sending files and embeds.

## Quick start
You need to create a WebhookProvider via [`WebhookProvider#CCTOR(string)`](https://github.com/iRebbok/DSharp4Webhook/blob/92c99e77064e0ce6e1eb2e4601562f4aa649034e/src/Core/WebhookProvider.cs#L71) for creating webhooks via [`WebhookProvider#CreateWebhook`](https://github.com/iRebbok/DSharp4Webhook/blob/92c99e77064e0ce6e1eb2e4601562f4aa649034e/src/Core/WebhookProvider.cs#L177) or you can create a webhook without a provider via the static method [`WebhookProvider#CreateSaticWebhook`](https://github.com/iRebbok/DSharp4Webhook/blob/92c99e77064e0ce6e1eb2e4601562f4aa649034e/src/Core/WebhookProvider.cs#L93).

### Creating and using a webhook
```csharp
WebhookProvider provider = new WebhookProvider("some.id");
IWebhook webhook = provider.CreateWebhook("https://discordapp.com/api/webhooks/id/token");
await webhook.SendMessageAsync("moosage");
```
The library also supports message queuing.
This is a useful thing when you don't need to delay the current thread.

### Inserting messages in a queue
```csharp
webhook.QueueMessage("my second moosage");
```

### Permanent change to the nickname of the webhook for sending messages
```csharp
webhook.WebhookInfo.Username = "weebhuk";
```

## Dependences
- `Newtonsoft.Json`
  - [License MIT](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
  - [GitHub](https://github.com/JamesNK/Newtonsoft.Json)
