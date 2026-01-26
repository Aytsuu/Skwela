"use client";

import axios, { AxiosError } from "axios";
import { useRouter } from "next/navigation";
import { useEffect } from "react";

export default function Protected({children, error} : {children: React.ReactNode; error: any}) {
  const router = useRouter();

  useEffect(() => {
    if (axios.isAxiosError(error) && error.response) {
      const status = error.response.status;
      if (status === 404) router.replace("/404/not-found")
    }
  }, [error])

  return children
}