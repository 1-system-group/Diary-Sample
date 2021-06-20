// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Diary_Sample.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Diary_Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDatabase<DiarySampleContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
   .ConfigureWebHostDefaults(webBuilder =>
   {
       webBuilder.UseStartup<Startup>();
   });
        }
    }
}