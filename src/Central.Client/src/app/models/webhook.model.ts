export interface Webhook {
  id: number;
  eventType: string;
  url: string;
  created: string;
  updated: string;
}

export interface CreateWebhookRequest {
  eventType: string;
  url: string;
}

export interface UpdateWebhookRequest {
  id: number;
  eventType: string;
  url: string;
}

export const WebhookEventTypes = [
  { value: 'DocumentAdded', label: 'Document Added' },
  { value: 'DocumentUpdated', label: 'Document Updated' },
  { value: 'DocumentDeleted', label: 'Document Deleted' }
];
