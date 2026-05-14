import React from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Activity } from "lucide-react";

export interface ActivityItem {
  id: number;
  message: string;
  time: string;
  location: string;
}

export interface RecentActivityProps {
  activities: ActivityItem[];
}

export const RecentActivity = ({ activities }: RecentActivityProps) => {
  return (
    <Card className="border border-border bg-card shadow-soft">
      <CardHeader>
        <CardTitle className="text-xl font-bold tracking-tight">Recent Activity</CardTitle>
        <CardDescription className="text-sm font-medium">Latest engagement updates from your active sessions.</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-6">
          {activities.map((activity) => (
            <div key={activity.id} className="flex items-center gap-4 group">
              <div className="w-10 h-10 rounded-full bg-muted flex items-center justify-center shrink-0 group-hover:bg-primary/10 transition-colors">
                <Activity size={18} className="text-muted-foreground group-hover:text-primary transition-colors" />
              </div>
              <div className="flex-1 space-y-0.5">
                <p className="text-sm font-semibold tracking-tight">{activity.message}</p>
                <p className="text-[11px] font-medium text-muted-foreground">{activity.time} • {activity.location}</p>
              </div>
              <Button variant="ghost" size="sm" className="h-8 px-3 text-xs font-bold hover:bg-muted">
                View
              </Button>
            </div>
          ))}
          <Button variant="outline" className="w-full mt-4 text-xs font-bold h-9 border-border/60 hover:bg-muted">
            View All Activity
          </Button>
        </div>
      </CardContent>
    </Card>
  );
};
