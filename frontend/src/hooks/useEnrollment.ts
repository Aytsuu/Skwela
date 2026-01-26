import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { EnrollmentService } from "../services/enrollment.service"
import { useAuth } from "../components/context/AuthContext"

export const useCreateEnrollment = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();
  return useMutation({
    mutationFn: EnrollmentService.create,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['studentEnrolledClasses', user?.userId] });
      queryClient.setQueryData(['studentEnrolledClasses', user?.userId], (old: any) => [
        ...old,
        data
      ])
    }
  })
}

export const useGetEnrolledClasses = (userId: string, role: string) => {
  return useQuery({
    queryKey: ['studentEnrolledClasses', userId],
    queryFn: () => EnrollmentService.getEnrolledClasses(userId),
    staleTime: 5000,
    enabled: !!userId && role === 'student',
    retry: false
  })
}

export const useUpdateStatus = () => {
  const queryClient = useQueryClient();
  const { user } = useAuth();
  return useMutation({
    mutationFn: EnrollmentService.updateStatus,
    onSuccess: (data, variables) => {
      const { classId } = variables;
      queryClient.invalidateQueries({ queryKey: ['studentEnrolledClasses', user?.userId] });
      queryClient.setQueryData(['studentEnrolledClasses', user?.userId], (old: any) => {
        return old.filter((enrollment: any) => enrollment.class_id !== classId)
      })
    }
  })
}