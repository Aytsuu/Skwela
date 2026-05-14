import { queryError } from "@/helpers/errorDisplay";
import { ClassroomData } from "../types/classroom";
import { api } from "./api.service";

export const ClassroomService = {
  create: async (data: FormData) => {
    try {
      const res = await api.post<ClassroomData>('api/classroom/create', data);
      return res.data;
    } catch (err) {
      throw err;
    }
  },
  getCreatedClassrooms: async () => {
    try {
      const res = await api.get<ClassroomData[]>(`api/classroom/get`)
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  getData: async (classId: string) => {
    try {
      const res = await api.get<ClassroomData>(`api/classroom/get/${classId}`);
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  delete: async (classId: string) => {
    try {
      const res = await api.delete(`api/classroom/delete/${classId}`);
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  },
  update: async (classId: string, data: FormData) => {
    try {
      const res = await api.patch<ClassroomData>(`api/classroom/patch/${classId}`, data, {
        headers: {
          "Content-Type": "multipart/form-data"
        }
      });
      return res.data;
    } catch (err: unknown) {
      queryError(err);
      throw err;
    }
  }
}