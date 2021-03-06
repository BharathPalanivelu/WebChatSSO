﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;

namespace WebChatSSO.Dialogs
{
    public class DisplayTokenDialog : ComponentDialog
    {
        string _connectionName;

        public DisplayTokenDialog(IConfiguration configuration)
            : base(nameof(DisplayTokenDialog))
        {
            _connectionName = configuration.GetSection("ConnectionName")?.Value;

            var steps = new WaterfallStep[] {
                DisplayTokenAsync
            };

            AddDialog(new WaterfallDialog(nameof(DisplayTokenDialog), steps));
        }

        private async Task<DialogTurnResult> DisplayTokenAsync(WaterfallStepContext context, CancellationToken cancellationToken)
        {
            var adapter = context.Context.Adapter as IUserTokenProvider;

            var token = await adapter.GetUserTokenAsync(context.Context, _connectionName, null, cancellationToken);

            if (token == null)
            {
                await context.Context.SendActivityAsync("User has no cached token.");
            }
            else
            {
                await context.Context.SendActivityAsync($"Here is your token: {token.Token}");
            }

            return await context.EndDialogAsync(null, cancellationToken);
        }
    }
}
