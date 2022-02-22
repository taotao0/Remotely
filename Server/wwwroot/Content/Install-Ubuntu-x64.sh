#!/bin/bash
HostName=
Organization=
GUID=$(cat /proc/sys/kernel/random/uuid)
UpdatePackagePath=""


Args=( "$@" )
ArgLength=${#Args[@]}

for (( i=0; i<${ArgLength}; i+=2 ));
do
    if [ "${Args[$i]}" = "--uninstall" ]; then
        systemctl stop uremote-agent
        rm -r -f /usr/local/bin/URemote
        rm -f /etc/systemd/system/uremote-agent.service
        systemctl daemon-reload
        exit
    elif [ "${Args[$i]}" = "--path" ]; then
        UpdatePackagePath="${Args[$i+1]}"
    fi
done

UbuntuVersion=$(lsb_release -r -s)

wget -q https://packages.microsoft.com/config/ubuntu/$UbuntuVersion/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get update
apt-get -y install apt-transport-https
apt-get update
apt-get -y install dotnet-runtime-5.0
rm packages-microsoft-prod.deb

apt-get -y install libx11-dev
apt-get -y install libxrandr-dev
apt-get -y install unzip
apt-get -y install libc6-dev
apt-get -y install libgdiplus
apt-get -y install libxtst-dev
apt-get -y install xclip
apt-get -y install jq
apt-get -y install curl


if [ -f "/usr/local/bin/URemote/ConnectionInfo.json" ]; then
    SavedGUID=`cat "/usr/local/bin/URemote/ConnectionInfo.json" | jq -r '.DeviceID'`
     if [[ "$SavedGUID" != "null" && -n "$SavedGUID" ]]; then
        GUID="$SavedGUID"
    fi
fi

rm -r -f /usr/local/bin/URemote
rm -f /etc/systemd/system/uremote-agent.service

mkdir -p /usr/local/bin/URemote/
cd /usr/local/bin/URemote/

if [ -z "$UpdatePackagePath" ]; then
    echo  "Downloading client..." >> /tmp/URemote_Install.log
    wget $HostName/Content/URemote-Linux.zip
else
    echo  "Copying install files..." >> /tmp/URemote_Install.log
    cp "$UpdatePackagePath" /usr/local/bin/URemote/URemote-Linux.zip
    rm -f "$UpdatePackagePath"
fi

unzip ./URemote-Linux.zip
rm -f ./URemote-Linux.zip
chmod +x ./URemote_Agent
chmod +x ./Desktop/URemote_Desktop


connectionInfo="{
    \"DeviceID\":\"$GUID\", 
    \"Host\":\"$HostName\",
    \"OrganizationID\": \"$Organization\",
    \"ServerVerificationToken\":\"\"
}"

echo "$connectionInfo" > ./ConnectionInfo.json

curl --head $HostName/Content/URemote-Linux.zip | grep -i "etag" | cut -d' ' -f 2 > ./etag.txt

echo Creating service... >> /tmp/URemote_Install.log

serviceConfig="[Unit]
Description=The URemote agent used for remote access.

[Service]
WorkingDirectory=/usr/local/bin/URemote/
ExecStart=/usr/local/bin/URemote/URemote_Agent
Restart=always
StartLimitIntervalSec=0
RestartSec=10

[Install]
WantedBy=graphical.target"

echo "$serviceConfig" > /etc/systemd/system/uremote-agent.service

systemctl enable uremote-agent
systemctl restart uremote-agent

echo Install complete. >> /tmp/URemote_Install.log
