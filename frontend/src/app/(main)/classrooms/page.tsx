"use client";

import {
  Plus,
  BookOpen,
  Loader2,
  LucideIcon,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { Dialog, DialogFooter, DialogHeader } from "@/components/ui/dialog";
import {
  DialogContent,
  DialogDescription,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useForm } from "react-hook-form";
import z from "zod";
import { classroomSchema } from "@/schemas/classroom.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/components/ui/button";
import React from "react";
import {
  useCreateClassroom,
  useGetCreatedClassrooms,
} from "@/hooks/use-classroom";
import { ClassroomData } from "@/types/classroom";
import Link from "next/link";
import {
  Card,
} from "@/components/ui/card";
import ClassroomForm from "./ClassroomForm";
import { queryError } from "@/helpers/errorDisplay";
import { slugFormat } from "@/helpers/urlFormatter";
import Image from "next/image";
import { motion } from "framer-motion";

// ─── Card Components ──────────────────────────────────────────────────────────
function OwnedClassroomCard({ classroom }: { classroom: ClassroomData }) {
  const classNameUrl = slugFormat(classroom.className);

  return (
    <Link href={`/classrooms/${classNameUrl}?id=${classroom.classId}`} className="block group h-full">
      <Card className="h-full overflow-hidden border border-border bg-card transition-all duration-300 hover:shadow-soft-hover hover:-translate-y-2">
        <div className="p-6 flex flex-col items-center text-center space-y-6">
          <div className="relative w-28 h-28 rounded-full overflow-hidden shadow-soft transition-all duration-300 flex items-center justify-center">
            {classroom.classBanner ? (
              <img
                src={classroom.classBanner}
                alt={`${classroom.className} banner`}
                className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-110"
              />
            ) : (
              <div className="h-full w-full bg-gray-200 flex items-center justify-center">
              </div>
            )}
          </div>
          <div className="space-y-1">
            <h3 className="text-base font-semibold tracking-tight group-hover:gradient-text transition-colors">
              {classroom.className}
            </h3>
            <p className="line-clamp-2 text-sm text-muted-foreground font-medium leading-relaxed">
              {classroom.classDescription || "No description provided."}
            </p>
          </div>
        </div>
      </Card>
    </Link>
  );
}

function EmptyState({
  icon: Icon,
  title,
  description,
  action,
}: {
  icon: LucideIcon;
  title: string;
  description: string;
  action?: React.ReactNode;
}) {
  return (
    <div className="col-span-full flex flex-col items-center justify-center py-32 text-center">
      <div className="w-12 h-12 rounded-full bg-muted flex items-center justify-center mb-4">
        <Icon size={20} className="text-muted-foreground" />
      </div>
      <h3 className="font-medium text-lg mb-1 tracking-tight">{title}</h3>
      <p className="text-xs text-muted-foreground max-w-xs mb-6 leading-relaxed">
        {description}
      </p>
      {action}
    </div>
  );
}

// ─── Main Page ────────────────────────────────────────────────────────────────
const ClassroomsContent = () => {
  const { user } = useAuth();

  const createClassForm = useForm<z.infer<typeof classroomSchema>>({
    resolver: zodResolver(classroomSchema),
    defaultValues: { name: "", description: "", bannerFile: "" },
  });

  const [isMounted, setIsMounted] = React.useState<boolean>(false);
  const [isOpenCreateClassroom, setIsOpenCreateClassroom] =
    React.useState<boolean>(false);
  const [isSubmitting, setIsSubmitting] = React.useState<boolean>(false);
  const [showDiscardDialog, setShowDiscardDialog] = React.useState<boolean>(false);

  const { mutateAsync: createClassroom } = useCreateClassroom();
  const {
    data: createdClassrooms,
    isLoading: isLoadingClassrooms,
    isError: isClassroomsError,
  } = useGetCreatedClassrooms();
  
  React.useEffect(() => {
    setIsMounted(true);
  }, []);

  const handleCreateClassroom = async () => {
    if (!(await createClassForm.trigger(['description', 'name']))) return;
    let isSuccess = false;
    try {
      setIsSubmitting(true);
      const values = createClassForm.getValues();
      const formData = new FormData();
      formData.append("userId", user?.userId as string);
      formData.append("name", values.name);
      formData.append("description", values.description);
      if (values.bannerFile) formData.append("bannerFile", values.bannerFile);
      await createClassroom(formData);
      isSuccess = true;
    } catch (err: any) {
      queryError(err);
    } finally {
      setIsSubmitting(false);
      if (isSuccess) {
        setIsOpenCreateClassroom(false);
        createClassForm.reset();
      }
    }
  };

  const handleCreateDialogOpenChange = (nextOpen: boolean) => {
    if (isSubmitting) return;

    if (!nextOpen && createClassForm.formState.isDirty) {
      setShowDiscardDialog(true);
      return;
    }

    setIsOpenCreateClassroom(nextOpen);
    if (!nextOpen) {
      createClassForm.reset();
    }
  };

  const handleDiscardChanges = () => {
    createClassForm.reset();
    setShowDiscardDialog(false);
    setIsOpenCreateClassroom(false);
  };

  if (!isMounted) return null;

  const ownedCount = createdClassrooms?.length ?? 0;

  return (
    <div className="flex-1 overflow-y-auto p-4 md:p-6 lg:p-8">
      <div className="w-full max-w-6xl mx-auto space-y-12 pb-20">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-3xl font-extrabold tracking-tight">Classrooms</h1>
            <p className="text-sm font-medium text-muted-foreground mt-1">Manage and monitor your active classes.</p>
          </div>
          <Button
            onClick={() => setIsOpenCreateClassroom(true)}
          >
            <Plus size={16} />
            New Classroom
          </Button>
        </div>

        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          {isLoadingClassrooms ? (
            Array.from({ length: 4 }).map((_, index) => (
              <motion.div
                key={index}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4, delay: index * 0.1 }}
              >
                <Card className="overflow-hidden border border-border bg-card shadow-none">
                  <div className="p-6 flex flex-col items-center space-y-6">
                    <div className="w-28 h-28 rounded-full animate-pulse bg-muted/30" />
                    <div className="space-y-2 w-full flex flex-col items-center">
                      <div className="h-4 w-2/3 animate-pulse rounded bg-muted/50" />
                      <div className="h-3 w-full animate-pulse rounded bg-muted/50" />
                    </div>
                  </div>
                </Card>
              </motion.div>
            ))
          ) : isClassroomsError ? (
            <EmptyState
              icon={BookOpen}
              title="Could not load classrooms"
              description="Please refresh and try again. If the problem continues, check your network connection."
            />
          ) : ownedCount === 0 ? (
            <EmptyState
              icon={BookOpen}
              title="No classrooms yet"
              description="Create your first classroom and start teaching or invite students."
              action={
                <Button onClick={() => setIsOpenCreateClassroom(true)} className="bg-indigo-600 text-white hover:bg-indigo-700 h-10 px-6 rounded-full font-bold gap-2 transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5">
                  <Plus size={18} />
                  Create Classroom
                </Button>
              }
            />
          ) : (
            createdClassrooms?.map((classroom: ClassroomData, i: number) => (
              <motion.div
                key={classroom.classId}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4, delay: i * 0.1 }}
              >
                <OwnedClassroomCard classroom={classroom} />
              </motion.div>
            ))
          )}
        </div>

        <Dialog
          open={isOpenCreateClassroom}
          onOpenChange={handleCreateDialogOpenChange}
        >
          <DialogContent className="max-h-[90vh] overflow-y-auto rounded-2xl sm:max-w-3xl">
            <DialogHeader>
              <div className="mb-2 inline-flex w-fit items-center gap-2 rounded-full border border-border/70 bg-muted/40 px-3 py-1 text-xs text-muted-foreground">
                <BookOpen size={14} />
                New Classroom
              </div>
              <DialogTitle className="text-xl md:text-2xl">Create a classroom</DialogTitle>
              <DialogDescription>
                Fill in the details below. You can always edit the classroom later.
              </DialogDescription>
            </DialogHeader>

            <ClassroomForm form={createClassForm} />

            <div className="rounded-xl border border-border/60 bg-muted/30 px-3 py-2 text-xs text-muted-foreground">
              Tip: Add a clear class name and a banner image so students can recognize this classroom quickly.
            </div>

            <DialogFooter className="gap-3 pt-4">
              <Button
                variant="ghost"
                onClick={() => handleCreateDialogOpenChange(false)}
                disabled={isSubmitting}
              >
                Cancel
              </Button>
              <Button
                className="bg-indigo-600 text-white hover:bg-indigo-700 px-6 rounded-full font-bold transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5"
                onClick={handleCreateClassroom}
                disabled={isSubmitting || !createClassForm.formState.isDirty}
              >
                {isSubmitting ? (
                  <span className="inline-flex items-center gap-2">
                    <Loader2 size={16} className="animate-spin" />
                    Creating...
                  </span>
                ) : (
                  "Create class"
                )}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        <AlertDialog open={showDiscardDialog} onOpenChange={setShowDiscardDialog}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Discard changes?</AlertDialogTitle>
              <AlertDialogDescription>
                You have unsaved classroom details. Closing now will remove your edits.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Keep editing</AlertDialogCancel>
              <AlertDialogAction onClick={handleDiscardChanges}>
                Discard changes
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </div>
  );
};

export default function Page() {
  return (
    <React.Suspense fallback={<div className="p-10 flex justify-center text-muted-foreground text-sm">Loading...</div>}>
      <ClassroomsContent />
    </React.Suspense>
  );
}
