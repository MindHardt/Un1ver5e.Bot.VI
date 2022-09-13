using Disqord;
using Disqord.Bot.Commands.Application;
using Microsoft.EntityFrameworkCore;
using Qmmands;
using Un1ver5e.Bot.Commands.DeleteThisButton;
using Un1ver5e.Bot.Models;
using Un1ver5e.Bot.Services.Database;

namespace Un1ver5e.Bot.Commands
{
    public class InfoCommand : DiscordApplicationGuildModuleBase
    {
        private readonly BotContext _dbctx;

        public InfoCommand(BotContext dbctx)
        {
            _dbctx = dbctx;
        }

        //INFO
        [UserCommand("Информация")]
        public async ValueTask<IResult> InfoMember(IMember member)
        {
            int tagLimit = (await _dbctx.Users.FirstOrDefaultAsync(u => u.Id == Context.AuthorId.RawValue) ?? new UserData()).TagsCountLimit;
            int tagCount = await _dbctx.Tags.Where(t => t.AuthorId == Context.AuthorId.RawValue).CountAsync();

            int roleLimit = Disqord.Discord.Limits.Message.Embed.Field.MaxValueLength / 23; //23 for role mention length + line separator
            var embed = new LocalEmbed()
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
                        Name = $"Теги",
                        Value = $"{tagCount}/{tagLimit}"
                    },
                    new LocalEmbedField()
                    {
                        Name = $"Роли (первые {roleLimit})",
                        Value = string.Join('\n', member.RoleIds.Take(roleLimit).Select(id => $"<@&{id.RawValue}>"))
                    }
                },
            };
            return Response(new LocalInteractionMessageResponse()
                .AddEmbed(embed)
                .AddDeleteThisButton(Context.AuthorId));
        }
    }
}
