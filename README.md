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

### Settings

Settings are handled through the settings framework provided by WPF.

Some functions were written to allow for saving and restoring of settings.

The `SaveSettings` function simply takes the values from the checkboxes, text boxes etc. and assigns them to the `Odyssey.Properties.Settings.Default.SettingName` for the given setting. This function is called on the click event
for the "Apply" button in the settings panel.

The `LoadSettings` function checks the WPF managed settings for values and applies
them as necessary.

### Theming

Themes are managed through a `Theming` function which currently takes a bool
to enable or disable the dark theme.

Either the light or dark theme could be customised or replaced through this function.


# Credits

http://materialdesigninxaml.net/