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

## Why do players in my lobby see their own messages twice?
Currently, the simplest way to show player's formatted messages (who don't have BetterChat) without resending the entire chat window every time they send a message (which can take up a lot of bandwidth) is to just let them see their chat messages twice.

This is due to the messages players send being added to the chat on their end before the server ever gets to see it (and thus be able to format it), and then forwarding that message to all other players.

Players that join your lobby using BetterChat will not add their own messages to their chat before the host, thus preventing the issue with seeing their own messages twice.
