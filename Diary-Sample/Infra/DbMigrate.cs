// -----------------------------------------------------------------------
// <copyright file="DbMigrate.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Infra;

public static class DbMigrate
{
    public static IHost MigrateDatabase<T>(this IHost host)
    where T : DiarySampleContext
    {
        host = host ?? throw new ArgumentNullException(nameof(host));
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var db = services.GetRequiredService<T>();
                db.Database.Migrate();
            }
            catch (SqlException ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, message: "An error occurred while migrating the database.");
            }
        }
        return host;
    }
}