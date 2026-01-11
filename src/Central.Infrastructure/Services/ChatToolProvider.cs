using Central.Domain.Documents;
using Central.Domain.Ports;

using OpenAI.Chat;

namespace Central.Infrastructure.Services;

/// <summary>
/// Provider for building ChatTool definitions for OpenAI API.
/// </summary>
public sealed class ChatToolProvider : IChatToolProvider
{
    public List<ChatTool> BuildChatTools(IEnumerable<DocumentTool> enabledTools)
    {
        var tools = new List<ChatTool>();
        var enabledToolsSet = new HashSet<DocumentTool>(enabledTools);

        if (enabledToolsSet.Contains(DocumentTool.SetTitle))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_document_title",
                functionDescription: "Sets the title of the current document. Use this when you have analyzed the document and determined an appropriate title based on its content.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "title": {
                            "type": "string",
                            "description": "The new title for the document"
                        }
                    },
                    "required": ["title"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.SetDate))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_document_date",
                functionDescription: "Sets the document date for the current document. This is the date associated with the document content, not the creation date. Use ISO 8601 format.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "documentDate": {
                            "type": "string",
                            "description": "The document date in ISO 8601 format (e.g., 2024-01-15T10:30:00Z or 2024-01-15)"
                        }
                    },
                    "required": ["documentDate"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.SetContract))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_contract",
                functionDescription: "Sets the contract for the current document by contract ID.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "contractId": {
                            "type": "number",
                            "description": "The ID of the contract to assign"
                        }
                    },
                    "required": ["contractId"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.SetCorrespondent))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_correspondent",
                functionDescription: "Sets the correspondent for the current document by correspondent ID.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "correspondentId": {
                            "type": "number",
                            "description": "The ID of the correspondent to assign"
                        }
                    },
                    "required": ["correspondentId"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.SetDocumentType))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_document_type",
                functionDescription: "Sets the document type for the current document by document type ID.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "documentTypeId": {
                            "type": "number",
                            "description": "The ID of the document type to assign"
                        }
                    },
                    "required": ["documentTypeId"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.SetTags))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_tags",
                functionDescription: "Sets the tags for the current document by tag IDs. Replaces all existing tags.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "tagIds": {
                            "type": "array",
                            "items": {
                                "type": "number"
                            },
                            "description": "Array of tag IDs to assign to the document"
                        }
                    },
                    "required": ["tagIds"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.SetContent))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "set_content",
                functionDescription: "Sets or updates the textual content of the current document.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "content": {
                            "type": "string",
                            "description": "The text content to set"
                        }
                    },
                    "required": ["content"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetContent))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_document_content",
                functionDescription: "Get the content of the current document to analyze its text and structure."
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetDocument))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_document",
                functionDescription: "Get detailed information about a specific document by its ID.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "documentId": {
                            "type": "number",
                            "description": "The ID of the document to retrieve"
                        }
                    },
                    "required": ["documentId"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetSimilar))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_similar_documents",
                functionDescription: "Retrieves a list of existing documents with their titles to help create consistent naming. You can filter by document type or correspondent.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "documentTypeId": {
                            "type": "number",
                            "description": "Optional: Filter by document type ID"
                        },
                        "correspondentId": {
                            "type": "number",
                            "description": "Optional: Filter by correspondent ID"
                        },
                        "limit": {
                            "type": "number",
                            "description": "Maximum number of documents to return (default: 10, max: 50)"
                        }
                    }
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetContracts))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_contracts",
                functionDescription: "Retrieves all available contracts with their IDs and names."
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetDocumentTypes))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_document_types",
                functionDescription: "Retrieves all available document types with their IDs and names."
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetCorrespondents))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_correspondents",
                functionDescription: "Retrieves all available correspondents with their IDs and names."
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.GetTags))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "get_tags",
                functionDescription: "Retrieves all available tags with their IDs and names."
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.CreateContract))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "create_contract",
                functionDescription: "Creates a new contract with the specified name and optional description.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "name": {
                            "type": "string",
                            "description": "The name of the contract"
                        },
                        "description": {
                            "type": "string",
                            "description": "Optional description of the contract"
                        }
                    },
                    "required": ["name"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.CreateCorrespondent))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "create_correspondent",
                functionDescription: "Creates a new correspondent with the specified name and optional description.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "name": {
                            "type": "string",
                            "description": "The name of the correspondent"
                        },
                        "description": {
                            "type": "string",
                            "description": "Optional description of the correspondent"
                        }
                    },
                    "required": ["name"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.CreateDocumentType))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "create_document_type",
                functionDescription: "Creates a new document type with the specified name and optional description.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "name": {
                            "type": "string",
                            "description": "The name of the document type"
                        },
                        "description": {
                            "type": "string",
                            "description": "Optional description of the document type"
                        }
                    },
                    "required": ["name"]
                }
                """u8.ToArray())
            ));
        }

        if (enabledToolsSet.Contains(DocumentTool.CreateTag))
        {
            tools.Add(ChatTool.CreateFunctionTool(
                functionName: "create_tag",
                functionDescription: "Creates a new tag with the specified name and optional description.",
                functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "name": {
                            "type": "string",
                            "description": "The name of the tag"
                        },
                        "description": {
                            "type": "string",
                            "description": "Optional description of the tag"
                        }
                    },
                    "required": ["name"]
                }
                """u8.ToArray())
            ));
        }

        return tools;
    }
}
