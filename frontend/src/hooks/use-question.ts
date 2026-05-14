import { QuestionService } from "@/services/question.service"
import { useQuery } from "@tanstack/react-query"

export const useGetAssessmentQuestions = (assId: string) => {
  return useQuery({
    queryKey: ["assessmentQstns", assId],
    queryFn: () => QuestionService.list(assId),
    staleTime: 5000,
    enabled: !!assId,
    retry: false
  })
}