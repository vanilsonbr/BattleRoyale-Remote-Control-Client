cd /d %~dp0

@echo OFF
echo Stopping old service version...
net stop "BattleRoyalleRemoteController"
echo Uninstalling old service version...
sc delete "BattleRoyalleRemoteController"

echo Installing service...

sc create BattleRoyalleRemoteController binpath= \"%~dp0\BattleRoyalle.RemoteController.Client.exe\" start= auto
echo Starting service...
net start "BattleRoyalleRemoteController"
pause