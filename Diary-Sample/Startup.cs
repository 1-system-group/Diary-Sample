// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diary_Sample.Entities;
using Diary_Sample.Infra;
using Diary_Sample.Infra.Mail;
using Diary_Sample.Repositories;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using StackExchange.Redis;
using static Diary_Sample.Entities.DiarySampleContext;

namespace Diary_Sample
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
#pragma warning disable CA1822 // メンバーを static に設定します
        public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // メンバーを static に設定します
        {
            // JWTの設定を取得（appsettings.Development.jsonのみから取得。Production環境では環境変数から取得する）
            IConfigurationSection? appSettingSection = Configuration.GetSection("JwtConfigurableOptions");
            JwtConfigurableOptions jwtSettings = new JwtConfigurableOptions();
            appSettingSection.Bind(jwtSettings);

            services.AddControllersWithViews();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<ICreateService, CreateService>();
            services.AddSingleton<IReferService, ReferService>();
            services.AddSingleton<IEditService, EditService>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<ISharedService, SharedService>();
            services.AddSingleton<IDiaryRepository, DiaryRepository>();
            services.AddSingleton<DiarySampleContext>();
            services.AddSingleton<JwtConfigurableOptions>();
            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddSingleton(config => jwtSettings);

            if (_env.IsProduction())
            {
                // 本番環境用の設定
                services.AddSingleton<IEmailSender, EmailSender>();
            }
            else if (_env.IsDevelopment())
            {
                // 開発環境用の設定
                services.AddSingleton<IEmailSender, EmailSenderLocal>();
            }

            var jawsDb = Environment.GetEnvironmentVariable("JAWSDB_URL");
            services.AddDbContext<DiarySampleContext>(options =>
                options.UseMySQL(getDBConnectionString(Configuration)));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DiarySampleContext>()
                .AddDefaultTokenProviders();
            services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/Login", "/");
            });

            // Redis
            var redis = Environment.GetEnvironmentVariable("REDIS_URL");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
                string.IsNullOrEmpty(redis) ? Configuration.GetConnectionString("SessionConnectionString")
                : redis.Split("@")[1] + ",password=" + redis.Split("@")[0].Split(":")[2]));
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
                options.LoginPath = "/Auth/Unauthorized";
                // 権限のないユーザがリソースにアクセスしようとしたときにリダイレクトする相対パス
                options.AccessDeniedPath = "/Auth/AccessDenied";
                // HTTPのみでCookieを使用。（クライアント側のスクリプトでCookieにアクセスさせない）
                options.Cookie.HttpOnly = true;
                // 次回から自動ログインするを指定した際のCookie保存期間。指定しない場合の保存期間はセッション（ブラウザを閉じるまで）
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                // timeout 属性で設定した時間の半分を過ぎてアクセスすると、有効期限が延長された認証チケット/クッキーが再発行されます。
                options.SlidingExpiration = true;
                // セッションストアにRedisTicketStoreを使用する
                options.SessionStore = services.BuildServiceProvider().GetRequiredService<RedisTicketStore>();
            });

            // JWT設定
            JwtConfigurableOptions jwtConfig = JwtConfigurableOptions.getJwtConfigurableOptions(jwtSettings);
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.JwtIssuer,
                ValidAudience = jwtConfig.JwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtConfig.JwtKey)),
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication().AddJwtBearer(configureOptions =>
            {
                // APIリクエスト時の認証ハンドラ
                configureOptions.ClaimsIssuer = jwtConfig.JwtIssuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = false;

                configureOptions.Events = new JwtBearerEvents
                {
                    // 認証成功時の処理
                    OnTokenValidated = context => Task.FromResult(0)
                };
            });

            // Swagger
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Diary-Sample API";
                    document.Info.Description = "ASP.NET Core web API";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "1-system-group",
                        Email = string.Empty,
                        Url = "https://github.com/1-system-group/Diary-Sample",
                    };
                };
                config.AddSecurity("JWT", Enumerable.Empty<string>(),
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the value: bearer {your JWT token}."
                    });

                config.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
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

            // Swagger generator・Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}");
            });
        }
    }
}