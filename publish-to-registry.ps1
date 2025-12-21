#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build and publish the Central application to a private Docker registry.

.DESCRIPTION
    This script builds the Docker image for the Central Document Management System
    and publishes it to a private registry at 10.1.1.18:5000.

.PARAMETER Registry
    The registry URL (default: 10.1.1.18:5000)

.PARAMETER ImageName
    The image name (default: central)

.PARAMETER Tag
    The image tag (default: latest)

.PARAMETER SkipBuild
    Skip building the image and only tag/push existing image

.EXAMPLE
    .\publish-to-registry.ps1
    
.EXAMPLE
    .\publish-to-registry.ps1 -Tag v1.0.0
    
.EXAMPLE
    .\publish-to-registry.ps1 -Registry "myregistry.local:5000" -ImageName "my-app"
#>

param(
    [string]$Registry = "10.1.1.18:5000",
    [string]$ImageName = "central",
    [string]$Tag,
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

# Colors for output
function Write-Step {
    param([string]$Message)
    Write-Host ">>> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Error-Message {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

# Full image names
$localImage = "central-new-central-app:latest"
$registryImage = "$Registry/${ImageName}:${Tag}"
$registryImageLatest = "$Registry/${ImageName}:latest"

Write-Host ""
Write-Host "======================================" -ForegroundColor Yellow
Write-Host "  Docker Registry Publish Script" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "Registry:       $Registry" -ForegroundColor White
Write-Host "Image Name:     $ImageName" -ForegroundColor White
Write-Host "Tag:            $Tag" -ForegroundColor White
Write-Host "Full Name:      $registryImage" -ForegroundColor White
Write-Host ""

try {
    # Step 1: Build the image (unless skipped)
    if (-not $SkipBuild) {
        Write-Step "Building Docker image..."
        docker compose build central-app
        if ($LASTEXITCODE -ne 0) {
            throw "Docker build failed"
        }
        Write-Success "Docker image built successfully"
        Write-Host ""
    } else {
        Write-Host "Skipping build step..." -ForegroundColor Yellow
        Write-Host ""
    }

    # Step 2: Tag the image with registry name
    Write-Step "Tagging image for registry..."
    docker tag $localImage $registryImage
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to tag image with: $registryImage"
    }
    Write-Success "Image tagged: $registryImage"

    # Also tag as latest if not already latest
    if ($Tag -ne "latest") {
        docker tag $localImage $registryImageLatest
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to tag image with: $registryImageLatest"
        }
        Write-Success "Image tagged: $registryImageLatest"
    }
    Write-Host ""

    # Step 3: Push to registry
    Write-Step "Pushing image to registry..."
    docker push $registryImage
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push image to registry"
    }
    Write-Success "Image pushed: $registryImage"

    # Push latest tag if different
    if ($Tag -ne "latest") {
        docker push $registryImageLatest
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to push latest tag to registry"
        }
        Write-Success "Image pushed: $registryImageLatest"
    }
    Write-Host ""

    # Success summary
    Write-Host "======================================" -ForegroundColor Green
    Write-Host "  ✓ Successfully Published!" -ForegroundColor Green
    Write-Host "======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Image available at:" -ForegroundColor White
    Write-Host "  - $registryImage" -ForegroundColor Cyan
    if ($Tag -ne "latest") {
        Write-Host "  - $registryImageLatest" -ForegroundColor Cyan
    }
    Write-Host ""
    Write-Host "To pull the image:" -ForegroundColor White
    Write-Host "  docker pull $registryImage" -ForegroundColor Yellow
    Write-Host ""

} catch {
    Write-Host ""
    Write-Error-Message "Error: $_"
    Write-Host ""
    exit 1
}
