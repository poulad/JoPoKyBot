using Microsoft.AspNetCore.DataProtection;
using Telegram.Bot.Framework;

namespace JoPoKyBot
{
    public class BubbleGunnerGameHandler : GameHandlerBase
    {
        public BubbleGunnerGameHandler(IDataProtectionProvider protectionProvider)
            : base(protectionProvider, Constants.GameShortName)
        {

        }

        private static class Constants
        {
            public const string GameShortName = "bubblegunner";
        }
    }
}
