// -----------------------------------------------------------------------
// <copyright file="IEmailSender.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Infra.Mail;

public interface IEmailSender
{
    public Task SendEmailAsync(string email, string name, string subject, string htmlMessage, string textMessage);
}