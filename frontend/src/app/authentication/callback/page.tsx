"use client";

import { useRouter, useSearchParams } from "next/navigation";
import React from "react";
import { useAuth } from "@/components/context/AuthContext";

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

      if (userId && email && displayName && displayImage) {
          // Set user profile
        storeUser({
          userId: userId,
          email: email,
          displayName: displayName,
          displayImage: displayImage
        });

        router.replace('/dashboard');
      } else {
        router.replace('/?error=auth_failed')
      }

  }, []);

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
