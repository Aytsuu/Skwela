"use client";

import { Button } from "@/components/ui/button";
import { Shield, Github } from "lucide-react";
import Link from "next/link";
import React from "react";

export interface NavbarProps {
  onScrollToSection: (id: string) => void;
}

export const Navbar = ({ onScrollToSection }: NavbarProps) => {
  return (
    <nav className="fixed top-0 left-0 right-0 z-50 shadow-sm bg-indigo-50/30 backdrop-blur-lg transition-all">
      <div className="flex justify-between items-center mx-auto max-w-6xl py-4 px-8">
        <span 
          className="font-bold text-xl tracking-tight cursor-pointer flex items-center group" 
          onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
        >
          <img
            src="/assets/esecai_logo.svg" 
            alt="esecai logo" 
            width={34}
            height={34}
            className="transition-transform duration-300 group-hover:rotate-12"
          />
        </span>
        <ul className="hidden md:flex gap-10 text-sm font-medium text-muted-foreground">
          <li 
            className="hover:text-primary cursor-pointer transition-colors" 
            onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
          >
            Home
          </li>
          <li 
            className="hover:text-primary cursor-pointer transition-colors" 
            onClick={() => onScrollToSection("features")}
          >
            Services
          </li>
          <li 
            className="hover:text-primary cursor-pointer transition-colors" 
            onClick={() => onScrollToSection("how-it-works")}
          >
            How it Works
          </li>
        </ul>

        <div className="flex items-center gap-6">
          <span 
            onClick={() => window.open("https://github.com/Aytsuu/esecai", "_blank")}
            className="cursor-pointer text-muted-foreground hover:text-primary transition-colors"
          >
            <Github size={18} />
          </span>

          <Link href="authentication/login">
            <Button variant="default" size="sm" className="bg-primary font-semibold px-6 rounded-full h-9">
              Get Started
            </Button>
          </Link>
        </div>
      </div>
    </nav>
  );
};
