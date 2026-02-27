"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { useAuth } from "@/components/context/AuthContext";

export default function AuthCallback() {
  const { storeUser } = useAuth();
  const router = useRouter();

  useEffect(() => {
    // Read the URL parameters
      const urlParams = new URLSearchParams(window.location.search);
      const userId = urlParams.get('userId');
      const email = urlParams.get('email');
      const displayName = urlParams.get('displayName');
      const role = urlParams.get('role');

      if (userId && email && displayName && role) {
          // Set user profile
        storeUser({
          userId: userId,
          email: email,
          displayName: displayName,
          role: role
        });

        router.replace('/dashboard');
      } else {
        router.replace('/?error=auth_failed')
      }

  }, []);

  return <div>Authenticating...</div>;
}
