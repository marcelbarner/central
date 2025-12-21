#!/usr/bin/env bash
set -e

# Default values
REGISTRY="${REGISTRY:-10.1.1.18:5000}"
IMAGE_NAME="${IMAGE_NAME:-central}"
TAG="${TAG:-latest}"
SKIP_BUILD="${SKIP_BUILD:-false}"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Helper functions
print_step() {
    echo -e "${CYAN}>>> $1${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -r|--registry)
            REGISTRY="$2"
            shift 2
            ;;
        -i|--image)
            IMAGE_NAME="$2"
            shift 2
            ;;
        -t|--tag)
            TAG="$2"
            shift 2
            ;;
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  -r, --registry REGISTRY   Registry URL (default: 10.1.1.18:5000)"
            echo "  -i, --image IMAGE_NAME    Image name (default: central)"
            echo "  -t, --tag TAG             Image tag (default: latest)"
            echo "  --skip-build              Skip building, only tag and push"
            echo "  -h, --help                Show this help message"
            echo ""
            echo "Examples:"
            echo "  $0"
            echo "  $0 -t v1.0.0"
            echo "  $0 -r myregistry.local:5000 -i my-app"
            echo "  $0 --skip-build"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use -h or --help for usage information"
            exit 1
            ;;
    esac
done

# Full image names
LOCAL_IMAGE="central-new-central-app:latest"
REGISTRY_IMAGE="${REGISTRY}/${IMAGE_NAME}:${TAG}"
REGISTRY_IMAGE_LATEST="${REGISTRY}/${IMAGE_NAME}:latest"

echo ""
echo -e "${YELLOW}======================================${NC}"
echo -e "${YELLOW}  Docker Registry Publish Script${NC}"
echo -e "${YELLOW}======================================${NC}"
echo ""
echo -e "Registry:       ${REGISTRY}"
echo -e "Image Name:     ${IMAGE_NAME}"
echo -e "Tag:            ${TAG}"
echo -e "Full Name:      ${REGISTRY_IMAGE}"
echo ""

# Step 1: Build the image (unless skipped)
if [ "$SKIP_BUILD" = false ]; then
    print_step "Building Docker image..."
    if ! docker compose build central-app; then
        print_error "Docker build failed"
        exit 1
    fi
    print_success "Docker image built successfully"
    echo ""
else
    echo -e "${YELLOW}Skipping build step...${NC}"
    echo ""
fi

# Step 2: Tag the image with registry name
print_step "Tagging image for registry..."
if ! docker tag "$LOCAL_IMAGE" "$REGISTRY_IMAGE"; then
    print_error "Failed to tag image with: $REGISTRY_IMAGE"
    exit 1
fi
print_success "Image tagged: $REGISTRY_IMAGE"

# Also tag as latest if not already latest
if [ "$TAG" != "latest" ]; then
    if ! docker tag "$LOCAL_IMAGE" "$REGISTRY_IMAGE_LATEST"; then
        print_error "Failed to tag image with: $REGISTRY_IMAGE_LATEST"
        exit 1
    fi
    print_success "Image tagged: $REGISTRY_IMAGE_LATEST"
fi
echo ""

# Step 3: Push to registry
print_step "Pushing image to registry..."
if ! docker push "$REGISTRY_IMAGE"; then
    print_error "Failed to push image to registry"
    exit 1
fi
print_success "Image pushed: $REGISTRY_IMAGE"

# Push latest tag if different
if [ "$TAG" != "latest" ]; then
    if ! docker push "$REGISTRY_IMAGE_LATEST"; then
        print_error "Failed to push latest tag to registry"
        exit 1
    fi
    print_success "Image pushed: $REGISTRY_IMAGE_LATEST"
fi
echo ""

# Success summary
echo -e "${GREEN}======================================${NC}"
echo -e "${GREEN}  ✓ Successfully Published!${NC}"
echo -e "${GREEN}======================================${NC}"
echo ""
echo "Image available at:"
echo -e "  ${CYAN}- $REGISTRY_IMAGE${NC}"
if [ "$TAG" != "latest" ]; then
    echo -e "  ${CYAN}- $REGISTRY_IMAGE_LATEST${NC}"
fi
echo ""
echo "To pull the image:"
echo -e "  ${YELLOW}docker pull $REGISTRY_IMAGE${NC}"
echo ""
