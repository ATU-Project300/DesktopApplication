# Odyssey

A Windows desktop application to help find the right emulator for your games.

## Screenshots

![Application as of March 1st 2023](./media/01.03.23.png)

## For Users

### Setup

1. [Download latest version](https://github.com/ATU-Project300/DesktopApplication/releases/latest/download/setup.exe)

1. Put all your ROMs/ISOs into a folder making sure they are named similar to the game title.

1. Configure the path to the game folder

    * Optional: configure the paths to already installed emulators

1. Use the Emulator Manager to install the supported emulators

## For Developers

Please feel free to create an issue and/or PR if you notice anything incorrect here :)

### Trello

[Link](https://trello.com/b/0Fzksv3i/desktop-application-tasks)


### API Data Handling

Firstly, the `InitializeApiData` function is called.

This function exists only because we need something
to hold the async stuff.

It waits for the API to return all of its data (JSON)
and passes it to the `OccupyListVarGames` function.

This function simply iterates through every game
in the list, adding them as "`Game`" objects to our
"class local" list of Game objects called "`MyGames`".

We then set the DataContext of the application to
the `GameViewModel` class and through the constructor
of that class we pass our now full "`MyGames`" list.

This then allows us to set the contents of
the `GameViewModel.MyGames` list to the full
list containing our data.

Emulator data is handled the same way.

### ROMs

ROM files (ISO or Folders) do not have their paths stored
directly, their paths are also not modifiable.
ROM files are searched for using a set of functions
and a likely path to a ROM is determined and used.

This begins in the `StartGame` fucntion which is responsible
for constructing a launch command from the selected game
using the other functions to be mentioned.
The first being `PickEmulator`.

`PickEmulator` takes the argument of `game`.
A variable named `settings` is then created by
prepending "path" to `game.Emulator`, this creates,
for example, "pathXenia".

If a setting matching this name is not found, we return invalid.
Otherwise we return the value stored in the setting, provided the path
is valid. Following the above example, we would be returning
the value of `Properties.Settings.Default.pathXenia`.

Back in `StartGame`, if the returned string is "Invalid",
we set a boolean to true to indicate this.

In any other case, `FindGame` is called with the argument of `game`.
`FindGame` checks first if the emulator for the game is "RPCS3" and if so,
uses `FindFolder` to search for the game files. Otherwise `FindFile` is used.
The result of whichever function is used is returned.

`FindFile` and `FindFolder` are fairly generic functions
which simply iterate through each file or folder
in the directory (provided as an argument) and passes
each file or folder name to the `CompareStrings` function
which compares the file or folder name to the 
game title (also provided as an argument).

`CompareStrings` splits its two arguments into
Lists of strings of individual words and then 
iterates through each individual word in the
two lists and compares them, ignoring case.
In the case of a match, the word is added
to a `matchingWords` list.

After each string pair is compared, a "likeless"
is determined by dividing the number of matched
words and the total number of words, this is then
multiplied by 100 to be a percentage.
This likeness is returned as a double.

In the `FindFile` and `FindFolder` functions, this likeness
rating is used to determine if found path is correct
for the game based on its title. The likeness rating is
adjusted based on the number of characters in a game title.
Currently, for games over 7 characters in title length,
we require a likeness rating greater than 70%.
For games with shorter titles, this is reduced by 20%
to increase the likelyhood of the game being found.

### Settings

Settings are handled through the settings
framework provided by WPF.

Some functions were written to allow for saving
and restoring of settings along with verification of settings.

Measures have been taken to avoid hardcoding settings for saving
and loading. An example of this is for emulator paths where each
path related setting is expected to have the following naming structure:

`path[EmulatorName]` for the `Settings.settings` file.

`path[EmulatorName]TxtBx` for the text box which will contain the
value of the setting.

This is also considered in the game path setting which follows similar rules:

`pathGameFolder` for the `Settings.settings` file.

The `SaveSettings` function simply takes the values
from the checkboxes, text boxes etc. and assigns them
to the `Odyssey.Properties.Settings.Default.SettingName` for
the given setting. This function is called on the click event
for the "Apply" button in the settings panel.

The `LoadSettings` function checks the WPF managed
settings for values and applies them as necessary.

The `VerifySettings` function passes information about
each setting to the `VerifySetting` function which runs
a series of checks on the provided settings and acts accordingly.

`VerifySettings` uses a loop to iterate through the relevant TextBoxes.
For each TextBox which is named `path*`, determine the emulator name by removing "path" from its name. Then use the emulator name to find the emulator in the `MyEmulators` list, where we can then find the `Executable` property to pass to the `VerifySetting` function.

### Theming

Themes are managed through a `Theming` function which currently
takes a bool to enable or disable the dark theme.

Some variables are initally assigned to eventually hold colours.

The if statement then checks if the `dark` bool is True and in that case it assigns the dark colour scheme colours to the variables at the beginning of the function.

Loops are then used to iterate through all relevant elements and set the colours.

This could be slower than manually setting the colours for all elements individually but it's significantly faster for development as theming newly added elements is no longer a concern using this method.

# Credits

* http://materialdesigninxaml.net/

* https://mahapps.com/

* https://github.com/jrsoftware/issrc

* https://www.7-zip.org/