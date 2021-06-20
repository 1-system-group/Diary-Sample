// -----------------------------------------------------------------------
// <copyright file="DiarySampleContext.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Entities
{
    public partial class DiarySampleContext : IdentityDbContext
    {
        public virtual DbSet<Diary>? diary { get; set; }
        private readonly IConfiguration _config;

        public DiarySampleContext(IConfiguration config)
        {
            _config = config;
        }

        public static readonly ILoggerFactory MySQLLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public static string getDBConnectionString(IConfiguration configuration)
        {
            var jawsDbUrl = Environment.GetEnvironmentVariable("JAWSDB_URL");
            return string.IsNullOrEmpty(jawsDbUrl) ? configuration.GetConnectionString("DbConnectionString") : jawsDbUrl;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder == null)
            {
                throw new System.Exception();
            }
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                .UseLoggerFactory(MySQLLoggerFactory)
                .UseMySQL(getDBConnectionString(_config));
            }
        }
    }
}