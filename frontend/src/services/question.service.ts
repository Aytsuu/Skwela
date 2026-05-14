import { Question } from "@/types/question";
import { api } from "./api.service";
import { queryError } from "@/helpers/errorDisplay";

export const QuestionService = {
  list: async (assId: string) => {
    try {
      const res = await api.get<Question[]>("api/question/get", {
        params: {
          assId
        }
      });
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  }
}