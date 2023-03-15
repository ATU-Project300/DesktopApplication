
[Setup]
AppName=Odyssey
AppVersion=0.1
WizardStyle=modern
DefaultDirName={autopf}\Odyssey
DefaultGroupName=Odyssey
UninstallDisplayIcon={app}\Odyssey.exe
Compression=lzma2
SolidCompression=yes
OutputDir=../../../../

[Files]
Source: "*"; DestDir: "{app}"
Source: "Odyssey.exe"; DestDir: "{app}"
Source: "Odyssey.dll"; DestDir: "{app}"
Source: "Odyssey.runtimeconfig.json"; DestDir: "{app}"
Source: "API.dll"; DestDir: "{app}"
Source: "API.exe"; DestDir: "{app}"   
Source: "ControlzEx.dll"; DestDir: "{app}"    
Source: "MahApps.Metro.dll"; DestDir: "{app}"     
Source: "MaterialDesignColors.dll"; DestDir: "{app}"
Source: "MaterialDesignThemes.MahApps.dll"; DestDir: "{app}"
Source: "MaterialDesignThemes.Wpf.dll"; DestDir: "{app}"   
Source: "Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"
Source: "Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "7za.exe"; DestDir: "{app}"
Source: "7za.dll"; DestDir: "{app}"
Source: "7zxa.dll"; DestDir: "{app}"

[Icons]
Name: "{group}\Odyssey"; Filename: "{app}\Odyssey.exe"
