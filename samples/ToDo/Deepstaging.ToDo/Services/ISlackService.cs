// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.ToDo.Services;

/// <summary>
///    Service for sending messages to Slack
/// </summary>
public interface ISlackService
{
    /// <summary>
    ///   Posts a message to a Slack channel
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="message"></param>
    Task PostMessageAsync(string channel, string message);
}