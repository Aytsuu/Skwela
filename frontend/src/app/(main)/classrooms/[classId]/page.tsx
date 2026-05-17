"use client";

import { useRouter, useSearchParams } from "next/navigation";
import {
  useCreateStudent,
  useDeleteClassroom,
  useDeleteStudent,
  useGetClassroomData,
  useImportStudents,
  useUpdateClassroom,
  useUpdateStudent,
} from "@/hooks/use-classroom";
import { Button } from "@/components/ui/button";
import React from "react";
import Protected from "@/app/(main)/protected";
import { Badge } from "@/components/ui/badge";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
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
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  MoreVertical,
  Trash2,
  BookOpen,
  CalendarDays,
  Pen,
  UploadCloud,
  File as FileIcon,
  X,
  Loader2,
  ClipboardList,
  ChevronUp,
  ChevronDown,
  ArrowLeft,
  Sparkles,
  Plus,
  Users,
  UserPlus,
  FileSpreadsheet,
  PencilLine,
} from "lucide-react";
import { formatDate } from "@/helpers/dateFormatter";
import { toast } from "sonner";
import { useForm } from "react-hook-form";
import z from "zod";
import { classroomSchema } from "@/schemas/classroom.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import ClassroomForm from "../ClassroomForm";
import { useAssessmentList, useCreateAssessment } from "@/hooks/use-assessment";
import { useGetAssessmentQuestions } from "@/hooks/use-question";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { queryError } from "@/helpers/errorDisplay";
import { Question } from "@/types/question";
import { Separator } from "@/components/ui/separator";
import { motion } from "framer-motion";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { StudentData } from "@/types/classroom";

type OrganizedSection = {
  type: string;
  questions: Question[];
};

const QUESTION_TYPE_LABELS: Record<string, string> = {
  fill_blank: "FILL IN THE BLANK",
  hand_tracing: "HAND TRACING",
  matching: "MATCHING",
  mcq: "MULTIPLE CHOICES",
  problem_solving: "PROBLEM SOLVING",
  true_false: "TRUE OR FALSE",
  essay: "ESSAY",
};

const studentSchema = z.object({
  fname: z.string().trim().min(1, "First name is required"),
  mname: z.string().trim().optional(),
  lname: z.string().trim().min(1, "Last name is required"),
});

type StudentFormValues = z.infer<typeof studentSchema>;

const formatQuestionTypeLabel = (type: string) => {
  return QUESTION_TYPE_LABELS[type] ?? type.replace(/_/g, " ").toUpperCase();
};

const formatInlineCodeSnippet = (source: string) => {
  const indentStep = "    ";
  const inlineBlockKeywords = new Set(["else", "catch", "finally"]);
  const lines: string[] = [];
  let currentLine = "";
  let depth = 0;
  let parenDepth = 0;
  let stringDelimiter: '"' | "'" | "`" | null = null;
  let isEscaping = false;

  const buildIndent = (level: number) => indentStep.repeat(Math.max(0, level));
  const flushCurrentLine = () => {
    const trimmed = currentLine.trim();

    if (!trimmed) {
      currentLine = "";
      return;
    }

    lines.push(buildIndent(depth) + trimmed);
    currentLine = "";
  };

  const getNextWord = (text: string, startIndex: number) => {
    let index = startIndex;
    while (index < text.length && /\s/.test(text[index])) {
      index += 1;
    }

    const match = text.slice(index).match(/^[A-Za-z_]+/);

    return match?.[0] ?? "";
  };

  for (let index = 0; index < source.length; index += 1) {
    const char = source[index];

    if (stringDelimiter) {
      currentLine += char;

      if (isEscaping) {
        isEscaping = false;
        continue;
      }

      if (char === "\\") {
        isEscaping = true;
        continue;
      }

      if (char === stringDelimiter) {
        stringDelimiter = null;
      }

      continue;
    }

    if (char === '"' || char === "'" || char === "`") {
      stringDelimiter = char;
      currentLine += char;
      continue;
    }

    if (char === "(") {
      parenDepth += 1;
      currentLine += char;
      continue;
    }

    if (char === ")") {
      parenDepth = Math.max(0, parenDepth - 1);
      currentLine += char;
      continue;
    }

    if (char === "{") {
      currentLine += char;
      flushCurrentLine();
      depth += 1;
      continue;
    }

    if (char === "}") {
      flushCurrentLine();
      depth = Math.max(0, depth - 1);
      currentLine = "}";

      const nextWord = getNextWord(source, index + 1);
      if (inlineBlockKeywords.has(nextWord)) {
        currentLine += " ";
      } else {
        flushCurrentLine();
      }

      continue;
    }

    if (char === ";" && parenDepth === 0) {
      currentLine += char;
      flushCurrentLine();
      continue;
    }

    if (char === "\n" || char === "\r") {
      flushCurrentLine();
      continue;
    }

    currentLine += char;
  }

  flushCurrentLine();

  return lines.join("\n").trim();
};

const isLikelyCode = (text: string) => {
  const cleaned = text.trim().replace(/^"(.*)"$/, "$1").replace(/\\n/g, "\n").trim();
  return ((cleaned.match(/;/g) || []).length > 1) || cleaned.includes("{");
};

const formatAssessmentText = (text: string) => {
  if (!text) return "";

  // Basic cleaning
  let cleaned = text.trim().replace(/^"(.*)"$/, "$1").replace(/\\n/g, "\n").trim();

  if (isLikelyCode(text)) {
    cleaned = formatInlineCodeSnippet(cleaned);
  }

  return cleaned;
};

const getConfidenceMetadata = (confidence: string | number) => {
  const score = typeof confidence === "string" ? parseFloat(confidence) : confidence;
  
  if (score >= 0.9) {
    return {
      label: "Highly Reliable",
      className: "bg-emerald-100 text-emerald-700 border-emerald-200",
      iconColor: "text-emerald-500"
    };
  }
  if (score >= 0.7) {
    return {
      label: "Likely Accurate",
      className: "bg-amber-100 text-amber-700 border-amber-200",
      iconColor: "text-amber-500"
    };
  }
  return {
    label: "Review Suggested",
    className: "bg-rose-100 text-rose-700 border-rose-200",
    iconColor: "text-rose-500"
  };
};

const RubricDisplay = ({ rubric }: { rubric: string }) => {
  // Clean the rubric string from enclosing double quotes and trim
  const cleanRubric = rubric.trim().replace(/^"(.*)"$/, '$1').trim();
  
  // Try to parse the rubric string into structured data
  // Expected format: "Criterion 1 (X pts), Criterion 2 (Y pts), ..."
  // Splitting by comma but avoiding commas within parentheses if possible
  const criteria = cleanRubric.split(/,(?![^(]*\))/).map(item => {
    const trimmed = item.trim().replace(/\.$/, '');
    const match = trimmed.match(/^(.*?)\s*\((.*?)\)$/);
    if (match) {
      return { description: match[1], points: match[2] };
    }
    return { description: trimmed, points: "" };
  }).filter(c => c.description !== "");

  // If it doesn't look like a structured list, just show it as text
  if (criteria.length <= 1 && (criteria[0]?.points === "" || criteria[0]?.points === undefined)) {
    return (
      <div className="mt-3 rounded-lg border border-border/70 p-4 bg-background">
        <p className="mb-2 text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
          Rubric
        </p>
        <p className="text-sm whitespace-pre-wrap leading-relaxed italic">{cleanRubric}</p>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-xl border border-border/70 bg-background shadow-sm">
      <div className="bg-muted/30 px-4 py-2 border-b border-border/50">
        <p className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
          Grading Rubric
        </p>
      </div>
      <div className="overflow-x-auto">
        <table className="w-full text-xs text-left border-collapse">
          <thead className="bg-muted/10 text-[10px] uppercase tracking-wider text-muted-foreground/70">
            <tr>
              <th className="px-4 py-2.5 font-bold border-b border-border/40">Criterion</th>
              <th className="px-4 py-2.5 font-bold border-b border-border/40 text-right w-24">Weight</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border/40">
            {criteria.map((c, i) => (
              <tr key={i} className="hover:bg-muted/5 transition-colors">
                <td className="px-4 py-3 leading-relaxed text-foreground/90 font-medium">{c.description}</td>
                <td className="px-4 py-3 text-right whitespace-nowrap align-top">
                  {c.points && (
                    <Badge variant="outline" className="text-[10px] font-bold px-1.5 h-5 bg-muted/20 border-border/50">
                      {c.points}
                    </Badge>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

const PageComponent = () => {
  // Hooks & States
  const { user } = useAuth();
  const router = useRouter();
  const urlParams = useSearchParams();
  const classId = urlParams.get("id") as string;

  const form = useForm<z.infer<typeof classroomSchema>>({
    resolver: zodResolver(classroomSchema),
    defaultValues: { name: "", description: "" },
  });

  const [isMounted, setIsMounted] = React.useState<boolean>(false);
  const [showClassroomDialog, setShowClassroomDialog] = React.useState<boolean>(false);
  const [isUpdatingClassroom, setIsUpdatingClassroom] = React.useState<boolean>(false);
  const [showDeleteDialog, setShowDeleteDialog] = React.useState<boolean>(false);
  const [showStudentDialog, setShowStudentDialog] = React.useState<boolean>(false);
  const [showStudentDeleteDialog, setShowStudentDeleteDialog] = React.useState<boolean>(false);
  const [showImportStudentsDialog, setShowImportStudentsDialog] = React.useState<boolean>(false);
  const [files, setFiles] = React.useState<File[]>([]);
  const [csvFile, setCsvFile] = React.useState<File | null>(null);
  const [isDragging, setIsDragging] = React.useState<boolean>(false);
  const [isCreatingAssessment, setIsCreatingAssessment] = React.useState<boolean>(false);
  const [isSavingStudent, setIsSavingStudent] = React.useState<boolean>(false);
  const [isImportingStudents, setIsImportingStudents] = React.useState<boolean>(false);
  const [showUploadModal, setShowUploadModal] = React.useState<boolean>(false);
  const [editingStudent, setEditingStudent] = React.useState<StudentData | null>(null);
  const [selectedStudent, setSelectedStudent] = React.useState<StudentData | null>(null);

  const [selectedAssessmentId, setSelectedAssessmentId] = React.useState<string | null>(null);
  const [selectedAssessmentTitle, setSelectedAssessmentTitle] = React.useState<string | null>(null);
  const [collapsedSections, setCollapsedSections] = React.useState<Record<string, boolean>>({});

  const { mutateAsync: deleteClassroom } = useDeleteClassroom();
  const { mutateAsync: updateClassroom } = useUpdateClassroom();
  const { mutateAsync: createStudent } = useCreateStudent();
  const { mutateAsync: updateStudent } = useUpdateStudent();
  const { mutateAsync: deleteStudent } = useDeleteStudent();
  const { mutateAsync: importStudents } = useImportStudents();
  const { mutateAsync: createAssessment } = useCreateAssessment();
  const { data: classroomData, isLoading: isLoadingClassroomData, error } = useGetClassroomData(
    classId,
    user?.userId ?? "",
  );
  const { data: assessments, isLoading: isLoadingAssessments } = useAssessmentList(classId);
  const {
    data: assessmentQstns,
    isLoading: isLoadingQuestions,
  } = useGetAssessmentQuestions(selectedAssessmentId ?? "");

  // Flags
  const hasUpdate = form.formState.isDirty;

  const studentForm = useForm<StudentFormValues>({
    resolver: zodResolver(studentSchema),
    defaultValues: {
      fname: "",
      mname: "",
      lname: "",
    },
  });

  const organizedQuestions = React.useMemo<OrganizedSection[]>(() => {
    if (!assessmentQstns || assessmentQstns.length === 0) return [];

    const grouped = assessmentQstns.reduce<Record<string, Question[]>>(
      (acc, curr) => {
        if (!acc[curr.type]) {
          acc[curr.type] = [];
        }
        acc[curr.type].push(curr);
        return acc;
      },
      {},
    );

    return Object.entries(grouped)
      .map(([type, questions]) => ({
        type,
        questions: [...questions].sort((a, b) => Number(a.num) - Number(b.num)),
      }))
      .sort((a, b) => a.type.localeCompare(b.type));
  }, [assessmentQstns]);

  // Effects
  React.useEffect(() => {
    setIsMounted(true);
  }, []);

  React.useEffect(() => {
    if (organizedQuestions.length === 0) {
      setCollapsedSections({});
      return;
    }

    setCollapsedSections((prev) => {
      const next: Record<string, boolean> = {};
      organizedQuestions.forEach((section) => {
        next[section.type] = prev[section.type] ?? true;
      });
      return next;
    });
  }, [organizedQuestions]);

  const toggleSection = (type: string) => {
    setCollapsedSections((prev) => ({
      ...prev,
      [type]: !prev[type],
    }));
  };

  React.useEffect(() => {
    if (!classId) {
      toast.error("Page not found");
      router.replace("/classrooms");
    }
  }, [isMounted, classId, router]);

  React.useEffect(() => {
    if (!classroomData) return;

    form.reset({
      name: classroomData.className,
      description: classroomData.classDescription,
      bannerFile: `${process.env.NEXT_PUBLIC_FILE_BUCKET}/${classroomData.classBanner}`,
    }, { keepDirty: false });
  }, [classroomData, form, showClassroomDialog]);

  React.useEffect(() => {
    if (!showStudentDialog) {
      studentForm.reset({
        fname: "",
        mname: "",
        lname: "",
      });
      setEditingStudent(null);
      return;
    }

    if (editingStudent) {
      studentForm.reset({
        fname: editingStudent.fname,
        mname: editingStudent.mname,
        lname: editingStudent.lname,
      });
    }
  }, [editingStudent, showStudentDialog, studentForm]);

  // Handlers
  const handleUpdateClassroom = async () => {
    if (!hasUpdate) {
      toast.info("No changes were made");
      setIsUpdatingClassroom(false);
      return;
    }

    try {
      setIsUpdatingClassroom(true);
      const updatedFields = form.formState.dirtyFields;
      const formData = new FormData();
      const values = form.getValues();

      Object.entries(values).forEach(([key, value]) => {
        if (updatedFields[key as keyof typeof updatedFields] && value !== undefined) {
          formData.append(key, value);
        }
      });

      await updateClassroom({ classId, data: formData });
      setShowClassroomDialog(false);
      toast.success("Successfully updated classroom");
    } catch (err: unknown) {
      queryError(err as import("axios").AxiosError);
    } finally {
      setIsUpdatingClassroom(false);
    }
  };

  const handleRemoveClassroom = async () => {
    try {
      await deleteClassroom(classId);
      router.replace("/classrooms");
    } catch (err: unknown) {
      queryError(err as import("axios").AxiosError);
    }
  };

  const handleOpenCreateStudentDialog = () => {
    setEditingStudent(null);
    setShowStudentDialog(true);
  };

  const handleOpenUpdateStudentDialog = (student: StudentData) => {
    setEditingStudent(student);
    setShowStudentDialog(true);
  };

  const handleSaveStudent = studentForm.handleSubmit(async (values) => {
    try {
      setIsSavingStudent(true);
      const payload = {
        fname: values.fname.trim(),
        mname: values.mname?.trim() ?? "",
        lname: values.lname.trim(),
      };

      if (editingStudent) {
        await updateStudent({
          classId,
          studentId: editingStudent.studentId,
          payload,
        });
        toast.success("Student updated");
      } else {
        await createStudent({
          classId,
          payload,
        });
        toast.success("Student added");
      }

      setShowStudentDialog(false);
    } catch (err: unknown) {
      queryError(err as import("axios").AxiosError);
    } finally {
      setIsSavingStudent(false);
    }
  });

  const handleRemoveStudent = async () => {
    if (!selectedStudent) {
      return;
    }

    try {
      await deleteStudent({
        classId,
        studentId: selectedStudent.studentId,
      });
      setShowStudentDeleteDialog(false);
      setSelectedStudent(null);
      toast.success("Student removed");
    } catch (err: unknown) {
      queryError(err as import("axios").AxiosError);
    }
  };

  const handleImportStudents = async () => {
    if (!csvFile) {
      toast.info("Select a CSV file first");
      return;
    }

    try {
      setIsImportingStudents(true);
      const formData = new FormData();
      formData.append("file", csvFile);
      await importStudents({ classId, data: formData });
      setCsvFile(null);
      setShowImportStudentsDialog(false);
      toast.success("Students imported");
    } catch (err: unknown) {
      queryError(err as import("axios").AxiosError);
    } finally {
      setIsImportingStudents(false);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      const droppedFiles = Array.from(e.dataTransfer.files).filter(
        (file) => file.type === "application/pdf"
      );
      if (droppedFiles.length > 0) {
        setFiles((prev) => {
          const existing = new Set(prev.map((file) => `${file.name}-${file.size}`));
          const next = droppedFiles.filter((file) => !existing.has(`${file.name}-${file.size}`));
          return [...prev, ...next];
        });
      } else {
        toast.error("Only PDF files are allowed");
      }
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const selectedFiles = Array.from(e.target.files).filter(
        (file) => file.type === "application/pdf"
      );
      if (selectedFiles.length > 0) {
        setFiles((prev) => {
          const existing = new Set(prev.map((file) => `${file.name}-${file.size}`));
          const next = selectedFiles.filter((file) => !existing.has(`${file.name}-${file.size}`));
          return [...prev, ...next];
        });
      } else {
        toast.error("Only PDF files are allowed");
      }
    }
  };

  const handleRemoveFile = (index: number) => {
    setFiles((prev) => prev.filter((_, i) => i !== index));
  };

  const handleCreate = async () => {
    if (files.length === 0) {
      toast.info("Add at least one PDF file first");
      return;
    }

    try {
      setIsCreatingAssessment(true);
      const formData = new FormData();
      formData.append('classId', classId);
      files.forEach((file) => formData.append('files', file));
      await createAssessment(formData);
      setFiles([]);
      setShowUploadModal(false);
      toast.success("Assessment created");
    } catch (err: unknown) {
      queryError(err as import("axios").AxiosError);
    } finally {
      setIsCreatingAssessment(false);
    }
  }

  if (!isMounted || !classId) return null;

  const data = classroomData;
  const bannerUrl = data?.classBanner
    ? data.classBanner.startsWith("http")
      ? data.classBanner
      : `${process.env.NEXT_PUBLIC_FILE_BUCKET}/${data.classBanner}`
    : undefined;

  return (
    <Protected error={error}>
      <div className="flex-1 flex flex-col min-h-0">
        {/* ── Three-column layout ── */}
        <div className="flex flex-1 min-h-0 overflow-hidden w-full">
          {/* Left Sidebar: Assessments List */}
          <div className="w-72 flex-shrink-0 border-r border-border bg-muted/10 flex flex-col">
            <div className="p-4 border-b border-border space-y-4">
              <Link
                href="/classrooms"
                className="flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-primary transition-colors group"
              >
                <ArrowLeft size={14} className="group-hover:-translate-x-1 transition-transform" />
                BACK TO CLASSROOMS
              </Link>
              <div className="flex items-center justify-between">
                <h2 className="font-bold text-[10px] uppercase tracking-widest text-muted-foreground/80 flex items-center gap-2">
                  <ClipboardList size={12} />
                  Assessments
                </h2>
                <div className="flex items-center gap-1">
                  {assessments && assessments.length > 0 && (
                    <Badge variant="outline" className="text-[10px] h-5 px-1.5 opacity-60">
                      {assessments.length}
                    </Badge>
                  )}
                  <Button
                    size="icon"
                    variant="ghost"
                    className="h-6 w-6 text-muted-foreground hover:text-primary transition-colors"
                    onClick={() => setShowUploadModal(true)}
                  >
                    <Plus size={14} />
                  </Button>
                </div>
              </div>
            </div>
            <div className="flex-1 overflow-y-auto p-4 space-y-2">
              {isLoadingAssessments ? (
                <div className="py-8 text-center text-sm text-muted-foreground flex flex-col items-center gap-2">
                  <Loader2 size={16} className="animate-spin" />
                  Loading...
                </div>
              ) : assessments && assessments.length > 0 ? (
                assessments.map((assessment, i) => (
                  <motion.div
                    key={assessment.id}
                    initial={{ opacity: 0, x: -10 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ duration: 0.3, delay: i * 0.05 }}
                  >
                    <div
                      onClick={() => {
                        setSelectedAssessmentId(assessment.id);
                        setSelectedAssessmentTitle(assessment.title);
                      }}
                      className={`group p-3 rounded-xl border transition-all cursor-pointer shadow-sm ${
                        selectedAssessmentId === assessment.id 
                          ? "border-primary bg-primary/5 ring-1 ring-primary/10" 
                          : "border-border/50 bg-card hover:border-primary/30 hover:bg-muted/50"
                      }`}
                    >
                      <div className="flex flex-col gap-2">
                        <div className="flex items-start justify-between gap-2">
                          <h3 className={`text-xs font-bold leading-tight line-clamp-2 transition-colors ${
                            selectedAssessmentId === assessment.id ? "text-primary" : "text-foreground"
                          }`}>
                            {assessment.title}
                          </h3>
                        </div>
                        
                        <div className="flex items-center justify-between text-[10px] text-muted-foreground font-medium">
                          <div className="flex items-center gap-1 opacity-70">
                            <ClipboardList size={10} />
                            <span className="uppercase tracking-wider">{assessment.type}</span>
                          </div>
                          <span className="opacity-60">{formatDate(assessment.createdAt)}</span>
                        </div>
                      </div>
                    </div>
                  </motion.div>
                ))
              ) : (
                <div className="py-8 text-center border border-dashed rounded-xl bg-card">
                  <div className="mx-auto mb-2 flex h-8 w-8 items-center justify-center rounded-full bg-muted">
                    <ClipboardList size={14} className="text-muted-foreground" />
                  </div>
                  <p className="text-xs font-medium">No assessments</p>
                  <p className="mt-1 text-[10px] text-muted-foreground px-2">
                    Click the plus icon to upload PDFs.
                  </p>
                </div>
              )}
            </div>
          </div>

          {/* Center: Dynamic screen for assessment details */}
          <div className="flex-1 flex flex-col min-h-0 bg-background">
            {!selectedAssessmentId ? (
              <div className="flex-1 flex flex-col items-center justify-center p-6 relative">
                <div className="text-center space-y-4 max-w-sm">
                  <div className="mx-auto w-16 h-16 rounded-2xl bg-muted/50 flex items-center justify-center">
                    <FileIcon className="h-8 w-8 text-muted-foreground/50" />
                  </div>
                  <div>
                    <h3 className="text-lg font-medium text-foreground">Select an assessment</h3>
                    <p className="text-sm text-muted-foreground mt-1">
                      Choose an assessment from the left sidebar to view its details, or upload new materials to generate one.
                    </p>
                  </div>
                </div>
              </div>
            ) : isLoadingQuestions ? (
              <div className="flex-1 flex flex-col items-center justify-center p-6">
                <Loader2 size={32} className="animate-spin text-primary/40" />
                <p className="mt-4 text-sm text-muted-foreground font-medium">Loading assessment details...</p>
              </div>
            ) : organizedQuestions.length === 0 ? (
              <div className="flex-1 flex flex-col items-center justify-center p-6">
                <div className="mx-auto mb-3 flex h-10 w-10 items-center justify-center rounded-full bg-muted">
                  <ClipboardList size={18} className="text-muted-foreground" />
                </div>
                <p className="text-sm font-medium">No questions found</p>
                <p className="mt-1 text-xs text-muted-foreground">
                  This assessment does not have generated questions yet.
                </p>
              </div>
            ) : (
              <>
                {/* Fixed Header section */}
                <div className="px-6 py-4 border-b border-border bg-background z-10">
                  <div className="w-full flex items-center justify-between gap-4">
                    <header className="space-y-1">
                      <h1 className="text-lg font-semibold">
                        {selectedAssessmentTitle}
                      </h1>
                    </header>
                  </div>
                </div>

                {/* Scrollable Content section */}
                <div className="flex-1 overflow-y-auto p-6">
                  <div className="max-w-4xl mx-auto space-y-4">
                    {organizedQuestions.map((section, i) => (
                      <motion.div
                        key={section.type}
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.4, delay: i * 0.1 }}
                      >
                        <Card className="overflow-hidden border-border/60 shadow-sm">
                          <CardHeader className="border-b bg-muted/25 px-5 py-4">
                            <div className="flex flex-wrap items-center justify-between gap-2">
                              <CardTitle className="text-base font-semibold">{formatQuestionTypeLabel(section.type)}</CardTitle>
                              <div className="flex items-center gap-2">
                                <Badge className="font-bold">{section.questions.length} questions</Badge>
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => toggleSection(section.type)}
                                  className="h-8 gap-1 px-2 text-xs"
                                >
                                  {collapsedSections[section.type] ? (
                                    <>
                                      <ChevronDown size={14} />
                                      Expand
                                    </>
                                  ) : (
                                    <>
                                      <ChevronUp size={14} />
                                      Collapse
                                    </>
                                  )}
                                </Button>
                              </div>
                            </div>
                            <CardDescription className="text-xs">
                              Question set grouped by type for easier review.
                            </CardDescription>
                          </CardHeader>

                          {!collapsedSections[section.type] && (
                            <CardContent className="space-y-4 px-5 pb-5">
                              {/* Display section-level rubric if available */}
                              {section.questions.some(q => q.rubric && q.rubric.trim() !== "null") && (
                                <div className="mb-6">
                                  <RubricDisplay 
                                    rubric={section.questions.find(q => q.rubric && q.rubric.trim() !== "null")?.rubric || ""} 
                                  />
                                  <Separator className="mt-6 bg-border/40" />
                                </div>
                              )}

                              {section.questions.map((qstn) => (
                                <article
                                  key={qstn.id}
                                  className="rounded-xl border border-border/70 bg-card p-5 space-y-3"
                                >
                                  <div className="flex flex-wrap items-center gap-2">
                                    <Badge className="bg-primary/10 text-primary border-0 font-bold">#{qstn.num}</Badge>
                                    <Badge variant="outline" className="font-semibold">{qstn.maxPoints} pts</Badge>
                                    {qstn.aiConfidence && (
                                      <Badge 
                                        variant="secondary" 
                                        className={`gap-1 border-0 font-bold ${getConfidenceMetadata(qstn.aiConfidence).className}`}
                                      >
                                        <Sparkles size={12} className={getConfidenceMetadata(qstn.aiConfidence).iconColor} />
                                        {getConfidenceMetadata(qstn.aiConfidence).label}
                                      </Badge>
                                    )}
                                  </div>

                                  <div className={`text-sm leading-relaxed whitespace-pre-wrap ${
                                    isLikelyCode(qstn.text) ? "font-mono" : "font-medium"
                                  }`}>
                                    {formatAssessmentText(qstn.text)}
                                  </div>

                                  {qstn.answer && qstn.answer.trim() !== "null" && qstn.answer.trim() !== "" && (
                                    <div className="mt-4 rounded-lg bg-muted/50 p-4 border border-border/40">
                                      <p className="mb-2 text-[10px] font-bold uppercase tracking-widest text-primary/70">
                                        Suggested Answer
                                      </p>
                                      <div className="text-sm whitespace-pre-wrap leading-relaxed font-mono">
                                        {formatAssessmentText(qstn.answer)}
                                      </div>
                                    </div>
                                  )}
                                </article>
                              ))}
                            </CardContent>
                          )}
                        </Card>
                      </motion.div>
                    ))}
                  </div>
                </div>
              </>
            )}
          </div>

          {/* Right Sidebar: Classroom Meta Data */}
          <div className="w-80 flex-shrink-0 border-l border-border bg-muted/10 overflow-y-auto p-5">
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h2 className="font-semibold text-sm flex items-center gap-2">
                  <BookOpen size={16} />
                  Classroom Info
                </h2>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button size="icon" variant="ghost" className="h-8 w-8">
                      <MoreVertical size={14} />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end" className="w-44">
                    <DropdownMenuItem
                      className="gap-2 cursor-pointer"
                      onSelect={() => setShowClassroomDialog(true)}
                    >
                      <Pen size={14} />
                      Edit Details
                    </DropdownMenuItem>
                    <DropdownMenuItem
                      className="gap-2 cursor-pointer text-destructive focus:bg-destructive/10 focus:text-destructive"
                      onSelect={() => setShowDeleteDialog(true)}
                    >
                      <Trash2 size={14} />
                      Delete Classroom
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>

              {isLoadingClassroomData ? (
                <div className="py-8 text-center flex justify-center">
                  <Loader2 size={16} className="animate-spin text-muted-foreground" />
                </div>
              ) : data ? (
                <div className="space-y-5">
                  <div className="w-full aspect-video rounded-xl overflow-hidden border border-border relative">
                    {bannerUrl ? (
                      <img
                        src={bannerUrl}
                        alt="Class banner"
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <div className="w-full h-full bg-primary/10 flex items-center justify-center">
                        <BookOpen className="text-primary/40 h-8 w-8" />
                      </div>
                    )}
                  </div>
                  
                  <div className="space-y-2">
                    <h1 className="text-lg font-bold leading-tight">
                      {data.className}
                    </h1>
                    {data.classDescription && (
                      <p className="text-sm text-muted-foreground mt-2 leading-relaxed">
                        {data.classDescription}
                      </p>
                    )}
                  </div>
                  
                  <div className="pt-4 border-t border-border/60">
                    <p className="text-xs text-muted-foreground flex items-center gap-2 font-medium">
                      <CalendarDays size={14} />
                      Created {formatDate(data.classCreatedAt ?? "")}
                    </p>
                  </div>

                  <div className="pt-4 border-t border-border/60 space-y-4">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <h3 className="text-sm font-semibold flex items-center gap-2">
                          <Users size={15} />
                          Students
                        </h3>
                        <p className="mt-1 text-xs text-muted-foreground">
                          Manage the roster for this classroom.
                        </p>
                      </div>
                      <Badge variant="outline" className="text-[10px] h-5 px-1.5">
                        {data.students?.length ?? 0}
                      </Badge>
                    </div>

                    <div className="grid grid-cols-2 gap-2">
                      <Button
                        type="button"
                        variant="outline"
                        className="justify-start gap-2"
                        onClick={handleOpenCreateStudentDialog}
                      >
                        <UserPlus size={14} />
                        Add Student
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        className="justify-start gap-2"
                        onClick={() => setShowImportStudentsDialog(true)}
                      >
                        <FileSpreadsheet size={14} />
                        Import CSV
                      </Button>
                    </div>

                    <div className="space-y-2">
                      {data.students && data.students.length > 0 ? (
                        [...data.students]
                          .sort((left, right) => {
                            const leftName = `${left.lname} ${left.fname} ${left.mname}`.toLowerCase();
                            const rightName = `${right.lname} ${right.fname} ${right.mname}`.toLowerCase();
                            return leftName.localeCompare(rightName);
                          })
                          .map((student) => (
                            <div
                              key={student.studentId}
                              className="rounded-xl border border-border/60 bg-card p-3 shadow-sm"
                            >
                              <div className="flex items-start justify-between gap-2">
                                <div className="min-w-0">
                                  <p className="text-sm font-semibold leading-tight">
                                    {student.lname}, {student.fname}
                                  </p>
                                  {student.mname && (
                                    <p className="mt-1 text-xs text-muted-foreground">
                                      Middle name: {student.mname}
                                    </p>
                                  )}
                                </div>
                                <div className="flex items-center gap-1">
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8"
                                    onClick={() => handleOpenUpdateStudentDialog(student)}
                                  >
                                    <PencilLine size={14} />
                                  </Button>
                                  <Button
                                    type="button"
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 text-destructive hover:text-destructive"
                                    onClick={() => {
                                      setSelectedStudent(student);
                                      setShowStudentDeleteDialog(true);
                                    }}
                                  >
                                    <Trash2 size={14} />
                                  </Button>
                                </div>
                              </div>
                            </div>
                          ))
                      ) : (
                        <div className="rounded-xl border border-dashed border-border/60 bg-card px-4 py-6 text-center">
                          <p className="text-xs font-medium">No students yet</p>
                          <p className="mt-1 text-[11px] text-muted-foreground">
                            Add students individually or import a CSV with `fname,mname,lname` headers.
                          </p>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              ) : null}
            </div>
          </div>
        </div>

        {/* ── Upload Materials Dialog ── */}
        <Dialog open={showUploadModal} onOpenChange={setShowUploadModal}>
          <DialogContent className="sm:max-w-md rounded-2xl">
            <DialogHeader>
              <div className="mb-2 inline-flex w-fit items-center gap-2 rounded-full border border-border/70 bg-muted/40 px-3 py-1 text-xs text-muted-foreground">
                <UploadCloud size={14} />
                Upload Materials
              </div>
              <DialogTitle>Add new materials</DialogTitle>
              <DialogDescription>
                Upload PDF files to generate a new assessment for this classroom.
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-4">
              <label
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
                onDrop={handleDrop}
                className={`relative flex cursor-pointer flex-col items-center justify-center rounded-xl border-2 border-dashed p-8 text-center transition-colors ${
                  isDragging
                    ? "border-primary bg-primary/5"
                    : "border-border/60 bg-card hover:bg-muted/50"
                }`}
              >
                <input
                  type="file"
                  multiple
                  accept=".pdf"
                  onChange={handleFileChange}
                  className="absolute h-0 w-0 opacity-0"
                />
                <UploadCloud className="h-8 w-8 text-muted-foreground/50 mb-2" />
                <p className="text-sm font-medium">Drag files here or click to browse</p>
                <p className="text-xs text-muted-foreground mt-1">PDF files only</p>
              </label>

              {files.length > 0 && (
                <div className="space-y-3 rounded-xl border border-border/60 bg-muted/30 p-3">
                  <div className="max-h-48 space-y-2 overflow-y-auto pr-1">
                    {files.map((file, index) => (
                      <div
                        key={`${file.name}-${index}`}
                        className="flex items-center justify-between gap-2 rounded-lg border border-border/40 bg-card p-2"
                      >
                        <div className="flex items-center gap-2 overflow-hidden">
                          <FileIcon className="h-4 w-4 shrink-0 text-primary" />
                          <span className="truncate text-xs font-medium">{file.name}</span>
                        </div>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-6 w-6 shrink-0 text-muted-foreground hover:text-destructive"
                          onClick={() => handleRemoveFile(index)}
                        >
                          <X className="h-3 w-3" />
                        </Button>
                      </div>
                    ))}
                  </div>

                  <Button
                    className="w-full font-bold"
                    onClick={handleCreate}
                    disabled={isCreatingAssessment}
                  >
                    {isCreatingAssessment ? (
                      <span className="inline-flex items-center gap-2">
                        <Loader2 size={14} className="animate-spin" />
                        Creating...
                      </span>
                    ) : (
                      "Generate Assessment"
                    )}
                  </Button>
                </div>
              )}
            </div>
          </DialogContent>
        </Dialog>

        {/* ── Classroom Form Dialog ── */}
        <Dialog open={showClassroomDialog} onOpenChange={setShowClassroomDialog}>
          <DialogContent className="sm:max-w-2xl rounded-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <BookOpen size={18} />
                Update a classroom
              </DialogTitle>
              <DialogDescription>
                Change your classroom details and save your changes.
              </DialogDescription>
            </DialogHeader>

            <ClassroomForm form={form} />

            <DialogFooter className="gap-2">
              <Button
                variant="outline"
                onClick={() => setShowClassroomDialog(false)}
                disabled={isUpdatingClassroom}
              >
                Cancel
              </Button>
              <Button
                onClick={handleUpdateClassroom}
                disabled={isUpdatingClassroom}
              >
                {isUpdatingClassroom ? "Updating..." : "Update class"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        {/* ── Delete Dialog ── */}
        <Dialog open={showStudentDialog} onOpenChange={setShowStudentDialog}>
          <DialogContent className="sm:max-w-md rounded-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <Users size={18} />
                {editingStudent ? "Update student" : "Add student"}
              </DialogTitle>
              <DialogDescription>
                Capture the student name exactly as it should appear in classroom records.
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-2">
              <div className="space-y-2">
                <Label htmlFor="fname">First name</Label>
                <Input id="fname" {...studentForm.register("fname")} />
                {studentForm.formState.errors.fname && (
                  <p className="text-xs text-destructive">{studentForm.formState.errors.fname.message}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="mname">Middle name</Label>
                <Input id="mname" {...studentForm.register("mname")} />
              </div>

              <div className="space-y-2">
                <Label htmlFor="lname">Last name</Label>
                <Input id="lname" {...studentForm.register("lname")} />
                {studentForm.formState.errors.lname && (
                  <p className="text-xs text-destructive">{studentForm.formState.errors.lname.message}</p>
                )}
              </div>
            </div>

            <DialogFooter className="gap-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => setShowStudentDialog(false)}
                disabled={isSavingStudent}
              >
                Cancel
              </Button>
              <Button type="button" onClick={handleSaveStudent} disabled={isSavingStudent}>
                {isSavingStudent ? "Saving..." : editingStudent ? "Update student" : "Add student"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        <Dialog open={showImportStudentsDialog} onOpenChange={setShowImportStudentsDialog}>
          <DialogContent className="sm:max-w-md rounded-2xl">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <FileSpreadsheet size={18} />
                Import students from CSV
              </DialogTitle>
              <DialogDescription>
                Upload a `.csv` file with the headers `fname,mname,lname`. Middle name can be blank.
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-4 py-2">
              <div className="space-y-2">
                <Label htmlFor="studentsCsv">CSV file</Label>
                <Input
                  id="studentsCsv"
                  type="file"
                  accept=".csv,text/csv"
                  onChange={(event) => {
                    const nextFile = event.target.files?.[0] ?? null;
                    setCsvFile(nextFile);
                  }}
                />
              </div>
              <div className="rounded-lg border border-border/60 bg-muted/20 p-3 text-xs text-muted-foreground">
                Example row: `Juan,Cruz,Dela Cruz`
              </div>
            </div>

            <DialogFooter className="gap-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => setShowImportStudentsDialog(false)}
                disabled={isImportingStudents}
              >
                Cancel
              </Button>
              <Button type="button" onClick={handleImportStudents} disabled={isImportingStudents}>
                {isImportingStudents ? "Importing..." : "Import students"}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>

        <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
          <AlertDialogContent className="rounded-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle>Delete classroom?</AlertDialogTitle>
              <AlertDialogDescription>
                This is permanent. All posts and materials will be lost.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancel</AlertDialogCancel>
              <AlertDialogAction
                className="bg-destructive text-white hover:bg-destructive/90"
                onClick={handleRemoveClassroom}
              >
                Delete
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>

        <AlertDialog open={showStudentDeleteDialog} onOpenChange={setShowStudentDeleteDialog}>
          <AlertDialogContent className="rounded-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle>Delete student?</AlertDialogTitle>
              <AlertDialogDescription>
                {selectedStudent
                  ? `This removes ${selectedStudent.fname} ${selectedStudent.lname} from the classroom roster.`
                  : "This removes the selected student from the classroom roster."}
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancel</AlertDialogCancel>
              <AlertDialogAction
                className="bg-destructive text-white hover:bg-destructive/90"
                onClick={handleRemoveStudent}
              >
                Delete
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </Protected>
  );
};
export default PageComponent;
