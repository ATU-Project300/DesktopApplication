# Odyssey

A Windows desktop application to help find the right emulator for your games.

## For Users

### Setup

1. Download latest version

1. Profit

## For Developers

### Trello

[Link](https://trello.com/b/0Fzksv3i/desktop-application-tasks)

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

The `SaveSettings` function simply takes the values
from the checkboxes, text boxes etc. and assigns them
to the `Odyssey.Properties.Settings.Default.SettingName` for
the given setting. This function is called on the click event
for the "Apply" button in the settings panel.

The `LoadSettings` function checks the WPF managed
settings for values and applies them as necessary.

The `VerifySettings` function uses a local function named
`TxBxLengthCheck` to check the length of aTextBox (passed as argument "t")
for a given emulator (passed as argument "emulator").
If the length of `t` is less than 4 characters, we display
a message that something may be incorrect with the provided path.

### Theming

Themes are managed through a `Theming` function which currently
takes a bool to enable or disable the dark theme.

Either the light or dark theme could be customised or
replaced through this function as the colours in use
are defined in variables.


# Credits

http://materialdesigninxaml.net/