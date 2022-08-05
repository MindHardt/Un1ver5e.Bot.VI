using Disqord;
using Disqord.Bot.Commands.Application;
using Qmmands;

namespace Un1ver5e.Bot.Commands
{
    public class AvatarCommand : DiscordApplicationGuildModuleBase
    {
        [UserCommand("Аватар")]
        public IResult Avatar(IMember member)
        {
            string url = member.GetAvatarUrl(CdnAssetFormat.Png, 1024);
            string nick = member.Nick ?? member.Name;

            LocalEmbed embed = new()
            {
                ImageUrl = url,
                Description = url,
                Title = $"Аватар {nick}"
            };

            return Response(embed);
        }


    }
}
