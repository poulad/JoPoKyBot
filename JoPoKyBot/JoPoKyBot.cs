using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Framework;
using Telegram.Bot.Types;

namespace JoPoKyBot
{
    public class JoPoKyBot : BotBase<JoPoKyBot>
    {
        private readonly ILogger<JoPoKyBot> _logger;

        public JoPoKyBot(IOptions<BotOptions<JoPoKyBot>> botOptions,
            ILogger<JoPoKyBot> logger)
            : base(botOptions)
        {
            _logger = logger;
        }

        public override Task HandleUnknownMessage(Update update)
        {
            _logger.LogWarning("Don't know how to handle update of type `{0}`", update.Type);
            return Task.CompletedTask;
        }

        public override Task HandleFaultedUpdate(Update update, Exception e)
        {
            _logger.LogError("Error in handling update of type `{0}`. {1}",
                update.Type, e.Message);
            return Task.CompletedTask;
        }
    }
}
