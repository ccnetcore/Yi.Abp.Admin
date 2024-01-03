#!/bin/bash
./end.sh
nohup dotnet Yi.Abp.Web.dll > /dev/null 2>&1 &
echo "Yi-启动成功!"
