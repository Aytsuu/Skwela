"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";
import Cookies from "js-cookie";
import {jwtDecode} from "jwt-decode";
import { useAuth } from "@/components/context/AuthContext";
import { UserProfile } from "@/types/auth";

export default function AuthCallback() {
  const { storeUser } = useAuth();
  const router = useRouter();

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const accessToken = params.get("token");
    const refreshToken = params.get("refresh");
    if (accessToken && refreshToken) {
      const sixtyMinutes = new Date(new Date().getTime() + 60 * 60 * 1000);
      const isProduction = process.env.NODE_ENV === "production";

      Cookies.set("accessToken", accessToken, {
        expires: sixtyMinutes,
        path: "/",
      });

      Cookies.set("refreshToken", refreshToken, {
        expires: 7,
        path: "/",
      });

      const decode: any = jwtDecode(accessToken);
      const userData: UserProfile = {
        userId: decode.sub,
        username: decode.unique_name,
        email: decode.email,
        displayName: decode.name,
        displayImage: decode.display_image,
        role: decode['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      };

      storeUser(userData);
      router.replace('/dashboard');
    } else {
      router.replace('/?error=auth_failed');
    }
  }, []);

  return <div>Authenticating...</div>;
}
