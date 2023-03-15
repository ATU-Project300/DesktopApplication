
[Setup]
AppName=Odyssey
AppVersion=0.2
WizardStyle=modern
DefaultDirName={autopf}\Odyssey
DefaultGroupName=Odyssey
UninstallDisplayIcon={app}\Odyssey.exe
Compression=lzma2
SolidCompression=yes
OutputDir=../../../../

[Files]
Source: "*"; DestDir: "{app}"

[Icons]
Name: "{group}\Odyssey"; Filename: "{app}\Odyssey.exe"
