'use client';

import { createContext, useContext, useEffect, useState } from "react";
import { UserProfile } from "../../types/auth";
import Cookies from 'js-cookie';
import { AuthService } from "../../services/auth.service";
import { jwtDecode } from "jwt-decode"; 
interface AuthContextType {
  user: UserProfile;
  storeUser: (userData: UserProfile) => void;
}

const AuthContext = createContext<AuthContextType | null>(null)

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<UserProfile>();

  const storeUser = (userData: UserProfile) => {
    setUser(userData);
  }

  useEffect(() => {
    const restoreUser = () => {
      const accessToken = Cookies.get('accessToken')

      if (!accessToken) return;

      try {
        const decode: any = jwtDecode(accessToken);
        const userData: UserProfile = {
          userId: decode.sub,
          username: decode.unique_name,
          email: decode.email,
          displayName: decode.name,
          displayImage: decode.display_image,
          role: decode['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
        }
        setUser(userData);
      } catch (error) {
        alert("Invalid token found. Please login again.")
        AuthService.logout();
      }
    }

    restoreUser();
  }, [])

  return (
    <AuthContext.Provider value={{ user: user!, storeUser }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
}