import Link from "next/link";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";
import { Avatar, AvatarFallback, AvatarImage } from "../ui/avatar";
import { Bell, LogOut, Settings, User, LucideIcon } from "lucide-react";
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
import { useAuth } from "../context/AuthContext";
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

interface NavItem {
  path: string;
  title: string;
}

interface DropdownItem {
  title: string;
  icon: LucideIcon;
  action: () => void
}

export const Header = () => {
  const { user, logout } = useAuth();
  const pathname = usePathname();
  const currentPath = pathname.split("/").pop();

  const [isOpenLogoutDialog, setIsOpenLogoutDialog] = React.useState<boolean>(false);

  const nav_items: NavItem[] = [
    {
      path: "dashboard",
      title: "Dashboard",
    },
    {
      path: "classrooms",
      title: "Classrooms",
    },
  ];

  const dropdown_items: DropdownItem[] = [
    {
      title: "Profile",
      icon: User,
      action: () => {}
    },
    {
      title: "Settings",
      icon: Settings,
      action: () => {}
    },
    {
      title: "Logout",
      icon: LogOut,
      action: () => setIsOpenLogoutDialog(true)
    },
  ];

  return (
    <>
      <div className="flex justify-between items-center py-2 px-5 bg-white shadow-sm">
        <div className="flex items-center gap-4">
          {nav_items.map((item: NavItem) => (
            <Link key={item.path} href={item.path} className="relative">
              {item.title}
              <div
                className={`
              absolute w-0 left-1/2 -translate-x-1/2 ${item.path.toLowerCase() === currentPath && "w-full"} h-0.5 bg-black rounded-full
              transition-all duration-300
            `}
              />
            </Link>
          ))}
        </div>
        <div className="flex gap-4">
          <Popover>
            <PopoverTrigger>
              <Bell className="cursor-pointer hover:scale-105" />
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
              <div className="flex items-center gap-4 cursor-pointer p-2 hover:bg-gray-100 rounded-sm group">
                <Avatar>
                  <AvatarImage
                    src="https://github.com/shadcn.png"
                    alt="@shadcn"
                  />
                  <AvatarFallback>CN</AvatarFallback>
                </Avatar>
                <div className="text-start flex flex-col truncate">
                  <span className="font-medium">
                    {user?.displayName ?? "User"}
                  </span>
                  <span className="text-xs text-muted-foreground">
                    {user?.email}
                  </span>
                </div>
              </div>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="w-40" align="start">
              <DropdownMenuGroup>
                <DropdownMenuLabel>My Account</DropdownMenuLabel>
                {dropdown_items.map((item: DropdownItem, index: number) => (
                  <DropdownMenuItem key={index} className="cursor-pointer" onClick={item.action}>
                    <item.icon /> {item.title}
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
            <AlertDialogTitle>Are you sure of this action?</AlertDialogTitle>
            <AlertDialogDescription></AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setIsOpenLogoutDialog(false)}>Cancel</AlertDialogCancel>
            <AlertDialogAction variant={"destructive"} onClick={logout}>Yes, Logout</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
};
