# CrabGameBetterChat
A BepInEx mod for Crab Game that adds server-side formatting capabilities to the chat.

## What kind of formatting capabilities are you yapping about?
Upon starting the game with this mod and creating a lobby, you will see that any chat messages sent by players will now include the player's player number next to their name in parentheses.

To configure the formatting, you can head to the "BepInEx/config/lammas123.BetterChat.cfg" config file, and from there use the following default formatting options:
- USERNAME -> The player's username.
- PLAYER_NUMBER -> The player's unique number.
- CLIENT_ID -> The player's steam id/client id.
- LIVING -> Whether the player is alive or dead.

You can also change the text shown for the LIVING formatting type whether the player is alive or dead.

Other mods may also add more formats, such as [PermissionGroups](https://github.com/lammas321/CrabGamePermissionGroups).

### Limitations
Due to limitations, unless the player sending the message also has the mod, their messages won't be formatted on their end.