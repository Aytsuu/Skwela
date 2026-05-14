"use client";

import { useAuth } from "@/context/AuthContext";
import { 
  GraduationCap, 
  Users, 
  Activity, 
  Clock, 
  Plus
} from "lucide-react";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { StatsGrid, StatItem } from "@/components/compositions/stats-grid";
import { RecentActivity, ActivityItem } from "@/components/compositions/recent-activity";
import { QuickLinks, QuickLinkItem } from "@/components/compositions/quick-links";

export default function DashboardPage() {
  const { user } = useAuth();

  const stats: StatItem[] = [
    {
      title: "Total Classrooms",
      value: "12",
      icon: GraduationCap,
      description: "+2 from last month",
      color: "text-blue-600",
      bg: "bg-blue-50"
    },
    {
      title: "Active Students",
      value: "450",
      icon: Users,
      description: "Across all classes",
      color: "text-emerald-600",
      bg: "bg-emerald-50"
    },
    {
      title: "Avg. Engagement",
      value: "84%",
      icon: Activity,
      description: "+5% vs average",
      color: "text-indigo-600",
      bg: "bg-indigo-50"
    },
    {
      title: "Pending Reviews",
      value: "8",
      icon: Clock,
      description: "Needs attention",
      color: "text-amber-600",
      bg: "bg-amber-50"
    }
  ];

  const recentActivities: ActivityItem[] = [
    { id: 1, message: "High engagement detected in \"Introduction to AI\"", time: "15 minutes ago", location: "Room 302" },
    { id: 2, message: "High engagement detected in \"Introduction to AI\"", time: "15 minutes ago", location: "Room 302" },
    { id: 3, message: "High engagement detected in \"Introduction to AI\"", time: "15 minutes ago", location: "Room 302" },
  ];

  const quickLinks: QuickLinkItem[] = [
    { label: "Manage Students", href: "/classrooms" },
    { label: "Workspace Settings", href: "/settings" },
    { label: "Help & Documentation", href: "#" },
  ];

  return (
    <div className="flex-1 overflow-y-auto p-4 md:p-6 lg:p-8">
      <div className="w-full max-w-7xl mx-auto space-y-10 pb-20">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-3xl font-extrabold tracking-tight">Welcome back, {user?.displayName || "Professor"}!</h1>
            <p className="text-sm font-medium text-muted-foreground mt-1">Here&apos;s what&apos;s happening in your classrooms today.</p>
          </div>
        </div>

        <StatsGrid stats={stats} />

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2">
            <RecentActivity activities={recentActivities} />
          </div>
          <div>
            <QuickLinks links={quickLinks} />
          </div>
        </div>
      </div>
    </div>
  );
}
