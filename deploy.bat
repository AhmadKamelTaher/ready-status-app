@echo off
echo ==========================================
echo   Ready Status App - Google Cloud Deploy
echo ==========================================
echo.

REM Check if gcloud is installed
where gcloud >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Google Cloud CLI is not installed!
    echo.
    echo Please download and install from:
    echo https://cloud.google.com/sdk/docs/install
    echo.
    pause
    exit /b 1
)

echo Step 1: Logging into Google Cloud...
gcloud auth login

echo.
set /p PROJECT_ID="Enter your Google Cloud Project ID: "

echo.
echo Step 2: Setting project to %PROJECT_ID%...
gcloud config set project %PROJECT_ID%

echo.
echo Step 3: Enabling required APIs...
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com

echo.
set /p DATABASE_URL="Enter your PostgreSQL connection string (from Supabase or Cloud SQL): "

echo.
echo Step 4: Building and deploying to Cloud Run...
gcloud run deploy readystatus-app ^
    --source . ^
    --region=us-central1 ^
    --allow-unauthenticated ^
    --set-env-vars="DATABASE_URL=%DATABASE_URL%"

echo.
echo ==========================================
echo   Deployment Complete!
echo ==========================================
echo.
echo Your app should now be live!
echo Run 'gcloud run services describe readystatus-app --region=us-central1' to get the URL
echo.
pause
