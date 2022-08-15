using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;
using Un1ver5e.Bot.Commands.DeleteThisButton;

namespace Un1ver5e.Bot.Commands
{
    public class InfoCommandModule : DiscordApplicationGuildModuleBase
    {
        private async ValueTask<LocalEmbed> GetInfoEmbed(IMember member)
        {
            int roleLimit = Disqord.Discord.Limits.Message.Embed.Field.MaxValueLength / 23; //23 for role mention length + line separator

            return new LocalEmbed()
            {
                Title = $"Информация о {member.Name}#{member.Discriminator}",
                ImageUrl = member.GetAvatarUrl(CdnAssetFormat.Png, 1024),
                Fields = new LocalEmbedField[]
                {
                    new LocalEmbedField()
                    {
                        Name = "Никнейм на сервере",
                        Value = $"`{member.Nick ?? "-"}`",
                        IsInline = true
                    },
                    new LocalEmbedField()
                    {
                        Name = "Упоминание",
                        Value = member.Mention,
                        IsInline = true
                    },
                    new LocalEmbedField()
                    {
                        Name = "Discord ID",
                        Value = $"`{member.Id.RawValue}`",
                    },
                    new LocalEmbedField()
                    {
                        Name = "Владелец бота",
                        Value = await Bot.IsOwnerAsync(member.Id) ? "**Да**" : "**Нет**",
                        IsInline = true
                    },
                    new LocalEmbedField()
                    {
                        Name = "Бот",
                        Value = member.IsBot ? "**Да**" : "**Нет**",
                        IsInline = true
                    },
                    new LocalEmbedField()
                    {
                        Name = "Присоединился",
                        Value = $"{member.JoinedAt.Value.ToRelativeDiscordTime()}"
                    },
                    new LocalEmbedField()
                    {
                        Name = $"Роли (первые {roleLimit})",
                        Value = string.Join('\n', member.RoleIds.Take(roleLimit).Select(id => $"<@&{id.RawValue}>"))
                    }
                },
            };
        }

        [UserCommand("Информация")]
        public async ValueTask<IResult> InfoMember(IMember member) 
            => Response(new LocalInteractionMessageResponse()
                .AddEmbed(await GetInfoEmbed(member))
                .AddDeleteThisButton(Context.AuthorId));

        [SlashCommand("инфо")]
        [Description("Информация о пользователе (аналог юзер-команды \"Информация\")")]
        public async ValueTask<IResult> InfoSlash(
            [Name("Пользователь"), Description("Интересующий пользователь")] IMember member)
            => Response(new LocalInteractionMessageResponse()
                .AddEmbed(await GetInfoEmbed(member))
                .AddDeleteThisButton(Context.AuthorId));
    }
}
