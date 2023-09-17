// -----------------------------------------------------------------------
// <copyright file="EmailSenderLocal.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Infra.Mail;

public class EmailSenderLocal : IEmailSender
{
    private readonly ILogger<EmailSenderLocal> _logger;
    public EmailSenderLocal(ILogger<EmailSenderLocal> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string name, string subject, string htmlMessage, string textMessage)
    {
        // ローカルではログに出力する
        return Task.Run(() =>
        {
            _logger.LogInformation($"送信先アドレス：{email}");
            _logger.LogInformation($"宛先名：{name}");
            _logger.LogInformation($"件名：{subject}");
            _logger.LogInformation($"HTML：{htmlMessage}");
            _logger.LogInformation($"TEXT：{textMessage}");
        });
    }
}