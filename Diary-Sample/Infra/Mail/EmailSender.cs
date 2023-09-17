// -----------------------------------------------------------------------
// <copyright file="EmailSender.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;

namespace Diary_Sample.Infra.Mail;

public class EmailSender : IEmailSender
{
    private static string _senderMailAddress = string.Empty;
    private static string _apiKey = string.Empty;
    private readonly ILogger<EmailSender> _logger;
    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;

        // 通知用の送信メールアドレスを環境変数から取得
        _senderMailAddress = Environment.GetEnvironmentVariable("NOTIFICATION_MAIL_ADDRESS") ?? string.Empty;
        _logger.LogInformation($"SenderMailAddress : {_senderMailAddress}");
        // SendGridのAPIキーを環境変数から取得
        _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? string.Empty;
    }

    public Task SendEmailAsync(string email, string name, string subject, string htmlMessage, string textMessage)
    {
        var services = ConfigureServices(new ServiceCollection()).BuildServiceProvider();
        var client = services.GetRequiredService<ISendGridClient>();
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_senderMailAddress, _senderMailAddress),
            Subject = subject,
        };
        msg.AddContent(MimeType.Html, htmlMessage);
        msg.AddContent(MimeType.Text, textMessage);
        msg.AddTo(new EmailAddress(email, $"{name} 様"));

        // メール送信
        return Task.Run(() =>
        {
            // 送信者メールアドレス が取得できている場合のみ送信
            if (!string.IsNullOrEmpty(_senderMailAddress) && !string.IsNullOrEmpty(_apiKey))
            {
                var response = client.SendEmailAsync(msg).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation($"送信先アドレス：{email}");
                _logger.LogInformation($"宛先名：{name}");
                _logger.LogInformation($"件名：{subject}");
                _logger.LogInformation($"HTML：{htmlMessage}");
                _logger.LogInformation($"TEXT：{textMessage}");
            }
        });
    }
    private static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddSendGrid(options =>
        {
            // SendGridのAPIキーを環境変数から取得
            options.ApiKey = !string.IsNullOrEmpty(_apiKey) ? _apiKey : "API_KEY_IS_NOTHING";
        });

        return services;
    }
}