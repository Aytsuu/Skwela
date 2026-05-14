"use client";

import { useAuth } from "@/context/AuthContext";
import { 
  User, 
  Mail, 
  Shield, 
  Bell, 
  Globe, 
  Monitor,
  Camera
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Separator } from "@/components/ui/separator";

export default function SettingsPage() {
  const { user } = useAuth();

  return (
    <div className="flex-1 overflow-y-auto p-4 md:p-6 lg:p-8">
      <div className="w-full max-w-4xl mx-auto space-y-10 pb-20">
        <div>
          <h1 className="text-3xl font-extrabold tracking-tight">Settings</h1>
          <p className="text-sm font-medium text-muted-foreground mt-1">Manage your account and workspace preferences.</p>
        </div>

        <div className="grid gap-8">
          {/* Profile Section */}
          <Card className="border border-border bg-card">
            <CardHeader>
              <CardTitle>Profile</CardTitle>
              <CardDescription>Personal information visible across the platform.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex flex-col sm:flex-row items-start sm:items-center gap-6">
                <div className="relative group">
                  <div className="p-1 rounded-full bg-gradient-to-r from-indigo-600 to-pink-600 shadow-soft group-hover:shadow-soft-hover transition-all">
                    <Avatar className="w-20 h-20 border-4 border-background transition-opacity group-hover:opacity-80">
                      <AvatarImage src={user?.displayImage} />
                      <AvatarFallback className="text-xl font-bold">
                        {user?.displayName?.charAt(0).toUpperCase() || 'U'}
                      </AvatarFallback>
                    </Avatar>
                  </div>
                  <div className="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer z-10">
                     <Camera size={20} className="text-white drop-shadow-md" />
                  </div>
                </div>
                <div className="space-y-1">
                  <Button variant="outline" size="sm" className="h-8 text-xs font-semibold">
                    Change Avatar
                  </Button>
                  <p className="text-[10px] text-muted-foreground mt-1">JPG, GIF or PNG. Max size of 2MB.</p>
                </div>
              </div>

              <Separator className="bg-border/50" />

              <div className="grid gap-4 sm:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="display-name" className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Display Name</Label>
                  <Input id="display-name" defaultValue={user?.displayName || ""} placeholder="e.g. John Doe" />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="email" className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Email Address</Label>
                  <Input id="email" type="email" defaultValue={user?.email || ""} disabled className="bg-muted/30" />
                </div>
              </div>
              
              <div className="flex justify-end pt-2">
                <Button size="sm" className="h-9 px-6">Save Profile</Button>
              </div>
            </CardContent>
          </Card>

          {/* Workspace Section */}
          <Card className="border border-border bg-card">
            <CardHeader>
              <CardTitle>Workspace</CardTitle>
              <CardDescription>Control how you interact with your classrooms.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 rounded-md bg-muted">
                    <Monitor size={16} />
                  </div>
                  <div>
                    <p className="text-sm font-medium">Appearance</p>
                    <p className="text-xs text-muted-foreground">Customize how esecai looks on your device.</p>
                  </div>
                </div>
                <Button variant="outline" size="sm" className="h-8">Set Theme</Button>
              </div>

              <Separator className="bg-border/50" />

              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 rounded-md bg-muted">
                    <Bell size={16} />
                  </div>
                  <div>
                    <p className="text-sm font-medium">Notifications</p>
                    <p className="text-xs text-muted-foreground">Choose what updates you want to receive.</p>
                  </div>
                </div>
                <Button variant="outline" size="sm" className="h-8">Configure</Button>
              </div>

              <Separator className="bg-border/50" />

              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 rounded-md bg-muted">
                    <Globe size={16} />
                  </div>
                  <div>
                    <p className="text-sm font-medium">Language & Region</p>
                    <p className="text-xs text-muted-foreground">Update your local preferences.</p>
                  </div>
                </div>
                <Button variant="outline" size="sm" className="h-8">Manage</Button>
              </div>
            </CardContent>
          </Card>

          {/* Danger Zone */}
          <Card className="border border-destructive/30 bg-destructive/[0.03] shadow-none">
            <CardHeader>
              <CardTitle className="text-destructive">Danger Zone</CardTitle>
              <CardDescription>Permanently delete your account and all associated data.</CardDescription>
            </CardHeader>
            <CardContent>
              <Button variant="destructive" size="sm" className="h-9 font-semibold">Delete Account</Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
