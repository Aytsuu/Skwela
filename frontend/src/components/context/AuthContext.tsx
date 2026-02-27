"use client";

import { createContext, useContext, useEffect, useState } from "react";
import { UserProfile } from "../../types/auth";
import { AuthService } from "@/services/auth.service";
import { redirect } from "next/navigation";

interface AuthContextType {
  user: UserProfile;
  storeUser: (userData: UserProfile) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<UserProfile | null>(null);

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
      redirect('/authentication/login'); 
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
          role: me.role,
        });
      } catch (err) {
        setUser(null)
      } finally {
        // Do nothing, let it finish 
      }
    };

    getCurrentUser();
  }, []);

  return (
    <AuthContext.Provider value={{ user: user!, storeUser, logout }}>
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
