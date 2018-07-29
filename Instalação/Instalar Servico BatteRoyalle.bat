cd /d %~dp0

@echo OFF
echo Stopping old service version...
net stop "BattleRoyaleRemoteController"
echo Uninstalling old service version...
sc delete "BattleRoyaleRemoteController"

echo Installing service...

sc create BattleRoyaleRemoteController binpath= \"%~dp0\BattleRoyalle.RemoteController.Client.exe\" start= auto
echo Starting server complete
pause