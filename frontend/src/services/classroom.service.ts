import { queryError } from "@/helpers/errorDisplay";
import { ClassroomData, StudentData } from "../types/classroom";
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
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  getData: async (classId: string) => {
    try {
      const res = await api.get<ClassroomData>(`api/classroom/get/${classId}`);
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  delete: async (classId: string) => {
    try {
      const res = await api.delete(`api/classroom/delete/${classId}`);
      return res.data;
    } catch (err: any) {
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
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  createStudent: async (classId: string, payload: Omit<StudentData, "studentId">) => {
    try {
      const res = await api.post<StudentData>(`api/classroom/${classId}/students`, payload);
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  importStudents: async (classId: string, data: FormData) => {
    try {
      const res = await api.post<StudentData[]>(`api/classroom/${classId}/students/import`, data, {
        headers: {
          "Content-Type": "multipart/form-data"
        }
      });
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  updateStudent: async (classId: string, studentId: string, payload: Omit<StudentData, "studentId">) => {
    try {
      const res = await api.patch<StudentData>(`api/classroom/${classId}/students/${studentId}`, payload);
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  deleteStudent: async (classId: string, studentId: string) => {
    try {
      const res = await api.delete(`api/classroom/${classId}/students/${studentId}`);
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  }
}
