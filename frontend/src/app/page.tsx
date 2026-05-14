"use client";

import React from "react";
import { Navbar } from "@/components/compositions/navbar";
import { HeroSection } from "@/components/compositions/hero-section";
import { FeaturesSection } from "@/components/compositions/features-section";
import { Footer } from "@/components/compositions/footer";

export default function RootPage() {
  const scrollToSection = (id: string) => {
    const element = document.getElementById(id);
    if (element) {
      element.scrollIntoView({ behavior: "smooth" });
    }
  };

  return (
    <div className="min-h-screen bg-background text-foreground selection-notion">
      <Navbar onScrollToSection={scrollToSection} />
      
      <main className="pt-24 relative">
        <HeroSection onScrollToSection={scrollToSection} />
        <FeaturesSection />
        <Footer />
      </main>
    </div>
  );
}
