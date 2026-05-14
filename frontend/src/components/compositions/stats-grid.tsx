"use client";

import React from "react";
import { motion } from "framer-motion";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { LucideIcon } from "lucide-react";

export interface StatItem {
  title: string;
  value: string;
  icon: LucideIcon;
  description: string;
  color: string;
  bg: string;
}

export interface StatsGridProps {
  stats: StatItem[];
}

export const StatsGrid = ({ stats }: StatsGridProps) => {
  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      {stats.map((stat, i) => (
        <motion.div
          key={stat.title}
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: i * 0.1 }}
        >
          <Card className="border border-border bg-card shadow-soft hover:shadow-soft-hover transition-all">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-[10px] font-bold text-muted-foreground uppercase tracking-widest">
                {stat.title}
              </CardTitle>
              <div className={`p-2 rounded-md ${stat.bg} ${stat.color}`}>
                <stat.icon size={16} />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-black tracking-tighter">{stat.value}</div>
              <p className="text-xs font-medium text-muted-foreground mt-1">
                {stat.description}
              </p>
            </CardContent>
          </Card>
        </motion.div>
      ))}
    </div>
  );
};
