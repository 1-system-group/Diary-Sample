// -----------------------------------------------------------------------
// <copyright file="EmailSender.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Infra.Mail
{
    public class EmailSender : IEmailSender
    {
        private readonly string _senderMailAddress;
        private readonly ILogger<EmailSender> _logger;
        private readonly AWSCredentials _awsCredentials;
        private readonly RegionEndpoint _awsRegion;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;

            // 通知用の送信メールアドレスを環境変数から取得
            _senderMailAddress = Environment.GetEnvironmentVariable("NOTIFICATION_MAIL_ADDRESS") ?? string.Empty;
            _logger.LogInformation($"SenderMailAddress : {_senderMailAddress}");

            // AWSのクレデンシャルとリージョンを環境変数から取得
            var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY") ?? string.Empty;
            var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY") ?? string.Empty;
            var regionName = Environment.GetEnvironmentVariable("AWS_REGION") ?? string.Empty;
            _awsCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            _awsRegion = RegionEndpoint.GetBySystemName(regionName);
        }

        public Task SendEmailAsync(string email, string name, string subject, string htmlMessage, string textMessage)
        {
            var sendRequest = new SendEmailRequest()
            {
                FromEmailAddress = _senderMailAddress,
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string> { email },
                },
                Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = subject },
                        Body = new Body
                        {
                            Html = new Content { Charset = "UTF-8", Data = htmlMessage },
                            Text = new Content { Charset = "UTF-8", Data = textMessage },
                        },
                    },
                },
            };
            // メール送信
            return Task.Run(() =>
            {
                try
                {
                    using var client = new AmazonSimpleEmailServiceV2Client(_awsCredentials, _awsRegion);
                    // 送信者メールアドレス が取得できている場合のみ送信
                    if (!string.IsNullOrEmpty(_senderMailAddress))
                    {
                        client.SendEmailAsync(sendRequest).ConfigureAwait(false);
                        Console.WriteLine("The email was sent.");
                    }
                    else
                    {
                        _logger.LogInformation($"送信先アドレス：{email}");
                        _logger.LogInformation($"宛先名：{name}");
                        _logger.LogInformation($"件名：{subject}");
                        _logger.LogInformation($"HTML：{htmlMessage}");
                        _logger.LogInformation($"TEXT：{textMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            });
        }
    }
}