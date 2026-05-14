import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ClassroomService } from "../services/classroom.service";
import { ClassroomData } from "../types/classroom";
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
