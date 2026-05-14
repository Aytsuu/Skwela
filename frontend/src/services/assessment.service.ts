import React from 'react'
import { api } from './api.service';
import { Assessment } from '@/types/assessment';
import { queryError } from '@/helpers/errorDisplay';

export const AssessmentService = {
  create: async (data: FormData) => {
    try {
      const res = await api.post<Assessment>("api/assessment/create", data);
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  },
  list: async (classId: string) => {
    try {
      const res = await api.get<Assessment[]>("api/assessment/get", {
        params: {
          classId
        }
      });
      return res.data;
    } catch (err: any) {
      queryError(err);
      throw err;
    }
  }
}