"use client";

import { Button } from "@/components/ui/button";
import { Github } from "lucide-react";
import Link from "next/link";

export default function Home() {
  return (
      <div className="flex justify-between a items-center mx-auto max-w-5xl py-6">
        <span className="font-bold">Skwela</span>
        <ul className="flex gap-6">
          <Link href={"#"}>Home</Link>
          <Link href={"#"}>Services</Link>
          <Link href={"#"}>Stories</Link>
          <Link href={"#"}>Pricing</Link>
          <Link href={"#"}>Docs</Link>
        </ul>
        <div className="flex items-center gap-4">
          <span 
            onClick={() => {
              window.open("https://github.com/Aytsuu/Skwela", "_blank")
            }}
            className="cursor-pointer"
          >
            <Github />
          </span>
          <Link href="authentication/login">
            <Button className="rounded-full cursor-pointer">
            Start Learning
          </Button>
          </Link>
        </div>
      </div>
  );
}
