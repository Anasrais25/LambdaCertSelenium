# Selenium C# (LambdaTest) — Certification Assignment

This repository contains an NUnit Selenium C# test-suite for the LambdaTest Certification assignment. It implements the 3 scenarios required and runs on LambdaTest Selenium Grid in parallel.

## Files
- `src/SeleniumCSharpSample/` — C# project with tests.
- `.gitpod.yml` — single-click Gitpod environment to build & run.
- `env.sample` — shows environment variables required.
- `.gitignore` — ignore build products & secrets.

## Prerequisites
1. LambdaTest account (get `LT_USERNAME` and `LT_ACCESS_KEY` from your LambdaTest profile > Access Keys).
2. GitHub repository (private), shared with `LambdaTest-Certifications` or `admin@lambdatestcertifications.com`.
3. (Optional) Gitpod account to run one-click.



## Set environment variables (Gitpod)
- Go to Gitpod → Variables → Add:
  - `LT_USERNAME` = your username
  - `LT_ACCESS_KEY` = your access key

## Run tests in Gitpod (one-click)
1. Open: `https://gitpod.io/#https://github.com/<your-org>/<repo>`
2. Wait for the workspace to start (build will run).
3. In the terminal run:
   ```bash
   cd src/SeleniumCSharpSample
   dotnet test --logger "trx;LogFileName=test_results.trx"
4. Dotnet is already configured in .gitpod.yml. If it show error then surn below commands:
        # Install Microsoft package signing key and feed
        wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
        sudo dpkg -i packages-microsoft-prod.deb
        
        # Install dependencies and .NET SDK (8.0 recommended)
        sudo apt-get update
        sudo apt-get install -y dotnet-sdk-8.0
