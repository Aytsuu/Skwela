"use client";

import {
  Plus,
  BookOpen,
  Loader2,
  LucideIcon,
  ImagePlus,
} from "lucide-react";
import { useAuth } from "@/context/AuthContext";
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
import { queryError } from "@/helpers/errorDisplay";
import { slugFormat } from "@/helpers/urlFormatter";
import { motion, AnimatePresence } from "framer-motion";
import { toast } from "sonner";

// ─── Card Components ──────────────────────────────────────────────────────────
function CreateClassroomCard() {
  const { user } = useAuth();
  const { mutateAsync: createClassroom } = useCreateClassroom();
  const [isSubmitting, setIsSubmitting] = React.useState(false);
  const fileInputRef = React.useRef<HTMLInputElement>(null);

  const form = useForm<z.infer<typeof classroomSchema>>({
    resolver: zodResolver(classroomSchema),
    defaultValues: { name: "", description: "", bannerFile: "" },
  });

  const bannerFile = form.watch("bannerFile");
  const name = form.watch("name");
  const description = form.watch("description");

  const bannerPreview = React.useMemo(() => {
    if (!bannerFile) return null;
    if (typeof bannerFile === "string") return bannerFile;
    return URL.createObjectURL(bannerFile as File);
  }, [bannerFile]);

  const handleCreate = async () => {
    const isValid = await form.trigger(["name", "description"]);
    if (!isValid) return;

    try {
      setIsSubmitting(true);
      const values = form.getValues();
      const formData = new FormData();
      formData.append("userId", user?.userId as string);
      formData.append("name", values.name);
      formData.append("description", values.description);
      if (values.bannerFile) formData.append("bannerFile", values.bannerFile);

      await createClassroom(formData);
      form.reset({ name: "", description: "", bannerFile: "" });
      toast.success("Classroom created successfully!");
    } catch (err: any) {
      queryError(err);
    } finally {
      setIsSubmitting(false);
    }
  };

  const hasAnyInput = !!(name || description || bannerFile);

  return (
    <Card className="h-full overflow-hidden border-2 border-dashed border-muted-foreground/20 bg-muted/5 transition-all duration-300 hover:border-indigo-500/50 hover:bg-indigo-50/10">
      <div className="p-6 flex flex-col items-center text-center space-y-4">
        <div
          className="relative w-28 h-28 rounded-full overflow-hidden shadow-soft flex items-center justify-center bg-muted/30 cursor-pointer group/banner transition-all duration-300"
          onClick={() => fileInputRef.current?.click()}
        >
          {bannerPreview ? (
            <img
              src={bannerPreview}
              className="h-full w-full object-cover"
              alt="Banner preview"
            />
          ) : (
            <div className="flex flex-col items-center gap-1">
              <ImagePlus className="text-muted-foreground group-hover/banner:text-indigo-500 transition-colors" size={28} />
              <span className="text-[10px] font-medium text-muted-foreground">Banner</span>
            </div>
          )}
          <div className="absolute inset-0 bg-black/40 opacity-0 group-hover/banner:opacity-100 transition-opacity flex items-center justify-center">
            <p className="text-white text-[10px] font-bold">CHANGE</p>
          </div>
          <input
            type="file"
            ref={fileInputRef}
            className="hidden"
            accept="image/*"
            onChange={(e) => {
              const file = e.target.files?.[0];
              if (file) form.setValue("bannerFile", file, { shouldDirty: true });
            }}
          />
        </div>

        <div className="space-y-2 w-full">
          <input
            {...form.register("name")}
            placeholder="New Class Name"
            className="w-full bg-transparent text-center text-base font-bold tracking-tight focus:outline-none placeholder:text-muted-foreground/40 border-none shadow-none focus-visible:ring-0"
          />
          <textarea
            {...form.register("description")}
            placeholder="Class description (optional)"
            rows={2}
            className="w-full bg-transparent text-center text-xs text-muted-foreground font-medium leading-relaxed resize-none focus:outline-none placeholder:text-muted-foreground/30 border-none shadow-none focus-visible:ring-0"
          />
        </div>

        <AnimatePresence>
          {hasAnyInput && (
            <motion.div
              initial={{ opacity: 0, height: 0, marginTop: 0 }}
              animate={{ opacity: 1, height: "auto", marginTop: 16 }}
              exit={{ opacity: 0, height: 0, marginTop: 0 }}
              className="w-full overflow-hidden"
            >
              <Button
                size="sm"
                onClick={handleCreate}
                disabled={isSubmitting || !name}
                className="w-full bg-indigo-600 hover:bg-indigo-700 text-white rounded-full font-bold transition-all h-9 gap-2 shadow-sm hover:shadow-md"
              >
                {isSubmitting ? (
                  <Loader2 className="animate-spin" size={14} />
                ) : (
                  <>
                    <Plus size={14} />
                    CREATE CLASS
                  </>
                )}
              </Button>
            </motion.div>
          )}
        </AnimatePresence>
      </div>
    </Card>
  );
}

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
  const [isMounted, setIsMounted] = React.useState<boolean>(false);
  const {
    data: createdClassrooms,
    isLoading: isLoadingClassrooms,
    isError: isClassroomsError,
  } = useGetCreatedClassrooms();
  
  React.useEffect(() => {
    setIsMounted(true);
  }, []);

  if (!isMounted) return null;

  return (
    <div className="flex-1 overflow-y-auto p-4 md:p-6 lg:p-8">
      <div className="w-full max-w-6xl mx-auto space-y-12 pb-20">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-3xl font-extrabold tracking-tight">Classrooms</h1>
            <p className="text-sm font-medium text-muted-foreground mt-1">Manage and monitor your active classes.</p>
          </div>
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
          ) : (
            <>
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4 }}
              >
                <CreateClassroomCard />
              </motion.div>
              {createdClassrooms?.map((classroom: ClassroomData, i: number) => (
                <motion.div
                  key={classroom.classId}
                  initial={{ opacity: 0, y: 10 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.4, delay: (i + 1) * 0.1 }}
                >
                  <OwnedClassroomCard classroom={classroom} />
                </motion.div>
              ))}
            </>
          )}
        </div>
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
