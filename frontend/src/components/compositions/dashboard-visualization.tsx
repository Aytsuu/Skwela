"use client";

import React from "react";
import { motion } from "framer-motion";

export const DashboardVisualization = () => {
  const fadeInUp = {
    initial: { opacity: 0, y: 10 },
    animate: { opacity: 1, y: 0 },
    transition: { duration: 0.4, ease: "easeOut" }
  };

  return (
    <div className="perspective-2000 mt-24">
      <motion.div 
        variants={fadeInUp} 
        className="isometric-card mx-auto max-w-5xl border border-border rounded-xl bg-card overflow-hidden shadow-soft-hover"
      >
        <div className="w-full h-10 bg-muted/30 border-b border-border flex items-center px-5 gap-2">
            <div className="w-3 h-3 rounded-full border border-border"></div>
            <div className="w-3 h-3 rounded-full border border-border"></div>
            <div className="w-3 h-3 rounded-full border border-border"></div>
            <div className="ml-4 h-4 w-32 bg-border/40 rounded-sm"></div>
        </div>
        <div className="w-full min-h-[400px] bg-background p-10">
            <div className="grid grid-cols-12 gap-8 h-full">
                <div className="col-span-8 space-y-8">
                <div className="h-48 w-full border border-border bg-muted/10 rounded-lg p-6">
                    <div className="h-4 w-32 bg-border/50 rounded mb-6"></div>
                    <div className="grid grid-cols-10 gap-2 items-end h-24">
                        {[40, 70, 45, 90, 65, 80, 55, 75, 50, 85].map((h, i) => (
                        <div key={i} className="bg-primary/20 border border-primary/30 rounded-t-sm w-full" style={{ height: `${h}%` }}></div>
                        ))}
                    </div>
                </div>
                <div className="grid grid-cols-2 gap-8">
                    <div className="h-32 border border-border bg-muted/10 rounded-lg p-6">
                    <div className="h-4 w-24 bg-border/50 rounded mb-4"></div>
                    <div className="h-8 w-16 bg-primary/10 rounded border border-primary/20"></div>
                    </div>
                    <div className="h-32 border border-border bg-muted/10 rounded-lg p-6">
                    <div className="h-4 w-28 bg-border/50 rounded mb-4"></div>
                    <div className="h-8 w-16 bg-secondary/10 rounded border border-secondary/20"></div>
                    </div>
                </div>
                </div>
                <div className="col-span-4 border border-border bg-muted/5 rounded-lg p-6 space-y-6">
                <div className="h-4 w-24 bg-border/50 rounded"></div>
                {[1, 2, 3, 4, 5].map(i => (
                    <div key={i} className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full border border-border bg-muted/20"></div>
                    <div className="space-y-2 flex-1">
                        <div className="h-3 w-full bg-border/30 rounded"></div>
                        <div className="h-2 w-2/3 bg-border/20 rounded"></div>
                    </div>
                    </div>
                ))}
                </div>
            </div>
        </div>
      </motion.div>
    </div>
  );
};
