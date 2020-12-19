// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Diary_Sample.Entities;
using Diary_Sample.Infra;
using Diary_Sample.Repositories;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Diary_Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
#pragma warning disable CA1822 // メンバーを static に設定します
        public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // メンバーを static に設定します
        {
            services.AddControllersWithViews();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<ICreateService, CreateService>();
            services.AddSingleton<IReferService, ReferService>();
            services.AddSingleton<IEditService, EditService>();
            services.AddSingleton<IDiaryRepository, DiaryRepository>();
            services.AddSingleton<DiarySampleContext>();

            services.AddDbContext<DiarySampleContext>(options =>
                        options.UseMySQL(
                            Configuration.GetConnectionString("DbConnectionString")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
             .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DiarySampleContext>().AddDefaultTokenProviders();
            services.AddRazorPages();

            // Redis
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetConnectionString("SessionConnectionString")));
            services.AddScoped<RedisTicketStore>();
            services.AddDataProtection()
                    .SetApplicationName("Diary_Sample.Infra")
                .PersistKeysToStackExchangeRedis(services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>(), "DataProtection-Keys");

            // セッション設定
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie名
                options.Cookie.Name = "diary_sample_auth_cookie";
                // アクセスが禁止されているリソースにアクセスしようとしたときにリダイレクトする相対パス
                // options.AccessDeniedPath = "/Auth/AccessDenied";
                // 認証されていないユーザーがリソースにアクセスしようとしたときにリダイレクトする相対パス
                options.LoginPath = "/Auth/NotAuthenticated";
                // HTTPのみでCookieを使用。（クライアント側のスクリプトでCookieにアクセスさせない）
                options.Cookie.HttpOnly = true;
                // 次回から自動ログインするを指定した際のCookie保存期間。指定しない場合の保存期間はセッション（ブラウザを閉じるまで）
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                // timeout 属性で設定した時間の半分を過ぎてアクセスすると、有効期限が延長された認証チケット/クッキーが再発行されます。
                options.SlidingExpiration = true;
                // セッションストアにRedisTicketStoreを使用する
                options.SessionStore = services.BuildServiceProvider().GetRequiredService<RedisTicketStore>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // メンバーを static に設定します
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1822 // メンバーを static に設定します
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Menu/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 認証・認可
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}");
            });
        }
    }
}