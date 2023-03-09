# Odyssey

A Windows desktop application to help find the right emulator for your games.

## Screenshots

![Application as of March 1st 2023](./media/01.03.23.png)

## For Users

### Setup

1. [Download latest version](https://github.com/ATU-Project300/DesktopApplication/releases/latest/download/setup.exe)

1. Put all your ROMs/ISOs into a folder making sure they are named similar to the game title.

1. Configure the paths to your emulators and game folder in the settings and start playing.

## For Developers

Please feel free to create an issue and/or PR if you notice anything incorrect here :)

### Trello

[Link](https://trello.com/b/0Fzksv3i/desktop-application-tasks)

### Adding Emulators

Note: Settings are added in pairs of `TextBlock` and `TextBox`, which will both be in the same Grid Row.

1. In the XAML (code view), locate the `TabPanel` named "Settings".

1. In the `Grid.RowDefinitions` tags, add another row of height `1*` (Above the one of height `2*`).


1. Duplicate the last emulator's pair of settings and increase the `Grid.Row` by 1 for the new pair of elements.

1. For any of the elements below the new settings (which are within the same Grid) increase the `Grid.Row` by `1` also.

1. In the newly created settings pair, replace any instances of an emulator name with that of your new emulator.

    Example, replacing `PPSSPPtxtbx` with `Xeniatxtbx`. Also `pathPPSSPPTxtBx` with `pathXeniaTxtBx`.
    
1. In the `Settings.settings` file, add a setting for your emulator, following the naming convention.

1. From here, add your games to your NoSQL database.


### API Data Handling

Firstly, the `InitializeApiData` function is called.

This function exists only because we need something
to hold the async stuff.

It waits for the API to return all of its data (JSON)
and passes it to the `OccupyListVar` function.

This function simply iterates through every game
in the list, adding them as "`Game`" objects to our
"class local" list of Game objects called "`myGames`".

We then set the DataContext of the application to
the `GameViewModel` class and through the constructor
of that class we pass our now full "`myGames`" list.

This then allows us to set the contents of
the `GameViewModel.myGames` list to the full
list containing our data.

### ROMs

ROM files (ISO or Folders) do not have their paths stored
directly, their paths are also not modifiable.
ROM files are searched for using a set of functions
and a likely path to a ROM is determined and used.

This begins in the `StartGame` fucntion which is responsible
for constructing a launch command from the selected game
using the other functions to be mentioned.
The first being `FindGame`.

`FindGame` takes a `Game` type as an argument. It first
checks if the game isn't null, if this check fails, the
function returns null.

It then checks if the Emulator associated with the game
which was passed as an argument is "RPCS3" using the
`PickEmulator` function, which returns the emulator
for a game. In the case that the game is to be
launcher with RPCS3, we return the result of the
`FindFolder` function with the Game Folder path and game
title as arguments. Otherwise we use the 
`FindFile` function which takes the same arguments.

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

### Theming

Themes are managed through a `Theming` function which currently
takes a bool to enable or disable the dark theme.

Either the light or dark theme could be customised or
replaced through this function as the colours in use
are defined in variables.


# Credits

* http://materialdesigninxaml.net/

* https://mahapps.com/

* https://github.com/jrsoftware/issrc