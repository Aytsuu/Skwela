"use client";

import React from "react";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { motion } from "framer-motion";
import { DashboardVisualization } from "./dashboard-visualization";

export interface HeroSectionProps {
  onScrollToSection: (id: string) => void;
}

export const HeroSection = ({ onScrollToSection }: HeroSectionProps) => {
  const fadeInUp = {
    initial: { opacity: 0, y: 10 },
    animate: { opacity: 1, y: 0 },
    transition: { duration: 0.4, ease: "easeOut" }
  };

  const staggerContainer = {
    animate: {
      transition: {
        staggerChildren: 0.1
      }
    }
  };

  return (
    <section className="py-24 md:py-40 px-8 max-w-6xl mx-auto text-center relative overflow-hidden">
      {/* Background Blobs */}
      <div className="bg-blob w-[500px] h-[500px] bg-primary/30 -top-24 -left-24" />
      <div className="bg-blob w-[400px] h-[400px] bg-secondary/20 bottom-0 -right-24" />

      <motion.div 
        initial="initial"
        animate="animate"
        variants={staggerContainer}
        className="space-y-8 relative z-10"
      >
        <motion.div variants={fadeInUp} className="space-y-6">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full border border-primary/20 bg-primary/5 text-[11px] font-bold uppercase tracking-wider text-primary shadow-glow">
            <span className="w-2 h-2 rounded-full bg-primary animate-pulse"></span>
            The future of classroom security
          </div>
          <h1 className="text-5xl md:text-8xl font-black tracking-tighter leading-[1.05] selection:bg-primary selection:text-white">
            The Classroom, <br/><span className="gradient-text">Evolved.</span>
          </h1>
          <p className="text-lg md:text-xl text-muted-foreground max-w-2xl mx-auto leading-relaxed font-medium">
            AI-powered security and engagement analysis for modern education. 
            Focus on teaching while we handle the integrity.
          </p>
        </motion.div>

        <motion.div variants={fadeInUp} className="flex flex-col sm:flex-row gap-4 justify-center items-center pt-6">
          <Link href="authentication/login">
            <Button size="lg" className="bg-indigo-600 text-white hover:bg-indigo-700 h-12 px-10 text-base font-bold rounded-full transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5">
              Get Started 
            </Button>
          </Link>
          <Button 
            variant="outline" 
            size="lg" 
            className="h-12 px-10 text-base font-bold rounded-full border-border hover:bg-muted/50 transition-all shadow-sm hover:shadow-md" 
            onClick={() => onScrollToSection("features")}
          >
            Platform Tour
          </Button>
        </motion.div>

        <DashboardVisualization />
      </motion.div>
    </section>
  );
};
