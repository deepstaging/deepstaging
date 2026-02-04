// SPDX-FileCopyrightText: 2024-present Deepstaging
// SPDX-License-Identifier: RPL-1.5
namespace Deepstaging.ToDo.Services;

/// <summary>
///    Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    ///  Sends an email
    /// </summary>
    /// <param name="to">The recipient email address</param>
    /// <param name="subject">The email subject</param>
    /// <param name="body">The email body</param>
    Task SendAsync(string to, string subject, string body);
}