import { ClassroomCreateRequest } from "../types/classroom";
import { api } from "./api.service";

export const ClassroomService = {
  create: async (data: ClassroomCreateRequest) => {
    try {
      const res = await api.post('api/classroom/create', data);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  getByUserId: async (userId: string) => {
    try {
      const res = await api.get(`api/classroom/get/${userId}`)
      return res.data;
    } catch (err) {
      throw err;
    }
  }
}