"use client";

import axios from "axios";
import { useRouter } from "next/navigation";
import React from "react";
import { toast } from "sonner";

export default function Protected({children, error} : {children: React.ReactNode; error: unknown}) {
  const router = useRouter();
  const [isMounted, setIsMounted] = React.useState<boolean>(false);

  React.useEffect(() => {
    setIsMounted(true)
  }, [])

  React.useEffect(() => {
    if (axios.isAxiosError(error) && error.response) {
      const status = error.response.status;
      if (status === 401) {
        toast.error("Page not found");
        router.replace("/authentication/login");
      }
    }
  }, [isMounted, error])

  return children
}