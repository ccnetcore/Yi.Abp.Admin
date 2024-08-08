@echo on

set SERVER_USER=root
set SERVER_IP=ccnetcore.com
set FILE_PATH=publish_02.zip
set REMOTE_PATH=/home/yi/build/publish_02.zip
set REMOTE_COMMAND="cd /home/yi/net8&&pwd&&unzip -o /home/yi/build/publish_02.zip  -d ./&&./start.sh"
set sevenzip_Path="D:\Program Files\7-Zip\7z.exe"

echo start
echo 1-build-start
:: dotnet publish
echo 1-build-end
echo 2-zip-start

%sevenzip_Path% a ./publish_02.zip ./src/Yi.Abp.Web/bin/Release/net8.0/linux-x64/publish/*
:: tar -cvf publish_02.zip -C "dist" "*"
echo 2-zip-end
echo 3-publish-start
scp %FILE_PATH% %SERVER_USER%@%SERVER_IP%:%REMOTE_PATH%
ssh %SERVER_USER%@%SERVER_IP% %REMOTE_COMMAND%
echo 3-publish-end
echo end
pause