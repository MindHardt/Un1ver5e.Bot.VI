using Arklens.Entities;
using Arklens.Entities.Properties.Attributes;
using Disqord;
using System.Text;

namespace Un1ver5e.Bot.Services.Arklens
{
    internal static class CharacterDisplayExtensions
    {
        private static string GetSignumSignature(int value)
        {
            return Math.Sign(value) switch
            {
                -1 => "<:downvote:994602040720359524>",
                0 => "➖",
                1 => "<:upvote:994602042242895913>",
                _ => throw new NotImplementedException()
            };
        }
        private static string GetAttributeSignature(string emoji, AttributeStat attribute)
        {
            return $"{attribute.Acronym}{emoji}: {attribute.GetSum()}{GetSignumSignature(attribute.GetSum() - attribute.GetRawValue())}";
        }
        private static string GetAllAttributesSignature(Character character)
        {
            StringBuilder sb = new();
            sb.Append(GetAttributeSignature("💪", character.Strength));
            sb.Append('\n');
            sb.Append(GetAttributeSignature("🏃", character.Dexterity));
            sb.Append('\n');
            sb.Append(GetAttributeSignature("🩸", character.Constitution));
            sb.Append('\n');
            sb.Append(GetAttributeSignature("🧠", character.Intelligence));
            sb.Append('\n');
            sb.Append(GetAttributeSignature("🦉", character.Wisdom));
            sb.Append('\n');
            sb.Append(GetAttributeSignature("👄", character.Charisma));
            return sb.ToString();
        }

        public static LocalEmbed CreateDisplay(this Character character)
        {
            return new LocalEmbed()
            {
                ThumbnailUrl = character.AvatarUrl,
                Author = new LocalEmbedAuthor()
                {
                    Name = character.Name
                },
                Fields =
                {
                    new LocalEmbedField()
                    {
                        Name = "Атрибуты",
                        IsInline = true,
                        Value = GetAllAttributesSignature(character).AsCodeBlock()
                    }
                }
            };
        }

    }
}
