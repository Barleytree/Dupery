--------------------------
| DUPERY - INSTRUCTIONS  |
--------------------------

You can customise the pool of printable duplicants by editing the .json files in this folder (You must start the game
once with the mod enabled for the personality files to be created).

If you haven't got your file extensions enabled in Windows, you will not see the ".json" extension. That's OK.

(See Steam Workshop page for more info about this mod)

-------------------------
| 1. PERSONALITIES.json |
-------------------------

Use this to define your own custom duplicants. Must be a JSON dictionary with UNIQUE keys.

REMEMBER TO ADD COMMAS (,) AFTER EACH LINE, AND AFTER EACH ENTRY IN THE DICTIONARY!

e.g.

{
  "DRACULA": {
    "Name": "Dracula",
    "Description": "This is the description for {0}.",
    "Gender": "Male",
    "StressTrait": "UglyCrier",
    "JoyTrait": "SparkleStreaker",
    "HeadShape": 1,
    "Eyes": 5,
    "Hair": 1,
    "Body": 3,
  },
  "CARMILLA": {
    "Name": "Carmilla",
    "Description": "{0} is the name of the duplicant.",
    "Gender": "Female",
    "StressTrait": "Aggressive",
    "JoyTrait": "StickerBomber",
    "HeadShape": 3,
    "Eyes": 2,
    "Hair": 14,
    "Body": 1,
  },
}

There is an image in the mod folder which shows the numbers associated with all of the available "HeadShape", "Eyes",
"Hair" and "Body" properties.

Here are all of the possible "joy" and "stress" traits (as of writing):

STRESS TRAITS:
"Aggressive"
"StressVomiter"
"UglyCrier"
"BingeEater"

JOY TRAITS:
"BalloonArtist"
"SparkleStreaker"
"StickerBomber"
"SuperProductive"

----------------------------------
| 2. OVERRIDE_PERSONALITIES.json |
----------------------------------

Use this file to edit the 35 default Oxygen Not included personalities. Lets give Mi-Ma a new description and silly
hairstyle (hairstyle "5" is the grey Super Saiyan looking one):

...
  "MIMA": {
    "Printable": true,
    "Randomize": false,
    "Description": "This isn't even {0}'s final form!",
    "Hair": 5
  },
...

The "Printable" property can be toggled between "true" and "false" to restore or remove the duplicant from the random
selection pool (will not affect duplicants you've already printed, i.e. saved games).

The "Randomize" property can be toggled between "true" and "false" to activate or disable randomization of this
duplicant's appearance, stress and joy traits. The randomization will occur every time you load the game and, like
the "Printable" toggle, will not affect duplicants you've already printed.

--------------------------------
| 3. OVERRIDE.<someModId>.json |
--------------------------------

This file works exactly the same as the "OVERRIDE_PERSONALITIES.json" file, except it will contain personalities
that you have imported from other mods. This lets you customise the personalities to your preference.

------------------------------
| 4. accessory_id_cache.json |
------------------------------

Used to store IDs for accessories imported by other mods. If you have any duplicants in a colony which are using
custom accessories (e.g. hairstyles), then deleting this file may cause them loose their style. Otherwise, you can
ignore this file.

----------------------------------
| HELP, SOMETHING ISN'T WORKING! |
----------------------------------

The first thing to do if something isn't working is to delete any of the .json files above which might be causing a
problem. Deleting all the .json files will reset the mod to its original state and any custom characters in saves will
not be affected ("accessory_id_cache.json" is a special case, see above).

If the problem is a JSON syntax error, then you may be able to fix it by checking for any missing commas,
quotation marks, colons or curly braces in the files. JSON can be annoyingly pedantic with these things.

If you've tried the above and still can't solve the problem, then report it on the mod's Steam Workshop page or GitHub.
