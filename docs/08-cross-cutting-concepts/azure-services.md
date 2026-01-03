# Azure Services Setup Guide

This guide explains how to configure Azure OpenAI and Azure Document Intelligence services for use with the document processing system.

## Overview

The processing system uses two Azure Cognitive Services:

1. **Azure OpenAI** - For intelligent text analysis, summarization, and metadata enrichment
2. **Azure Document Intelligence** (formerly Form Recognizer) - For extracting text, tables, and structure from documents

## Azure OpenAI Setup

### 1. Create Azure OpenAI Resource

```bash
# Using Azure CLI
az cognitiveservices account create \
  --name <your-openai-resource-name> \
  --resource-group <your-resource-group> \
  --kind OpenAI \
  --sku S0 \
  --location eastus
```

Or use the [Azure Portal](https://portal.azure.com):
1. Navigate to **Create a resource** > **AI + Machine Learning** > **Azure OpenAI**
2. Fill in the required fields (subscription, resource group, region, name)
3. Click **Review + create**

### 2. Deploy a Model

After creating the resource, deploy a GPT model:

```bash
# Using Azure CLI
az cognitiveservices account deployment create \
  --name <your-openai-resource-name> \
  --resource-group <your-resource-group> \
  --deployment-name gpt-4 \
  --model-name gpt-4 \
  --model-version "0613" \
  --model-format OpenAI \
  --sku-capacity 1 \
  --sku-name Standard
```

Or use Azure OpenAI Studio:
1. Navigate to your Azure OpenAI resource in the portal
2. Click **Go to Azure OpenAI Studio**
3. Select **Deployments** > **Create new deployment**
4. Choose a model (e.g., `gpt-4`, `gpt-35-turbo`)
5. Name your deployment (e.g., `gpt-4`)

### 3. Get Credentials

Retrieve your endpoint and API key:

```bash
# Get endpoint
az cognitiveservices account show \
  --name <your-openai-resource-name> \
  --resource-group <your-resource-group> \
  --query properties.endpoint

# Get API key
az cognitiveservices account keys list \
  --name <your-openai-resource-name> \
  --resource-group <your-resource-group> \
  --query key1
```

Or from the Azure Portal:
1. Navigate to your Azure OpenAI resource
2. Click **Keys and Endpoint** in the left menu
3. Copy **Endpoint** and **Key 1**

### 4. Configure in Process Definition

When creating a process step with type `AzureOpenAI`, use this configuration:

```json
{
  "Endpoint": "https://<your-resource-name>.openai.azure.com",
  "ApiKey": "<your-api-key>",
  "DeploymentName": "gpt-4",
  "Prompt": "Analyze this document and extract key information",
  "SystemPrompt": "You are a helpful AI assistant that extracts structured information from documents."
}
```

**Configuration Properties:**
- `Endpoint` - Your Azure OpenAI endpoint URL
- `ApiKey` - Your Azure OpenAI API key
- `DeploymentName` - Name of your deployed model (e.g., `gpt-4`)
- `Prompt` - The user prompt sent to the model (can reference `{documentContent}` placeholder)
- `SystemPrompt` - (Optional) System message to set context for the AI

## Azure Document Intelligence Setup

### 1. Create Document Intelligence Resource

```bash
# Using Azure CLI
az cognitiveservices account create \
  --name <your-doc-intel-resource-name> \
  --resource-group <your-resource-group> \
  --kind FormRecognizer \
  --sku S0 \
  --location eastus
```

Or use the [Azure Portal](https://portal.azure.com):
1. Navigate to **Create a resource** > **AI + Machine Learning** > **Document Intelligence**
2. Fill in the required fields
3. Click **Review + create**

### 2. Get Credentials

```bash
# Get endpoint
az cognitiveservices account show \
  --name <your-doc-intel-resource-name> \
  --resource-group <your-resource-group> \
  --query properties.endpoint

# Get API key
az cognitiveservices account keys list \
  --name <your-doc-intel-resource-name> \
  --resource-group <your-resource-group> \
  --query key1
```

Or from the Azure Portal:
1. Navigate to your Document Intelligence resource
2. Click **Keys and Endpoint**
3. Copy **Endpoint** and **Key 1**

### 3. Configure in Process Definition

When creating a process step with type `AzureDocumentIntelligence`, use this configuration:

```json
{
  "Endpoint": "https://<your-resource-name>.cognitiveservices.azure.com",
  "ApiKey": "<your-api-key>"
}
```

**Configuration Properties:**
- `Endpoint` - Your Azure Document Intelligence endpoint URL
- `ApiKey` - Your Azure Document Intelligence API key

## Security Best Practices

### 1. Use Azure Key Vault

Instead of hardcoding API keys in process definitions, store them in Azure Key Vault:

```csharp
// In appsettings.json, reference Key Vault secrets
{
  "Azure": {
    "OpenAI": {
      "Endpoint": "https://<resource>.openai.azure.com",
      "ApiKey": "@Microsoft.KeyVault(SecretUri=https://<vault>.vault.azure.net/secrets/openai-key/)"
    },
    "DocumentIntelligence": {
      "Endpoint": "https://<resource>.cognitiveservices.azure.com",
      "ApiKey": "@Microsoft.KeyVault(SecretUri=https://<vault>.vault.azure.net/secrets/doc-intel-key/)"
    }
  }
}
```

### 2. Use Managed Identity

For production deployments, use Managed Identity to avoid managing credentials:

```bash
# Assign Managed Identity to your app
az webapp identity assign \
  --name <your-app-name> \
  --resource-group <your-resource-group>

# Grant access to Azure OpenAI
az role assignment create \
  --assignee <managed-identity-principal-id> \
  --role "Cognitive Services OpenAI User" \
  --scope /subscriptions/<subscription-id>/resourceGroups/<resource-group>/providers/Microsoft.CognitiveServices/accounts/<openai-resource-name>
```

Then update the code to use `DefaultAzureCredential`:

```csharp
var credential = new DefaultAzureCredential();
var client = new AzureOpenAIClient(new Uri(endpoint), credential);
```

### 3. Rotate Keys Regularly

```bash
# Regenerate key
az cognitiveservices account keys regenerate \
  --name <your-resource-name> \
  --resource-group <your-resource-group> \
  --key-name key1
```

## Cost Management

### Azure OpenAI Pricing

- GPT-4: ~$0.03-$0.06 per 1K tokens (varies by region and version)
- GPT-3.5-Turbo: ~$0.0005-$0.002 per 1K tokens

**Tips:**
- Use `gpt-35-turbo` for simple tasks to reduce costs
- Set token limits in your prompts
- Monitor usage in Azure Portal > Cost Management

### Document Intelligence Pricing

- **Free Tier**: 500 pages/month
- **Standard**: $1.50 per 1,000 pages (prebuilt models)

**Tips:**
- Use batch processing to optimize API calls
- Cache results when possible
- Monitor page count usage

## Monitoring and Diagnostics

### Enable Application Insights

```bash
az cognitiveservices account update \
  --name <your-resource-name> \
  --resource-group <your-resource-group> \
  --set properties.customSubDomainName=<custom-name>
```

### View Metrics

In Azure Portal:
1. Navigate to your resource
2. Click **Metrics** in the left menu
3. Add metrics:
   - Total Calls
   - Data In/Out
   - Latency
   - Errors

### Set Up Alerts

```bash
az monitor metrics alert create \
  --name "High Error Rate" \
  --resource-group <your-resource-group> \
  --scopes /subscriptions/<subscription-id>/resourceGroups/<resource-group>/providers/Microsoft.CognitiveServices/accounts/<resource-name> \
  --condition "Total Errors > 10" \
  --window-size 5m \
  --evaluation-frequency 1m
```

## Testing Configuration

### Test Azure OpenAI

```bash
curl -X POST "https://<resource>.openai.azure.com/openai/deployments/gpt-4/chat/completions?api-version=2024-02-15-preview" \
  -H "api-key: <your-key>" \
  -H "Content-Type: application/json" \
  -d '{
    "messages": [
      {"role": "system", "content": "You are a helpful assistant."},
      {"role": "user", "content": "Hello!"}
    ]
  }'
```

### Test Document Intelligence

```bash
curl -X POST "https://<resource>.cognitiveservices.azure.com/formrecognizer/documentModels/prebuilt-layout:analyze?api-version=2023-07-31" \
  -H "Ocp-Apim-Subscription-Key: <your-key>" \
  -H "Content-Type: application/json" \
  -d '{
    "urlSource": "https://example.com/sample.pdf"
  }'
```

## Troubleshooting

### Common Errors

**401 Unauthorized**
- Verify API key is correct
- Check that the key hasn't expired
- Ensure endpoint URL is correct

**429 Too Many Requests**
- You've hit rate limits
- Implement retry logic with exponential backoff
- Consider upgrading to higher quota

**Invalid Deployment Name**
- Verify deployment name matches exactly
- Check that the model deployment is active in Azure OpenAI Studio

**Document Intelligence Timeout**
- Large documents may take longer to process
- Use async analysis for documents > 10MB
- Implement proper timeout handling in code

## References

- [Azure OpenAI Documentation](https://learn.microsoft.com/en-us/azure/ai-services/openai/)
- [Azure Document Intelligence Documentation](https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/)
- [Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net)
