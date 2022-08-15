### Un1ver5e.Bot.VI, aka MO (or its alter-ego MT) is a multi-purpose Discord bot written on top of [Disqord](https://github.com/Quahu/Disqord) library.

#### I am making this bot for several reasons:
- Learning C# (currently .NET 6)
- Making useful stuff for my friends and people in their guilds
- Pleasing my pride
- Playing board games with my friends who are far (WIP)

### If you want to host this bot yourself, feel free to, only thing you need is a PostgreSQL database. 
#### *Please note that this bot is developed in 🇷🇺 Russian language, as most of its users speak it*
#### The config for this bot is stored in a json, like this one below:
```json
{
  "discord_config": {
    "token": "YOUR TOKEN HERE",
    "prefixes": [ "mo" ],
    "owners": [ 298097988495081472 ],
    "logs_webhook_url": "YOUR WEBHOOK HERE (optional)"
  },
  "dice_service": {
    "cache_dice": "true",
    "cache_base": [ "1d2", "1d3", "1d4", "1d6", "1d8", "1d10", "1d12", "1d20", "1d100", "2d6" ]
  },
  "db_connstr_args": [ "Host=", "Username=", "Password=", "Database=" ]
}
```
#### Fill it with your own data and enjoy the show.

Technologies used:
- PostgreSQL + [EntityFrameworkCore](https://github.com/dotnet/efcore) (code-first apporoach)
- [Disqord](https://github.com/Quahu/Disqord)
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.DependencyInjection
- [Serilog](https://github.com/serilog/serilog)
- [ImageSharp](https://github.com/SixLabors/ImageSharp)

The bot is hosted on Raspberry Pi 4 powered by Ubuntu Server 21.10 (previously via Docker, now manual Deployment)

![Avatar goes here](/.github/Avatar.png)
