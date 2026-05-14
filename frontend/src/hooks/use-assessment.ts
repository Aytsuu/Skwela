import { AssessmentService } from "@/services/assessment.service";
import { Assessment } from "@/types/assessment";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

export const useCreateAssessment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: AssessmentService.create,
    onSuccess: (output, input) => {
      // Optimistic update
      const classId = input.get("classId");
      
      queryClient.setQueryData(["assessments", classId], (old: Assessment[] = []) => [
        ...old,
        ...input
      ]);

      queryClient.invalidateQueries({ queryKey: ['assessments', classId] });
    }
  })
}

export const useAssessmentList = (classId: string) => {
  return useQuery({
    queryKey: ["assessments", classId],
    queryFn: () => AssessmentService.list(classId),
    staleTime: 5000,
    enabled: !!classId,
    retry: false
  })
}