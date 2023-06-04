@echo off
set /p choice="Please enter your choice (1: Build image only, 2: Deploy docker compose only, 3: Do both): "


if %choice%==1 (
    powershell.exe -Command "docker build -t lab_memoryleak ."
) else if %choice%==2 (
    powershell.exe -Command "docker-compose up -d"
) else if %choice%==3 (
    powershell.exe -Command "docker build -t lab_memoryleak ."
    powershell.exe -Command "docker-compose up -d"
)

pause
