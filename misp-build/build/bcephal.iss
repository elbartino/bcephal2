; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{0D0D4FAF-B685-4A2A-B7F2-0CB1E75938CA}
AppName=B-Cephal Client
AppVerName=B-Cephal Client 5.0.0-alpha-20160701
AppPublisher=Moriset & co
AppPublisherURL=http://www.bcephal.com/
AppSupportURL=http://www.bcephal.com/support
AppUpdatesURL=http://www.bcephal.com/update
DefaultDirName={pf}\Moriset\Bcephal\Client
DefaultGroupName=Moriset\Bcephal\Client
OutputDir=..\
OutputBaseFilename=B-Cephal Client 5.0.0-alpha-20160701
Compression=lzma
SolidCompression=true
SetupIconFile=bcephal.ico
;WizardImageFile=bcephal.png

[Languages]
Name: english; MessagesFile: compiler:Default.isl
Name: french; MessagesFile: compiler:Languages\French.isl

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
   
[Files]
Source: ..\setup\bin\*; DestDir: {app}\bin; Flags: ignoreversion recursesubdirs createallsubdirs

Source: ..\setup\bin\redist\x64\officeviewer.ocx; DestDir: {app}\redist\x64; Check: IsWin64; Flags: restartreplace sharedfile regserver 64bit noregerror
Source: ..\setup\bin\redist\x64\EDOfficeViewerX.dll; DestDir: {app}\redist\x64; Check: IsWin64; Flags: restartreplace sharedfile regserver 64bit noregerror

Source: ..\setup\bin\redist\x64\officeviewer.ocx; DestDir: {app}\redist\x64; Check: "not IsWin64"; Flags: restartreplace sharedfile noregerror
Source: ..\setup\bin\redist\x64\EDOfficeViewerX.dll; DestDir: {app}\redist\x64; Check: "not IsWin64"; Flags: restartreplace sharedfile noregerror

Source: ..\setup\bin\redist\x86\officeviewer.ocx; DestDir: {app}\redist\x86; Flags: restartreplace sharedfile regserver 32bit noregerror
Source: ..\setup\bin\redist\x86\EDOfficeViewerX.dll; DestDir: {app}\redist\x86; Flags: restartreplace sharedfile regserver 32bit noregerror 


[Icons]
Name: {group}\Bcephal; Filename: {app}\bin\bcephal-client.exe; WorkingDir: {app}\bin
Name: {group}\{cm:UninstallProgram,Bcephal}; Filename: {uninstallexe}
Name: {commondesktop}\Bcephal; Filename: {app}\bin\bcephal-client.exe; WorkingDir: {app}\bin; Tasks: desktopicon

[Run]   
;Filename: {app}\bin\bcephal.exe; Description: {cm:LaunchProgram,Bcephal}; Flags: skipifdoesntexist waituntilidle postinstall

[UninstallDelete]
Name: {app}\EDOfficeViewerX.dll; Type: files
Name: {app}\*; Type: filesandordirs