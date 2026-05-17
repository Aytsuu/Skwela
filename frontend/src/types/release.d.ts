export interface ReleaseItem {
  releaseId: string;
  version?: string | null;
  title: string;
  summary: string;
  body: string;
  status: "draft" | "published";
  publishedAt?: string | null;
  createdAt: string;
  isRead: boolean;
}

export interface ReleaseEditorInput {
  version?: string | null;
  title: string;
  summary: string;
  body: string;
}
