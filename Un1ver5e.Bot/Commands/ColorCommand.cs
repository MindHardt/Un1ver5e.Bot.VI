using Disqord;
using Disqord.Bot.Commands.Application;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Text.RegularExpressions;
using Un1ver5e.Bot.Services.Graphics;

namespace Un1ver5e.Bot.Commands
{
    public class ColorCommand : DiscordApplicationGuildModuleBase
    {
        [SlashCommand("цвет")]
        [Description("Как выглядят цвета.")]
        public async ValueTask<IResult> Color(
            [Name("Цвет"), Description("Цвет в шестнадцатиричном виде (например 5561F5).")]
            string colorHex)
        {
            if (Regex.IsMatch(colorHex, "[0-9a-fA-F]{6}") == false) return Results.Failure("Неверная строка цвета! Введите 24-битную шестнадцатиричную строку, например `c0ffee`, `3aebca`.");

            Color color = Convert.ToInt32(colorHex.ToUpper(), 16);
            //Using Convert is deprecated in most cases, but it is the only way of converting hexadecimal numbers

            var gen = Context.Services.GetRequiredService<IMonoColorImageGenerator>();

            Stream pic = await gen.GetImage(color);
            string fileName = $"{color}.jpg";

            var response = new LocalInteractionMessageResponse()
                .AddAttachment(new LocalAttachment(pic, fileName))
                .AddEmbed(new LocalEmbed()
                {
                    Color = color,
                    ImageUrl = $"attachment://{fileName}",
                    Description = $"Цвет `{color}`"
                });

            return Response(response);
        }


        [AutoComplete("цвет")]
        public void ColorAutocomplete(
            [Name("Цвет")] AutoComplete<string> colorHex)
        {
            if (colorHex.IsFocused && Regex.IsMatch(colorHex.RawArgument, "[0-9a-fA-F]{6}"))
            {
                colorHex.Choices.Add(colorHex.RawArgument);
            }
        }
    }
}
