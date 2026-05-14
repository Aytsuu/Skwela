"use client";

import React from "react";
import Link from "next/link";
import { ArrowLeft, Shield } from "lucide-react";
import { LoginForm } from "@/components/compositions/login-form";

import Image from "next/image";

const LoginPage = () => {
  return (
    <div className="w-screen h-screen flex flex-col justify-center items-center bg-background relative selection-notion">
      <Link href="/" className="absolute top-8 left-8 flex items-center gap-2 text-xs font-bold uppercase tracking-widest text-muted-foreground hover:text-primary transition-colors">
        <ArrowLeft className="w-4 h-4" />
        Back to Home
      </Link>
      
      <div className="mb-8 flex items-center justify-center gap-1 group">
        <Image 
          src="/assets/esecai_logo.svg" 
          alt="esecai logo" 
          width={44} 
          height={44} 
          priority
          className="transition-transform duration-300 group-hover:rotate-12"
        />
        <span className="font-black text-3xl tracking-tighter">esecai</span>
      </div>

      <LoginForm />
    </div>
  );
}

export default LoginPage;
