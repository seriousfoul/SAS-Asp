@echo off
cd /d "C:\程式碼練習\面試\Steam 遊戲統計與圖表視覺化-ASP\Steam-Analyze-Statistics-ASP\Models"
echo 現在執行ASP.NET專案的更新


REM 執行 Python 程式
python findGameData.py

cd /d "C:\程式碼練習\面試\Steam 遊戲統計與圖表視覺化-ASP"

REM Git 上傳流程
git add --all
git commit -m "Auto update on %date% %time%"
git push