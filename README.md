# [CS2] Auto-Rotate-Maps-GoldKingZ (1.0.0)

### Auto Rotate Maps Depend Players In Server

![Mode3](https://github.com/oqyh/cs2-Auto-Rotate-Maps-GoldKingZ/assets/48490385/46895839-162d-43af-b423-40d11fc598aa)

![Mode1](https://github.com/oqyh/cs2-Auto-Rotate-Maps-GoldKingZ/assets/48490385/613cade1-cc9b-4476-b84b-2f0589d3aeb2)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## .:[ Configuration ]:.

> [!CAUTION]
> Config Located In ..\addons\counterstrikesharp\plugins\Auto-Rotate-Maps-GoldKingZ\config\config.json      
>                                                                                                                                                                                                  
> Map Config Located In ..\addons\counterstrikesharp\plugins\Auto-Rotate-Maps-GoldKingZ\config\RotationServerMapList.txt                                           
>                                                                                                                                                                                                 

```json
{
  // (0) = Disable
  // (1) = Get Maps From Top To Bottom In RotationServerMapList.txt
  // (2) = Get Random + Not Duplicated Maps In RotationServerMapList.txt
  "RotateMode": 1,

  //Wait X Mins Before You Do RotateMode 
  "RotateXTimerInMins": 5,

  //Do RotateMode When X Players In Server OR Less
  "RotateWhenXPlayersInServerORLess": 0,

//-----------------------------------------------------------------------------------------

  //Enable Logging Text Located In Auto-Rotate-Maps-GoldKingZ/logs/ ?
  "TextLog_Enable": false,

  //Log Message Format
  //{TIME} == Time
  //{DATE} == Date
  //{MAP} == Which Map Name Has Changed To
  "TextLog_MessageFormat": "[{DATE} - {TIME}] Server Has Less Players Changing Map To [{MAP}]",

  //Date and Time Formate
  "TextLog_DateFormat": "MM-dd-yyyy",
  "TextLog_TimeFormat": "HH:mm:ss",

  //Auto Delete Logs If More Than X (Days) Old
  "TextLog_AutoDeleteLogsMoreThanXdaysOld": 0,

//-----------------------------------------------------------------------------------------

  //Send Log To Discord Via WebHookURL
  // (0) = Disable
  // (1) = Text Only (Result Image : https://github.com/oqyh/Auto-Rotate-Maps-GoldKingZ/blob/main/Resources/Mode1.png?raw=true)
  // (2) = Text With Saparate Date And Time From Message (Result Image : https://github.com/oqyh/Auto-Rotate-Maps-GoldKingZ/blob/main/Resources/Mode2.png?raw=true)
  // (3) = Text With Saparate Date And Time From Message + Server Ip In Footer (Result Image : https://github.com/oqyh/Auto-Rotate-Maps-GoldKingZ/blob/main/Resources/Mode3.png?raw=true)
  "DiscordLog_EnableMode": 0,

  //Discord Log Message Format
  //{TIME} == Time
  //{DATE} == Date
  //{MODE} == Which Method Did It Used
  "DiscordLog_MessageFormat": "[{DATE} - {TIME}] Server Has Less Players Changing Map To [{MAP}]",

  //Date and Time Formate
  "DiscordLog_DateFormat": "MM-dd-yyyy",
  "DiscordLog_TimeFormat": "HH:mm:ss",

  //If DiscordLog_EnableMode (2) or (3) How Would You Side Color Message To Be Check (https://www.color-hex.com/) For Colors
  "DiscordLog_SideColor": "00FFFF",

  //Discord WebHookURL
  "DiscordLog_WebHookURL": "https://discord.com/api/webhooks/XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",

  //If DiscordLog_EnableMode (3) Image Url Footer
  "DiscordLog_FooterImage": "https://github.com/oqyh/cs2-Auto-Rotate-Maps-GoldKingZ/blob/main/Resources/serverip.png?raw=true",

}
```

## .:[ Change Log ]:.
```
(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
