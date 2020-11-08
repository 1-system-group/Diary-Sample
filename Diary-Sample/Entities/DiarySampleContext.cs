// -----------------------------------------------------------------------
// <copyright file="DiarySampleContext.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
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
                .UseMySQL(_config.GetConnectionString("DbConnectionString"));
            }
        }
    }
}