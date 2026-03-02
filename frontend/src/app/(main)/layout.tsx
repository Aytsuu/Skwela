"use client";

import { Header } from "@/components/compositions/header";

export default ({children} : {children: React.ReactNode}) => {
  return (
    <div className="w-screen h-screen flex flex-col bg-custom-primary">
      <Header/>
      <div className="p-5">
        {children}
      </div>
    </div>
  )
}