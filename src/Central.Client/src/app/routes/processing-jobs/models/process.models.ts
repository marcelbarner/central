export enum DocumentState {
  Imported = 'Imported',
  Processing = 'Processing',
  Processed = 'Processed',
  Failed = 'Failed'
}

export enum StepType {
  AzureOpenAI = 'AzureOpenAI',
  AzureDocumentIntelligence = 'AzureDocumentIntelligence'
}

export enum ExecutionStatus {
  Pending = 'Pending',
  Running = 'Running',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

export interface ProcessingStep {
  id?: number;
  name: string;
  description?: string;
  stepType: StepType;
  order: number;
  azureEndpoint?: string;
  azureApiKey?: string;
  azureModelOrDeployment?: string;
  prompt?: string;
  configuration?: string;
}

export interface ProcessDefinition {
  id: number;
  name: string;
  description?: string;
  enabled: boolean;
  triggerState: DocumentState;
  created: Date;
  updated: Date;
  steps: ProcessingStep[];
}

export interface CreateProcessDefinitionRequest {
  name: string;
  description?: string;
  enabled: boolean;
  triggerState: DocumentState;
  steps: ProcessingStep[];
}

export interface UpdateProcessDefinitionRequest {
  name: string;
  description?: string;
  enabled: boolean;
  triggerState: DocumentState;
  steps: ProcessingStep[];
}

export interface ProcessExecutionStep {
  id: number;
  stepName: string;
  stepType: StepType;
  order: number;
  status: ExecutionStatus;
  startedAt?: Date;
  completedAt?: Date;
  errorMessage?: string;
  output?: string;
}

export interface ProcessExecution {
  id: number;
  processDefinitionId: number;
  documentId: number;
  status: ExecutionStatus;
  startedAt?: Date;
  completedAt?: Date;
  errorMessage?: string;
  steps: ProcessExecutionStep[];
}

export interface CreateProcessExecutionRequest {
  processDefinitionId: number;
  documentId: number;
}
