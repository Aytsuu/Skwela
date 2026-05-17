"use client";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";
import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar";
import {
  Bell,
  LogOut,
  User,
  LucideIcon,
  CircleQuestionMark,
  LayoutDashboard,
  GraduationCap,
  Settings,
  Check,
  CheckCheck,
  Trash2,
  X,
  Loader2,
} from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "../ui/dialog";
import {
  Popover,
  PopoverContent,
  PopoverDescription,
  PopoverHeader,
  PopoverTitle,
  PopoverTrigger,
} from "../ui/popover";
import React from "react";
import { usePathname } from "next/navigation";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "../ui/alert-dialog";
import Link from "next/link";
import { cn } from "@/lib/utils";
import { useAuth } from "@/context/AuthContext";
import { useNotification } from "@/context/NotificationContext";
import { formatDate } from "@/helpers/dateFormatter";
import { ReleaseItem } from "@/types/release";

interface DropdownItem {
  title: string;
  icon: LucideIcon;
  action: () => void;
}

const navLinks = [
  {
    title: "Dashboard",
    url: "/dashboard",
    icon: LayoutDashboard,
  },
  {
    title: "Classrooms",
    url: "/classrooms",
    icon: GraduationCap,
  },
  {
    title: "Settings",
    url: "/settings",
    icon: Settings,
  }
];

export const Header = () => {
  const { user, logout } = useAuth();
  const { 
    notifications, 
    unreadCount, 
    isLoading, 
    isApplyingBulkAction,
    markReleaseAsRead, 
    markAllAsRead, 
    dismissNotification, 
    clearAll 
  } = useNotification();
  const pathname = usePathname();

  const [isOpenLogoutDialog, setIsOpenLogoutDialog] =
    React.useState<boolean>(false);
  const [selectedRelease, setSelectedRelease] = React.useState<ReleaseItem | null>(null);
  const [pendingNotificationId, setPendingNotificationId] = React.useState<string | null>(null);

  const dropdown_items: DropdownItem[] = [
    {
      title: "Profile",
      icon: User,
      action: () => {},
    },
    {
      title: "Logout",
      icon: LogOut,
      action: () => setIsOpenLogoutDialog(true),
    },
  ];

  const handleOpenRelease = async (release: ReleaseItem) => {
    setSelectedRelease(release);

    if (!release.isRead) {
      setPendingNotificationId(release.releaseId);
      try {
        await markReleaseAsRead(release.releaseId);
      } finally {
        setPendingNotificationId((current) => current === release.releaseId ? null : current);
      }
    }
  };

  const handleDismiss = async (e: React.MouseEvent, releaseId: string) => {
    e.stopPropagation();
    setPendingNotificationId(releaseId);
    try {
      await dismissNotification(releaseId);
      setSelectedRelease((current) => current?.releaseId === releaseId ? null : current);
    } finally {
      setPendingNotificationId((current) => current === releaseId ? null : current);
    }
  };

  React.useEffect(() => {
    if (!selectedRelease) {
      return;
    }

    const stillVisible = notifications.some((notification) => notification.releaseId === selectedRelease.releaseId);
    if (!stillVisible) {
      setSelectedRelease(null);
    }
  }, [notifications, selectedRelease]);

  return (
    <>
      <div className="flex justify-between items-center bg-indigo-50/30 px-6 border-b border-border sticky top-0 z-50 h-14 backdrop-blur-md">
        <div className="flex items-center gap-2 h-full">
          <Link href="/" className="flex items-center gap-1 mr-4 group">
            <div className="relative flex items-center justify-center">
              <img
                src="/assets/esecai_logo.svg" 
                alt="esecai logo" 
                width={30}
                height={30}
                className="transition-transform duration-300 group-hover:rotate-12"
              />
            </div>
          </Link>
          
          <nav className="items-center gap-4 hidden md:flex h-full">
            {navLinks.map((item) => {
              const isActive = pathname.startsWith(item.url);
              return (
                <Link
                  key={item.title}
                  href={item.url}
                  className={cn(
                    "flex items-center gap-2 px-2 py-4 text-sm font-semibold transition-colors relative h-full",
                    isActive 
                      ? "text-primary font-bold" 
                      : "text-muted-foreground hover:text-primary"
                  )}
                >
                  <item.icon size={15} />
                  {item.title}
                  {isActive && (
                    <div className="absolute bottom-0 left-0 w-full h-0.5 bg-primary rounded-t-full" />
                  )}
                </Link>
              );
            })}
          </nav>
        </div>

        <div className="flex gap-4 items-center h-full">
          <CircleQuestionMark
            size={18}
            className="cursor-pointer text-muted-foreground hover:text-primary transition-colors"
          />

          <Popover>
            <PopoverTrigger>
              <div className="relative cursor-pointer text-muted-foreground hover:text-primary transition-colors">
                <Bell size={18} />
                {unreadCount > 0 && (
                  <span className="absolute -right-2 -top-2 min-w-4 rounded-full bg-primary px-1 text-[10px] font-bold leading-4 text-primary-foreground">
                    {unreadCount > 9 ? "9+" : unreadCount}
                  </span>
                )}
              </div>
            </PopoverTrigger>
            <PopoverContent className="w-[360px] p-0" align="end">
              <PopoverHeader className="flex flex-row items-center justify-between">
                <div className="space-y-1">
                  <PopoverTitle>Notification</PopoverTitle>
                  <PopoverDescription>
                    Be notified with the latest updates
                  </PopoverDescription>
                </div>
                <div className="flex items-center gap-1">
                  <button
                    type="button"
                    onClick={() => void markAllAsRead()}
                    disabled={isApplyingBulkAction || unreadCount === 0}
                    className="p-1.5 rounded-md hover:bg-muted text-muted-foreground hover:text-primary transition-colors"
                    title="Mark all as read"
                  >
                    {isApplyingBulkAction ? <Loader2 size={16} className="animate-spin" /> : <CheckCheck size={16} />}
                  </button>
                  <button
                    type="button"
                    onClick={() => void clearAll()}
                    disabled={isApplyingBulkAction || notifications.length === 0}
                    className="p-1.5 rounded-md hover:bg-muted text-muted-foreground hover:text-destructive transition-colors"
                    title="Clear all"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </PopoverHeader>
              <div className="border-t border-border/70">
                {isLoading ? (
                  <div className="px-4 py-6 text-sm text-muted-foreground text-center">
                    <Loader2 size={16} className="animate-spin mx-auto mb-2" />
                    Loading updates...
                  </div>
                ) : notifications.length === 0 ? (
                  <div className="px-4 py-6 text-sm text-muted-foreground">No notifications yet.</div>
                ) : (
                  <div className="max-h-96 overflow-y-auto">
                    {notifications.map((notification) => (
                      <div
                        key={notification.releaseId}
                        className="group relative flex w-full flex-col gap-1 border-b border-border/60 px-4 py-3 text-left transition-colors hover:bg-muted/40 cursor-pointer"
                        onClick={() => void handleOpenRelease(notification)}
                      >
                        <div className="flex items-start justify-between gap-3">
                          <p className={cn("text-sm font-semibold", notification.isRead ? "text-foreground/80" : "text-foreground")}>
                            {notification.title}
                          </p>
                          <div className="flex items-center gap-2">
                            {!notification.isRead && (
                              <div className="flex items-center gap-2">
                                <span className="h-2 w-2 rounded-full bg-primary shrink-0" />
                                <button
                                  type="button"
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    setPendingNotificationId(notification.releaseId);
                                    void markReleaseAsRead(notification.releaseId).finally(() => {
                                      setPendingNotificationId((current) => current === notification.releaseId ? null : current);
                                    });
                                  }}
                                  disabled={pendingNotificationId === notification.releaseId}
                                  className="opacity-0 group-hover:opacity-100 p-1 rounded-md hover:bg-muted-foreground/10 text-muted-foreground hover:text-primary transition-all"
                                  title="Mark as read"
                                >
                                  {pendingNotificationId === notification.releaseId ? <Loader2 size={12} className="animate-spin" /> : <Check size={12} />}
                                </button>
                              </div>
                            )}
                            <button
                              type="button"
                              onClick={(e) => void handleDismiss(e, notification.releaseId)}
                              disabled={pendingNotificationId === notification.releaseId}
                              className="opacity-0 group-hover:opacity-100 p-1 rounded-md hover:bg-muted-foreground/10 text-muted-foreground hover:text-destructive transition-all"
                              title="Delete"
                            >
                              {pendingNotificationId === notification.releaseId ? <Loader2 size={12} className="animate-spin" /> : <X size={12} />}
                            </button>
                          </div>
                        </div>
                        <p className="line-clamp-2 text-xs text-muted-foreground pr-6">{notification.summary}</p>
                        <p className="text-[11px] font-medium text-muted-foreground">
                          {notification.version ? `${notification.version} • ` : ""}
                          {formatDate(notification.publishedAt ?? notification.createdAt)}
                        </p>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </PopoverContent>
          </Popover>

          <DropdownMenu>
            <DropdownMenuTrigger>
              <div className="flex items-center gap-4 cursor-pointer p-1 rounded-full group hover:bg-muted transition-colors">
                <div className="p-0.5 rounded-full bg-gradient-to-r from-indigo-600 to-pink-600 shadow-soft group-hover:shadow-soft-hover transition-all">
                  <Avatar className="w-8 h-8 border-2 border-background">
                    <AvatarImage src={user?.displayImage} alt="User Avatar" />
                    <AvatarFallback className="text-[10px] font-bold">
                      {user?.displayName?.charAt(0).toUpperCase() || 'U'}
                    </AvatarFallback>
                  </Avatar>
                </div>
              </div>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="w-40" align="end">
              <DropdownMenuGroup>
                <DropdownMenuLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">My Account</DropdownMenuLabel>
                {dropdown_items.map((item: DropdownItem, index: number) => (
                  <DropdownMenuItem
                    key={index}
                    className="cursor-pointer"
                    onSelect={item.action}
                  >
                    <item.icon className="mr-2 opacity-70" size={14} /> {item.title}
                  </DropdownMenuItem>
                ))}
              </DropdownMenuGroup>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      {/* Logout Confirmation Dialog */}
      <AlertDialog open={isOpenLogoutDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Log out of your account?</AlertDialogTitle>
            <AlertDialogDescription>
              You will be signed out of your session. Make sure you&apos;ve saved any 
              current classroom progress before leaving.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setIsOpenLogoutDialog(false)}>
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              onClick={logout}
            >
              Yes, Logout
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <Dialog open={selectedRelease !== null} onOpenChange={(open) => !open && setSelectedRelease(null)}>
        <DialogContent className="sm:max-w-2xl">
          <DialogHeader>
            <DialogTitle>{selectedRelease?.title}</DialogTitle>
            <DialogDescription>
              {selectedRelease?.version ? `${selectedRelease.version} • ` : ""}
              {selectedRelease ? formatDate(selectedRelease.publishedAt ?? selectedRelease.createdAt) : ""}
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <p className="text-sm font-medium text-muted-foreground">{selectedRelease?.summary}</p>
            <div className="max-h-[50vh] overflow-y-auto whitespace-pre-wrap text-sm leading-6 text-foreground">
              {selectedRelease?.body}
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};
