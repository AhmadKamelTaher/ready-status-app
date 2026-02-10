# Ready Status App - Google Cloud Deployment Guide

## 🚀 Quick Deployment Steps

### Prerequisites
1. **Google Cloud Account** - Create at https://cloud.google.com (free $300 credit for new users)
2. **Google Cloud CLI** - Download from https://cloud.google.com/sdk/docs/install
3. **Docker Desktop** (optional, for local testing)

---

## Step 1: Set Up Google Cloud

### 1.1 Install Google Cloud CLI
Download and install from: https://cloud.google.com/sdk/docs/install

### 1.2 Login and Create Project
```bash
# Login to Google Cloud
gcloud auth login

# Create a new project (replace YOUR_PROJECT_ID with a unique name)
gcloud projects create readystatus-app --name="Ready Status App"

# Set the project as active
gcloud config set project readystatus-app

# Enable billing (required for Cloud Run)
# Go to: https://console.cloud.google.com/billing
```

### 1.3 Enable Required APIs
```bash
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com
gcloud services enable sqladmin.googleapis.com
```

---

## Step 2: Set Up Database (Cloud SQL PostgreSQL)

### 2.1 Create PostgreSQL Instance
```bash
# Create a Cloud SQL PostgreSQL instance (this may take a few minutes)
gcloud sql instances create readystatus-db ^
    --database-version=POSTGRES_15 ^
    --tier=db-f1-micro ^
    --region=us-central1 ^
    --root-password=YourSecurePassword123!

# Create the database
gcloud sql databases create readystatusdb --instance=readystatus-db

# Create a user
gcloud sql users create appuser ^
    --instance=readystatus-db ^
    --password=YourAppPassword123!
```

### 2.2 Get Connection Details
```bash
# Get the connection name
gcloud sql instances describe readystatus-db --format="value(connectionName)"
```

---

## Step 3: Deploy to Google Cloud Run

### 3.1 Build and Deploy
```bash
# Navigate to your project folder
cd c:\Users\hp\Videos\alisheri\ReadyStatusApp

# Build and deploy to Cloud Run
gcloud run deploy readystatus-app ^
    --source . ^
    --region=us-central1 ^
    --allow-unauthenticated ^
    --add-cloudsql-instances=YOUR_PROJECT_ID:us-central1:readystatus-db ^
    --set-env-vars="DATABASE_URL=Host=/cloudsql/YOUR_PROJECT_ID:us-central1:readystatus-db;Database=readystatusdb;Username=appuser;Password=YourAppPassword123!"
```

### 3.2 Get Your App URL
After deployment, you'll receive a URL like:
`https://readystatus-app-xxxxx-uc.a.run.app`

---

## 🆓 FREE Alternative: Using Supabase (Easier!)

If you want a completely FREE option with less setup:

### Step 1: Create Supabase Account
1. Go to https://supabase.com
2. Sign up for free
3. Create a new project

### Step 2: Get Connection String
1. Go to Project Settings → Database
2. Copy the "Connection string" (URI format)
3. It looks like: `postgresql://postgres:[PASSWORD]@db.xxxxx.supabase.co:5432/postgres`

### Step 3: Deploy to Google Cloud Run
```bash
cd c:\Users\hp\Videos\alisheri\ReadyStatusApp

gcloud run deploy readystatus-app ^
    --source . ^
    --region=us-central1 ^
    --allow-unauthenticated ^
    --set-env-vars="DATABASE_URL=YOUR_SUPABASE_CONNECTION_STRING"
```

---

## 🆓 EASIEST Option: Railway.app (One-Click Deploy)

Railway offers the simplest deployment with free tier:

1. Go to https://railway.app
2. Sign up with GitHub
3. Click "New Project" → "Deploy from GitHub"
4. Connect your repository
5. Railway auto-detects .NET and sets up everything!
6. Add a PostgreSQL database from the dashboard
7. Your app is live!

---

## Default Login Credentials
- **Admin**: admin@readystatus.com / Admin123
- **User**: Register a new account

---

## Troubleshooting

### If deployment fails:
1. Check logs: `gcloud run services logs read readystatus-app`
2. Verify database connection string
3. Ensure all APIs are enabled

### Database connection issues:
- Make sure the Cloud SQL instance is running
- Verify the connection string format
- Check that the Cloud SQL Admin API is enabled

---

## Cost Estimate (Google Cloud)
- **Cloud Run**: Free tier includes 2 million requests/month
- **Cloud SQL**: ~$7-10/month for smallest instance
- **Total**: Can be FREE with Supabase or ~$10/month with Cloud SQL

---

## Need Help?
- Google Cloud Run Docs: https://cloud.google.com/run/docs
- Supabase Docs: https://supabase.com/docs
- Railway Docs: https://docs.railway.app
