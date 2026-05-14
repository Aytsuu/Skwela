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
  Settings
} from "lucide-react";
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

import Image from "next/image";

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
  const pathname = usePathname();

  const [isOpenLogoutDialog, setIsOpenLogoutDialog] =
    React.useState<boolean>(false);

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

  return (
    <>
      <div className="flex justify-between items-center bg-indigo-50/30 px-6 border-b border-border sticky top-0 z-50 h-14 backdrop-blur-md">
        <div className="flex items-center gap-2 h-full">
          <Link href="/" className="flex items-center gap-1 mr-4 group">
            <div className="relative flex items-center justify-center">
              <Image 
                src="/assets/esecai_logo.svg" 
                alt="esecai logo" 
                width={40} 
                height={40} 
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
              <Bell size={18} className="cursor-pointer text-muted-foreground hover:text-primary transition-colors" />
            </PopoverTrigger>
            <PopoverContent>
              <PopoverHeader>
                <PopoverTitle>Notification</PopoverTitle>
                <PopoverDescription>
                  Be notified with the latest updates
                </PopoverDescription>
              </PopoverHeader>
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
    </>
  );
};
