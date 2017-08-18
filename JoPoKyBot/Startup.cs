using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecurrentTasks;
using Telegram.Bot.Framework;

namespace JoPoKyBot
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDataProtection();

            services.AddTelegramBot<JoPoKyBot>(_configuration.GetSection("JoPoKyBot"))
                .AddUpdateHandler<StartCommand>()
                .AddUpdateHandler<BubbleGunnerGameHandler>()
                .Configure();
            services.AddTask<BotUpdateGetterTask<JoPoKyBot>>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ILogger logger = loggerFactory.CreateLogger<Startup>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                    appBuilder.Run(context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        return Task.CompletedTask;
                    })
                );
            }

            app.UseCors(builder => builder
                .WithOrigins("https://pouladpld.github.io")
                .WithMethods(HttpMethods.Get, HttpMethods.Post)
                .DisallowCredentials()
            );

            app.UseTelegramGame<JoPoKyBot>();

            if (env.IsDevelopment())
            {
                app.UseTelegramBotLongPolling<JoPoKyBot>();
                app.StartTask<BotUpdateGetterTask<JoPoKyBot>>(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3));
                logger.LogInformation("Update getting task is scheduled for bot " + nameof(JoPoKyBot));
            }
            else
            {
                app.UseTelegramBotWebhook<JoPoKyBot>();
                logger.LogInformation("Webhook is set for bot " + nameof(JoPoKyBot));
            }
        }
    }
}
