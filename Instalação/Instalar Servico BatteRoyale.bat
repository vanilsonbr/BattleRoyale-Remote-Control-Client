@echo OFF
echo Stopping old service version...
net stop "BattleRoyale Remote Controller"
echo Uninstalling old service version...
sc delete "BattleRoyale Remote Controller"

echo Installing service...
sc create "BattleRoyale Remote Controller" binpath= "%cd%\BatteRoyale.RemoteController.Client.exe" start= auto displayname= "BattleRoyale remote controller service"
echo Starting server complete
pause