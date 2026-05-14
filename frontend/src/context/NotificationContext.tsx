"use client";

import React, { createContext, useEffect, useState, useContext } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from "sonner";
import { useAuth } from './AuthContext';

interface NotificationProps {
  notifications: unknown[]
  unreadCount: number
}

export const NotificationContext = createContext<NotificationProps | null>(null);

export const NotificationProvider = ({ children } : { children: React.ReactNode }) => {
    const { user } = useAuth();

    const [connection, _setConnection] = useState<signalR.HubConnection | null>(() => {
        if (typeof window === "undefined") return null;
        const baseURL = process.env.NODE_ENV === "development" ? "http://localhost:8080" : process.env.NEXT_PUBLIC_API_URL;
        return new signalR.HubConnectionBuilder()
            .withUrl(`${baseURL}/hubs/notifications`, {
                withCredentials: true
            })
            .withAutomaticReconnect()
            .build();
    });
    const [notifications, setNotifications] = useState<unknown[]>([]);

    useEffect(() => {
        if (connection && user) {
            connection.start()
                .then(() => {
                    connection.on("ReceiveNotification", (notification: unknown) => {
                        const notif = notification as { message?: string, title?: string };
                        // Add to state
                        setNotifications(prev => [notification, ...prev]);
                        // Show visual alert
                        toast.success(notif.message || "", { title: notif.title } as unknown as any); // eslint-disable-line @typescript-eslint/no-explicit-any
                    });
                })
                .catch((e: unknown) => console.log('Connection failed: ', e));
        }

        return () => {
            if (connection) connection.stop();
        };
    }, [connection, user]);

    return (
        <NotificationContext.Provider value={{ notifications, unreadCount: notifications.length }}>
            {children}
        </NotificationContext.Provider>
    );
};

export const useNotification = () => {
  const context = useContext(NotificationContext);

  if (!context) {
    throw new Error("useAuth must be used within an NotificationProvider");
  }

  return context;
};