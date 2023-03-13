
[Setup]
AppName=Odyssey
AppVersion=1.0
WizardStyle=modern
DefaultDirName={autopf}\Odyssey
DefaultGroupName=Odyssey
UninstallDisplayIcon={app}\Odyssey.exe
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output

[Files]
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

[Icons]
Name: "{group}\Odyssey"; Filename: "{app}\Odyssey.exe"
