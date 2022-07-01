using Disqord;
using Disqord.Bot;
using Disqord.Extensions.Interactivity.Menus;
using Disqord.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Un1ver5e.Bot.Services.Tags
{
    internal class TagModifyView : ViewBase
    {
        private readonly DiscordBotBase _bot;
        private readonly Tag _tag;

        public TagModifyView(DiscordBotBase bot, Tag tag) : base(default)
        {
            _bot = bot;
            _tag = tag;
            ReloadMessage();
            AddComponent(new ButtonViewComponent(SwitchPublicity)
            {
                Label = "🌝🌚",
                Style = LocalButtonComponentStyle.Primary
            }); 
        }

        public async ValueTask SwitchPublicity(ButtonEventArgs e)
        {
            if (await _bot.IsOwnerAsync(e.AuthorId) == false) throw new AccessViolationException("Менять публичность могут только администраторы!");
            _tag.IsPublic = !_tag.IsPublic;
            ReloadMessage();
        }

        private void ReloadMessage() => MessageTemplate = m => m.AddEmbed(_tag.GetDisplay(_bot));
    }
}
