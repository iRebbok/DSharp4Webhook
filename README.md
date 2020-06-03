# DSharp4Webhook
[![GitHub CI/CD](https://github.com/iRebbok/DSharp4Webhook/workflows/GitHub%20CI/CD/badge.svg)](https://github.com/iRebbok/DSharp4Webhook/actions/)
[![GitHub RepoSize](https://img.shields.io/github/repo-size/iRebbok/DSharp4Webhook?label=Size)](https://github.com/iRebbok/DSharp4Webhook)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg?label=License)](https://opensource.org/licenses/MIT)
[![Latest GitHub Version](https://img.shields.io/github/v/release/iRebbok/DSharp4Webhook?label=GitHub%20Version)](https://github.com/iRebbok/DSharp4Webhook/releases/latest)
[![Latest NuGet Version](https://img.shields.io/nuget/v/DSharp4Webhook?label=NuGet%20Version)](https://www.nuget.org/packages/DSharp4Webhook/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/DSharp4Webhook?label=Downloads)](https://www.nuget.org/packages/DSharp4Webhook/)
[![Maintainability](https://api.codeclimate.com/v1/badges/292ecc83bf76c922501c/maintainability)](https://codeclimate.com/github/iRebbok/DSharp4Webhook/maintainability)

Unofficial C# library for interacting with the Discord Webhook API and Mono (Unity) support.

## Features
- Send messages (include embeds)
- Send files
- RateLimit support
- Allowed mentions support
- RestProviders (one on `HttpClient`, the other on `WebRequest`)
- Delete webhook
- Modify webhook
- Get information about webhook
- Manipulating operations at the action level (action queue, callbacks)

## Description
Library allows you to fully interact with discord webhook including unity projects that were built with Mono on .NET Framework 4.7.1 (.NET 4.x profile) compatibility.

## About the RestProviders
**Note: You should always choose the default provider that comes with the library, a mono-compatible provider has a memory leak, read more on [`StackOverflow`](https://stackoverflow.com/a/34539083/13175172).**

#### When should I use MonoProvder?
Only if you see such an exception.
```cs
FileNotFoundException: Could not load file or assembly 'System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies.
```
This usually happens on unity projects that are compatible with .NET 4.x, it does not include `System.Net.Http`, only in this case you should use MonoProvider.

#### How do I use MonoProvider?
MonoProvider is delivered as a separate package with [*NuGet*](https://www.nuget.org/packages/DSharp4Webhook.Rest.Mono/), you will need to download it and call `DSharp4Webhook.Rest.Mono.MonoProvder.SetupAsDefault()` **before you create a webhook**.

## Quick start
```csharp
// creating a webhook provider
var provider = new WebhookProvider("your.id");
// creating a webhook
var webhook = provider.CreateWebhook("webhook url");
// sending a message containing 'my content' and passing the callback,
// callback is called after the action has been performed
webhook.SendMessage("my content").Queue(() => Console.WriteLine("my message has been sent!"));
```

## FAQ

#### How do I send a file?
Files are sent via `MessageBuilder`, you just need to add its name and content to the file dictionary.
Example:
```csharp
// reading the file data in the current directory
var fileContent = File.ReadAllBytes("myFile.txt");
// getting a new message builder
var messageBuilder = ConstructorProvider.GetMentionBuilder();
// adding a file to the dictionary of files such as the file name and its content
// yeah, you can change the file name
messageBuilder.Files.Add("myFile.txt", fileContent);
// building a message
// note that you can't change the message content after building
var message = messageBuilder.Build();
// send a message
webhook.SendMessage(message).Queue();
```

#### How to handle constructors correctly?
Each constructor has a `Reset()` method, this method resets the constructor to its default preset (on the message builder also resets allowed mentions, but not the permanent one that was built with it), I recommend using this method on every constructor instead of creating.

#### That means permanently allowed mentions in the message builder?
When creating a constructor, you can select allowed mentions, which are saved for the entire lifecycle of the constructor, its processing: when you reset the constructor to the default preset, it resets the allowed mentions too, and if they were not set during the new build, then a permanent allowed mention is used.
Usually these are the default, which means `None` - no one will be mentioned.

#### What are the allowed mentions by default?
By default, all mentions are forbidden, and the webhook provider has the `AllowedMention` property, you can assign it and it will be used to create new webhooks, the property `AllowedMention` on webhooks is used to send messages that were not configured via message builder.

## Dependences
- `Newtonsoft.Json`
  - [License MIT](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
  - [GitHub](https://github.com/JamesNK/Newtonsoft.Json)
