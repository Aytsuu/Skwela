import { queryError } from "@/helpers/errorDisplay";
import { ReleaseEditorInput, ReleaseItem } from "@/types/release";
import { api } from "./api.service";

export const ReleaseService = {
  getReleases: async () => {
    try {
      const response = await api.get<ReleaseItem[]>("api/releases");
      return response.data;
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  getUnreadCount: async () => {
    try {
      const response = await api.get<{ unreadCount: number }>("api/releases/unread-count");
      return response.data.unreadCount;
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  markAsRead: async (releaseId: string) => {
    try {
      await api.post(`api/releases/${releaseId}/read`);
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  markAllAsRead: async () => {
    try {
      await api.post("api/releases/read-all");
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  dismissRelease: async (releaseId: string) => {
    try {
      await api.post(`api/releases/${releaseId}/dismiss`);
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  clearAll: async () => {
    try {
      await api.post("api/releases/clear-all");
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  getAdminReleases: async () => {
    try {
      const response = await api.get<ReleaseItem[]>("api/admin/releases");
      return response.data;
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  createDraft: async (payload: ReleaseEditorInput) => {
    try {
      const response = await api.post<ReleaseItem>("api/admin/releases", payload);
      return response.data;
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  updateDraft: async (releaseId: string, payload: ReleaseEditorInput) => {
    try {
      const response = await api.patch<ReleaseItem>(`api/admin/releases/${releaseId}`, payload);
      return response.data;
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  },
  publishRelease: async (releaseId: string) => {
    try {
      const response = await api.post<ReleaseItem>(`api/admin/releases/${releaseId}/publish`);
      return response.data;
    } catch (error: unknown) {
      queryError(error);
      throw error;
    }
  }
};
