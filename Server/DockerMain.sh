#!/bin/bash

echo "Entered main script."

ServerDir=/var/www/uremote
URemoteData=/remotely-data

AppSettingsVolume=/uremote-data/appsettings.json
AppSettingsWww=/var/www/uremote/appsettings.json

if [ ! -f "$AppSettingsVolume" ]; then
	echo "Copying appsettings.json to volume."
	cp "$AppSettingsWww" "$AppSettingsVolume"
fi

if [ -f "$AppSettingsWww" ]; then
	rm "$AppSettingsWww"
fi

ln -s "$AppSettingsVolume" "$AppSettingsWww"

echo "Starting URemote server."
exec /usr/bin/dotnet /var/www/uremote/URemote_Server.dll