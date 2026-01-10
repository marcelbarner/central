export interface Pipeline {
  id: number;
  name: string;
  description?: string;
  enabled: boolean;
  triggerState?: 'Imported' | 'Processing' | 'Review' | 'Approved' | 'Failed' | 'Processed';
  created: string;
  updated: string;
  steps: PipelineStep[];
}

export interface PipelineStep {
  id: number;
  name: string;
  stepType: 'TaskStep' | 'WaitStep';
  order: number;
  taskId?: number;
  waitDurationSeconds?: number;
}

export interface CreatePipelineRequest {
  name: string;
  description?: string;
  enabled: boolean;
  triggerState?: 'Imported' | 'Processing' | 'Review' | 'Approved' | 'Failed' | 'Processed';
  steps: CreatePipelineStepRequest[];
}

export interface CreatePipelineStepRequest {
  name: string;
  stepType: 'TaskStep' | 'WaitStep';
  order: number;
  taskId?: number;
  waitDurationSeconds?: number;
}

export interface UpdatePipelineRequest {
  name: string;
  description?: string;
  enabled: boolean;
  triggerState?: 'Imported' | 'Processing' | 'Review' | 'Approved' | 'Failed' | 'Processed';
  steps: UpdatePipelineStepRequest[];
}

export interface UpdatePipelineStepRequest {
  name: string;
  stepType: 'TaskStep' | 'WaitStep';
  order: number;
  taskId?: number;
  waitDurationSeconds?: number;
}

export interface PipelineExecution {
  id: number;
  pipelineId: number;
  documentId: number;
  status: 'Pending' | 'Running' | 'Completed' | 'Failed';
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  taskExecutionIds: number[];
}

export interface ExecutePipelineRequest {
  documentId: number;
}
