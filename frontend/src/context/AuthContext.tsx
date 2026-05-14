"use client";

import axios from "axios";
import { createContext, useContext, useEffect, useState } from "react";
import { AuthService } from "@/services/auth.service";
import { queryError } from "@/helpers/errorDisplay";
import { UserProfile } from "@/types/auth";

interface AuthContextType {
  user: UserProfile | null;
  authChecked: boolean;
  storeUser: (userData: UserProfile | null) => void;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<UserProfile | null>(null);
  const [authChecked, setAuthChecked] = useState(false);

  const storeUser = (userData: UserProfile | null) => {
    setUser(userData);
  };

  const logout = async () => {
    try {
      await AuthService.logout();
    } catch (error) {
      console.error("Logout failed on the server, clearing local state anyway", error);
    } finally {
      setUser(null);
      if (typeof window !== "undefined") {
        window.location.replace("/authentication/login");
      }
    }
  };

  useEffect(() => {
    const getCurrentUser = async () => {
      try {
        const me = await AuthService.me();

        // Store user data
        setUser({
          userId: me.userId,
          email: me.email,
          displayName: me.displayName,
          displayImage: me.displayImage
        });
      } catch (error: any) {
        // 401 on /me is expected when no session exists.
        if (axios.isAxiosError(error) && error.response?.status === 401) {
          setUser(null);
        } else {
          queryError(error);
          setUser(null);
        }
      } finally {
        setAuthChecked(true);
      }
    };

    void getCurrentUser();
  }, []);

  return (
    <AuthContext.Provider value={{ user, authChecked, storeUser, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
};
