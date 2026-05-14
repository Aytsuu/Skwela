"use client";

import { Header } from "@/components/compositions/header";

export default function MainLayout({children} : {children: React.ReactNode}) {
  return (
    <div className="h-screen flex flex-col bg-background overflow-hidden">
      <Header />
      <main className="flex-1 flex flex-col min-h-0">
        {children}
      </main>
    </div>
  )
}
