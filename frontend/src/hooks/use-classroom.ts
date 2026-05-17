import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ClassroomService } from "../services/classroom.service";
import { ClassroomData, StudentData } from "../types/classroom";
import { useAuth } from "@/context/AuthContext";

export const useCreateClassroom = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();
  return useMutation({
    mutationFn: ClassroomService.create,
    onSuccess: (data: ClassroomData) => {
      queryClient.setQueryData(
        ["createdClassrooms", user?.userId],
        (old: ClassroomData[] = []) => [...old, data],
      );
      queryClient.invalidateQueries({
        queryKey: ["classroomsByCreator", user?.userId],
      });
    },
  });
};

export const useGetCreatedClassrooms = () => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["createdClassrooms", user?.userId],
    queryFn: ClassroomService.getCreatedClassrooms,
    staleTime: 5000,
    enabled: !!user?.userId,
    retry: false,
  });
};

export const useGetClassroomData = (classId: string, userId: string) => {
  return useQuery({
    queryKey: ["classroomData", classId, userId],
    queryFn: () => ClassroomService.getData(classId),
    staleTime: 5000,
    enabled: !!classId && !!userId,
    retry: false
  });
};

export const useDeleteClassroom = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();
  return useMutation({
    mutationFn: ClassroomService.delete,
    onSuccess: (_, classId) => {
      queryClient.setQueryData(
        ["classroomsByCreator", user?.userId],
        (old: ClassroomData[]) =>
          old.filter((prev) => prev.classId !== classId),
      );
      
      queryClient.invalidateQueries({
        queryKey: ["classroomsByCreator", user?.userId],
      });
    },
  });
};

export const useUpdateClassroom = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();
  return useMutation({
    mutationFn: ({ classId, data }: { classId: string; data: FormData }) =>
      ClassroomService.update(classId, data),
    onSuccess: (data: ClassroomData, variables) => {
      const { classId } = variables;
      queryClient.setQueryData(["classroomData", classId, user?.userId], (old: ClassroomData) => ({
        ...old,
        ...data
      }));
    }
  });
};

const updateStudentCollection = (
  previous: ClassroomData | undefined,
  updater: (students: StudentData[]) => StudentData[],
) => {
  if (!previous) {
    return previous;
  }

  return {
    ...previous,
    students: updater(previous.students ?? []),
  };
};

export const useCreateStudent = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();

  return useMutation({
    mutationFn: ({ classId, payload }: { classId: string; payload: Omit<StudentData, "studentId"> }) =>
      ClassroomService.createStudent(classId, payload),
    onSuccess: (student, { classId }) => {
      queryClient.setQueryData(["classroomData", classId, user?.userId], (old: ClassroomData | undefined) =>
        updateStudentCollection(old, (students) => [...students, student]),
      );
    },
  });
};

export const useImportStudents = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();

  return useMutation({
    mutationFn: ({ classId, data }: { classId: string; data: FormData }) =>
      ClassroomService.importStudents(classId, data),
    onSuccess: (students, { classId }) => {
      queryClient.setQueryData(["classroomData", classId, user?.userId], (old: ClassroomData | undefined) =>
        updateStudentCollection(old, (currentStudents) => [...currentStudents, ...students]),
      );
    },
  });
};

export const useUpdateStudent = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();

  return useMutation({
    mutationFn: ({
      classId,
      studentId,
      payload,
    }: {
      classId: string;
      studentId: string;
      payload: Omit<StudentData, "studentId">;
    }) => ClassroomService.updateStudent(classId, studentId, payload),
    onSuccess: (student, { classId, studentId }) => {
      queryClient.setQueryData(["classroomData", classId, user?.userId], (old: ClassroomData | undefined) =>
        updateStudentCollection(old, (students) =>
          students.map((currentStudent) =>
            currentStudent.studentId === studentId ? student : currentStudent,
          ),
        ),
      );
    },
  });
};

export const useDeleteStudent = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();

  return useMutation({
    mutationFn: ({ classId, studentId }: { classId: string; studentId: string }) =>
      ClassroomService.deleteStudent(classId, studentId),
    onSuccess: (_, { classId, studentId }) => {
      queryClient.setQueryData(["classroomData", classId, user?.userId], (old: ClassroomData | undefined) =>
        updateStudentCollection(old, (students) =>
          students.filter((student) => student.studentId !== studentId),
        ),
      );
    },
  });
};
