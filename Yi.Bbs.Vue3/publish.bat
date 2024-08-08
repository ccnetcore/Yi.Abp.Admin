@echo on

set SERVER_USER=root
set SERVER_IP=ccnetcore.com
set FILE_PATH=publish_bbs_02.zip
set REMOTE_PATH=/home/yi/build/publish_bbs_02.zip
set REMOTE_COMMAND="cd /home/yi/bbs&&pwd&&unzip -o /home/yi/build/publish_bbs_02.zip  -d ./"
set sevenzip_Path="D:\Program Files\7-Zip\7z.exe"

echo start
echo 1-build-start
:: npm run build
echo 1-build-end
echo 2-zip-start

%sevenzip_Path% a ./publish_bbs_02.zip ./dist/*
:: tar -cvf publish_bbs_02.zip -C "dist" "*"
echo 2-zip-end
echo 3-publish-start
scp %FILE_PATH% %SERVER_USER%@%SERVER_IP%:%REMOTE_PATH%
ssh %SERVER_USER%@%SERVER_IP% %REMOTE_COMMAND%
echo 3-publish-end
echo end
pause