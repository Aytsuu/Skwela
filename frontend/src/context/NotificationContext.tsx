"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { toast } from "sonner";
import { useAuth } from "./AuthContext";
import { ReleaseService } from "@/services/release.service";
import { ReleaseItem } from "@/types/release";
import { usePathname, useSearchParams } from "next/navigation";

interface ReleasePublishedPayload {
  releaseId: string;
  version?: string | null;
  title: string;
  summary: string;
  body: string;
  publishedAt: string;
}

interface NotificationProps {
  notifications: ReleaseItem[];
  unreadCount: number;
  isLoading: boolean;
  isApplyingBulkAction: boolean;
  markReleaseAsRead: (releaseId: string) => Promise<void>;
  markAllAsRead: () => Promise<void>;
  dismissNotification: (releaseId: string) => Promise<void>;
  clearAll: () => Promise<void>;
  refreshNotifications: () => Promise<void>;
}

export const NotificationContext = createContext<NotificationProps | null>(null);

export const NotificationProvider = ({ children } : { children: React.ReactNode }) => {
  const { user, authChecked } = useAuth();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const [notifications, setNotifications] = useState<ReleaseItem[]>([]);
  const [unreadCount, setUnreadCount] = useState(() => {
    if (typeof window === "undefined") {
      return 0;
    }

    const cachedUnreadCount = window.sessionStorage.getItem("esecai:unread-notification-count");
    return cachedUnreadCount ? Number(cachedUnreadCount) || 0 : 0;
  });
  const [isLoading, setIsLoading] = useState(false);
  const [isApplyingBulkAction, setIsApplyingBulkAction] = useState(false);
  const defaultTitleRef = React.useRef("esecai - AI Activity Checker");

  const stripNotificationPrefix = React.useCallback((title: string) => {
    return title.replace(/^\(\d+\)\s+/, "").trim();
  }, []);

  const getBaseTitle = React.useCallback((currentPathname: string) => {
    if (currentPathname.startsWith("/dashboard")) {
      return "Dashboard - esecai";
    }

    if (currentPathname.startsWith("/classrooms")) {
      return "Classrooms - esecai";
    }

    if (currentPathname.startsWith("/settings")) {
      return "Settings - esecai";
    }

    return "esecai - AI Activity Checker";
  }, []);

  const refreshNotifications = React.useCallback(async () => {
    if (!authChecked) {
      return;
    }

    if (!user) {
      setNotifications([]);
      setUnreadCount(0);
      return;
    }

    setIsLoading(true);

    try {
      const [releases, count] = await Promise.all([
        ReleaseService.getReleases(),
        ReleaseService.getUnreadCount()
      ]);

      setNotifications(releases);
      setUnreadCount(count);
    } finally {
      setIsLoading(false);
    }
  }, [user]);

  useEffect(() => {
    if (typeof window === "undefined") {
      return;
    }

    window.sessionStorage.setItem("esecai:unread-notification-count", unreadCount.toString());
  }, [unreadCount]);

  const markReleaseAsRead = React.useCallback(async (releaseId: string) => {
    const target = notifications.find((notification) => notification.releaseId === releaseId);
    if (!target || target.isRead) {
      return;
    }

    try {
      await ReleaseService.markAsRead(releaseId);

      setNotifications((current) =>
        current.map((notification) =>
          notification.releaseId === releaseId
            ? { ...notification, isRead: true }
            : notification
        ));
      setUnreadCount((current) => Math.max(0, current - 1));
    } catch (error) {
      toast.error("Unable to mark notification as read.");
      await refreshNotifications();
      throw error;
    }
  }, [notifications, refreshNotifications]);

  const markAllAsRead = React.useCallback(async () => {
    if (unreadCount === 0 || isApplyingBulkAction) return;

    setIsApplyingBulkAction(true);

    try {
      await ReleaseService.markAllAsRead();
      setNotifications((current) => current.map((n) => ({ ...n, isRead: true })));
      setUnreadCount(0);
    } catch (error) {
      toast.error("Unable to mark all notifications as read.");
      await refreshNotifications();
      throw error;
    } finally {
      setIsApplyingBulkAction(false);
    }
  }, [isApplyingBulkAction, refreshNotifications, unreadCount]);

  const dismissNotification = React.useCallback(async (releaseId: string) => {
    const target = notifications.find((n) => n.releaseId === releaseId);
    if (!target) return;

    try {
      await ReleaseService.dismissRelease(releaseId);
      setNotifications((current) => current.filter((n) => n.releaseId !== releaseId));
      if (!target.isRead) {
        setUnreadCount((current) => Math.max(0, current - 1));
      }
    } catch (error) {
      toast.error("Unable to clear notification.");
      await refreshNotifications();
      throw error;
    }
  }, [notifications, refreshNotifications]);

  const clearAll = React.useCallback(async () => {
    if (notifications.length === 0 || isApplyingBulkAction) return;

    setIsApplyingBulkAction(true);

    try {
      await ReleaseService.clearAll();
      setNotifications([]);
      setUnreadCount(0);
    } catch (error) {
      toast.error("Unable to clear notifications.");
      await refreshNotifications();
      throw error;
    } finally {
      setIsApplyingBulkAction(false);
    }
  }, [isApplyingBulkAction, notifications.length, refreshNotifications]);

  useEffect(() => {
    void refreshNotifications();
  }, [refreshNotifications]);

  useEffect(() => {
    if (typeof document === "undefined") {
      return;
    }

    const currentTitle = stripNotificationPrefix(document.title);
    const fallbackTitle = getBaseTitle(pathname);
    defaultTitleRef.current = currentTitle === "esecai - AI Activity Checker"
      ? (defaultTitleRef.current !== "esecai - AI Activity Checker" ? defaultTitleRef.current : fallbackTitle)
      : currentTitle;

    document.title = unreadCount > 0
      ? `(${unreadCount}) ${defaultTitleRef.current}`
      : defaultTitleRef.current;

    return () => {
      document.title = defaultTitleRef.current;
    };
  }, [getBaseTitle, pathname, searchParams, stripNotificationPrefix, unreadCount]);

  useEffect(() => {
    if (!user || typeof window === "undefined") {
      return;
    }

    const baseURL = process.env.NODE_ENV === "development"
      ? "http://localhost:8080"
      : process.env.NEXT_PUBLIC_API_URL;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${baseURL}/hubs/notifications`, {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build();

    connection.on("ReleasePublished", (payload: ReleasePublishedPayload) => {
      let shouldIncrementUnread = false;
      const nextRelease: ReleaseItem = {
        releaseId: payload.releaseId,
        version: payload.version ?? null,
        title: payload.title,
        summary: payload.summary,
        body: payload.body,
        status: "published",
        publishedAt: payload.publishedAt,
        createdAt: payload.publishedAt,
        isRead: false
      };

      setNotifications((current) => {
        const existingIndex = current.findIndex((notification) => notification.releaseId === payload.releaseId);
        if (existingIndex >= 0) {
          const updated = [...current];
          updated[existingIndex] = {
            ...updated[existingIndex],
            ...nextRelease,
            isRead: updated[existingIndex].isRead
          };
          return updated;
        }

        shouldIncrementUnread = true;
        return [nextRelease, ...current];
      });
      if (shouldIncrementUnread) {
        setUnreadCount((current) => current + 1);
      }
      toast.success(payload.title, { description: payload.summary });
    });

    connection.start().catch((error: unknown) => {
      console.error("Notification connection failed", error);
    });

    return () => {
      connection.off("ReleasePublished");
      void connection.stop();
    };
  }, [authChecked, user]);

  useEffect(() => {
    if (!user || typeof window === "undefined" || typeof document === "undefined") {
      return;
    }

    const handleWindowFocus = () => {
      void refreshNotifications();
    };

    const handleVisibilityChange = () => {
      if (document.visibilityState === "visible") {
        void refreshNotifications();
      }
    };

    window.addEventListener("focus", handleWindowFocus);
    document.addEventListener("visibilitychange", handleVisibilityChange);

    return () => {
      window.removeEventListener("focus", handleWindowFocus);
      document.removeEventListener("visibilitychange", handleVisibilityChange);
    };
  }, [authChecked, refreshNotifications, user]);

  return (
    <NotificationContext.Provider value={{ 
      notifications, 
      unreadCount, 
      isLoading, 
      isApplyingBulkAction,
      markReleaseAsRead, 
      markAllAsRead, 
      dismissNotification, 
      clearAll, 
      refreshNotifications 
    }}>
      {children}
    </NotificationContext.Provider>
  );
};

export const useNotification = () => {
  const context = useContext(NotificationContext);

  if (!context) {
    throw new Error("useNotification must be used within a NotificationProvider");
  }

  return context;
};
