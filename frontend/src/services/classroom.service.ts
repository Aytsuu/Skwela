import { AxiosError } from "axios";
import { ClassroomCreateRequest, ClassroomResponse } from "../types/classroom";
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
  },
  getData: async (classId: string, userId: string, role: string) => {
    try {
      const res = await api.get<ClassroomResponse>(`api/classroom/get/${classId}/${userId}/${role}`);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  delete: async (classId: string) => {
    try {
      const res = await api.delete(`api/classroom/delete/${classId}`);
      return res.data;
    } catch (err) {
      throw err;
    }
  }
}