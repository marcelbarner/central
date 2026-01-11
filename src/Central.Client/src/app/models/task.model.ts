export interface Task {
  id: number;
  name: string;
  description?: string;
  taskType: 'AzureOpenAI' | 'AzureDocumentIntelligence';
  enabled: boolean;
  configuration: TaskConfiguration;
  created: string;
  updated: string;
}

export interface TaskConfiguration {
  azureEndpoint?: string;
  azureApiKey?: string;
  azureModelOrDeployment?: string;
  prompt?: string;
  temperature?: number;
  maxTokens?: number;
  allowedTools?: string[];
  capabilities?: string;
  documentIntelligenceOptions?: string;
}

export interface CreateTaskRequest {
  name: string;
  description?: string;
  taskType: 'AzureOpenAI' | 'AzureDocumentIntelligence';
  enabled: boolean;
  configuration: TaskConfiguration;
}

export interface UpdateTaskRequest {
  name: string;
  description?: string;
  taskType: 'AzureOpenAI' | 'AzureDocumentIntelligence';
  enabled: boolean;
  configuration: TaskConfiguration;
}

export interface TaskExecution {
  id: number;
  taskId: number;
  documentId: number;
  pipelineExecutionId?: number;
  status: 'Pending' | 'Running' | 'Completed' | 'Failed';
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  result?: string;
}

export interface ExecuteTaskRequest {
  documentId: number;
}
