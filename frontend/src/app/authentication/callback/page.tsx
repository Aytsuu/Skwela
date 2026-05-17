"use client";

import { useAuth } from "@/context/AuthContext";
import { useRouter, useSearchParams } from "next/navigation";
import React from "react";

const Callback = () => {
  const { storeUser } = useAuth();
  const router = useRouter();
  const urlParams = useSearchParams();

  React.useEffect(() => {
    // Read the URL parameters
      const userId = urlParams.get('userId');
      const email = urlParams.get('email');
      const displayName = urlParams.get('displayName');
      const displayImage = urlParams.get('displayImage');
      const isAdmin = urlParams.get('isAdmin') === 'true';

      if (userId && email && displayName && displayImage) {
          // Set user profile
        storeUser({
          userId: userId,
          email: email,
          displayName: displayName,
          displayImage: displayImage,
          isAdmin
        });

        router.replace('/dashboard');
      } else {
        router.replace('/?error=auth_failed')
      }

  }, [router, storeUser, urlParams]);

  return <div>Authenticating...</div>;
}

const srcappauthenticationcallbackpageComponent = () => {
  return (
    <React.Suspense fallback={<div>Loading...</div>}>
      <Callback/>
    </React.Suspense>
  )
}

export default srcappauthenticationcallbackpageComponent;
