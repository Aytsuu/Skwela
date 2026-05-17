"use client";

import { useAuth } from "@/context/AuthContext";
import { useNotification } from "@/context/NotificationContext";
import { 
  Bell, 
  Globe, 
  Monitor,
  Camera,
  PencilLine,
  Rocket
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Separator } from "@/components/ui/separator";
import { Textarea } from "@/components/ui/textarea";
import { ReleaseEditorInput, ReleaseItem } from "@/types/release";
import { ReleaseService } from "@/services/release.service";
import React from "react";
import { formatDate } from "@/helpers/dateFormatter";
import { toast } from "sonner";
import { cn } from "@/lib/utils";

const emptyReleaseDraft: ReleaseEditorInput = {
  version: "",
  title: "",
  summary: "",
  body: ""
};

export default function SettingsPage() {
  const { user } = useAuth();
  const { refreshNotifications } = useNotification();
  const [releaseDraft, setReleaseDraft] = React.useState<ReleaseEditorInput>(emptyReleaseDraft);
  const [adminReleases, setAdminReleases] = React.useState<ReleaseItem[]>([]);
  const [selectedReleaseId, setSelectedReleaseId] = React.useState<string | null>(null);
  const [isLoadingReleases, setIsLoadingReleases] = React.useState(false);
  const [isSavingRelease, setIsSavingRelease] = React.useState(false);
  const [isPublishingRelease, setIsPublishingRelease] = React.useState(false);

  const selectedRelease = adminReleases.find((release) => release.releaseId === selectedReleaseId) ?? null;

  const syncRelease = React.useCallback((nextRelease: ReleaseItem) => {
    setAdminReleases((current) => {
      const remaining = current.filter((release) => release.releaseId !== nextRelease.releaseId);
      return [nextRelease, ...remaining].sort((left, right) =>
        new Date(right.createdAt).getTime() - new Date(left.createdAt).getTime()
      );
    });
  }, []);

  const loadAdminReleases = React.useCallback(async () => {
    if (!user?.isAdmin) {
      return;
    }

    setIsLoadingReleases(true);

    try {
      const releases = await ReleaseService.getAdminReleases();
      setAdminReleases(releases);
    } finally {
      setIsLoadingReleases(false);
    }
  }, [user?.isAdmin]);

  React.useEffect(() => {
    void loadAdminReleases();
  }, [loadAdminReleases]);

  const handleReleaseDraftChange = (field: keyof ReleaseEditorInput, value: string) => {
    setReleaseDraft((current) => ({
      ...current,
      [field]: value
    }));
  };

  const handleSelectRelease = (release: ReleaseItem) => {
    setSelectedReleaseId(release.releaseId);
    setReleaseDraft({
      version: release.version ?? "",
      title: release.title,
      summary: release.summary,
      body: release.body
    });
  };

  const handleNewRelease = () => {
    setSelectedReleaseId(null);
    setReleaseDraft(emptyReleaseDraft);
  };

  const handleSaveRelease = async () => {
    setIsSavingRelease(true);

    try {
      const payload: ReleaseEditorInput = {
        version: releaseDraft.version?.trim() ?? "",
        title: releaseDraft.title.trim(),
        summary: releaseDraft.summary.trim(),
        body: releaseDraft.body.trim()
      };

      const release = selectedReleaseId
        ? await ReleaseService.updateDraft(selectedReleaseId, payload)
        : await ReleaseService.createDraft(payload);

      syncRelease(release);
      setSelectedReleaseId(release.releaseId);
      setReleaseDraft({
        version: release.version ?? "",
        title: release.title,
        summary: release.summary,
        body: release.body
      });
      toast.success(selectedReleaseId ? "Release draft updated" : "Release draft created");
    } catch {
      toast.error("Unable to save release draft");
    } finally {
      setIsSavingRelease(false);
    }
  };

  const handlePublishRelease = async () => {
    if (!selectedReleaseId) {
      return;
    }

    setIsPublishingRelease(true);

    try {
      const release = await ReleaseService.publishRelease(selectedReleaseId);
      syncRelease(release);
      await refreshNotifications();
      toast.success("Release published");
    } catch {
      toast.error("Unable to publish release");
    } finally {
      setIsPublishingRelease(false);
    }
  };

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

          {user?.isAdmin && (
            <Card className="border border-border bg-card">
              <CardHeader>
                <CardTitle>Release Updates</CardTitle>
                <CardDescription>Write and publish product update notes that appear in the in-app notification bell.</CardDescription>
              </CardHeader>
              <CardContent className="grid gap-6 lg:grid-cols-[280px_minmax(0,1fr)]">
                <div className="space-y-4">
                  <div className="flex items-center justify-between gap-3">
                    <p className="text-sm font-semibold">Drafts and published updates</p>
                    <Button variant="outline" size="sm" className="h-8" onClick={handleNewRelease}>
                      New draft
                    </Button>
                  </div>

                  <div className="rounded-xl border border-border/70">
                    {isLoadingReleases ? (
                      <div className="px-4 py-6 text-sm text-muted-foreground">Loading release history...</div>
                    ) : adminReleases.length === 0 ? (
                      <div className="px-4 py-6 text-sm text-muted-foreground">No release notes yet.</div>
                    ) : (
                      <div className="max-h-[420px] overflow-y-auto">
                        {adminReleases.map((release) => (
                          <button
                            key={release.releaseId}
                            type="button"
                            onClick={() => handleSelectRelease(release)}
                            className={cn(
                              "flex w-full flex-col gap-2 border-b border-border/60 px-4 py-3 text-left transition-colors hover:bg-muted/40",
                              selectedReleaseId === release.releaseId && "bg-muted/50"
                            )}
                          >
                            <div className="flex items-center justify-between gap-3">
                              <p className="text-sm font-semibold">{release.title}</p>
                              <span
                                className={cn(
                                  "rounded-full px-2 py-0.5 text-[10px] font-bold uppercase tracking-wide",
                                  release.status === "published"
                                    ? "bg-emerald-100 text-emerald-700"
                                    : "bg-amber-100 text-amber-700"
                                )}
                              >
                                {release.status}
                              </span>
                            </div>
                            <p className="line-clamp-2 text-xs text-muted-foreground">{release.summary}</p>
                            <p className="text-[11px] font-medium text-muted-foreground">
                              {release.version ? `${release.version} • ` : ""}
                              {formatDate(release.publishedAt ?? release.createdAt)}
                            </p>
                          </button>
                        ))}
                      </div>
                    )}
                  </div>
                </div>

                <div className="space-y-4">
                  <div className="flex items-center justify-between gap-3">
                    <div>
                      <p className="text-sm font-semibold">
                        {selectedRelease ? "Edit release note" : "Compose release note"}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        Published notes become visible to all authenticated users.
                      </p>
                    </div>
                    {selectedRelease?.status === "published" && (
                      <span className="rounded-full bg-emerald-100 px-2.5 py-1 text-[10px] font-bold uppercase tracking-wide text-emerald-700">
                        Published
                      </span>
                    )}
                  </div>

                  <div className="grid gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="release-version" className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Version</Label>
                      <Input
                        id="release-version"
                        value={releaseDraft.version ?? ""}
                        onChange={(event) => handleReleaseDraftChange("version", event.target.value)}
                        placeholder="e.g. 2.0.0"
                        disabled={selectedRelease?.status === "published"}
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="release-title" className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Title</Label>
                      <Input
                        id="release-title"
                        value={releaseDraft.title}
                        onChange={(event) => handleReleaseDraftChange("title", event.target.value)}
                        placeholder="What changed?"
                        disabled={selectedRelease?.status === "published"}
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="release-summary" className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Summary</Label>
                      <Textarea
                        id="release-summary"
                        value={releaseDraft.summary}
                        onChange={(event) => handleReleaseDraftChange("summary", event.target.value)}
                        placeholder="Short bell preview copy"
                        className="min-h-24"
                        disabled={selectedRelease?.status === "published"}
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="release-body" className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">Release note body</Label>
                      <Textarea
                        id="release-body"
                        value={releaseDraft.body}
                        onChange={(event) => handleReleaseDraftChange("body", event.target.value)}
                        placeholder="Write the full update details shown in the modal."
                        className="min-h-56"
                        disabled={selectedRelease?.status === "published"}
                      />
                    </div>
                  </div>

                  <div className="flex flex-wrap justify-end gap-3">
                    <Button variant="outline" className="h-9" onClick={handleNewRelease}>
                      <PencilLine size={15} className="mr-2" />
                      Reset
                    </Button>
                    <Button
                      className="h-9"
                      onClick={() => void handleSaveRelease()}
                      disabled={isSavingRelease || selectedRelease?.status === "published"}
                    >
                      {isSavingRelease ? "Saving..." : "Save draft"}
                    </Button>
                    <Button
                      className="h-9 bg-emerald-600 text-white hover:bg-emerald-700"
                      onClick={() => void handlePublishRelease()}
                      disabled={!selectedReleaseId || isPublishingRelease || selectedRelease?.status === "published"}
                    >
                      <Rocket size={15} className="mr-2" />
                      {isPublishingRelease ? "Publishing..." : "Publish"}
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

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
