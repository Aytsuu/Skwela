import { z } from 'zod';

export const classroomSchema = z.object({
  name: z.string(),
  description: z.string(),
  userId: z.string().min(1, 'User ID is required')
})