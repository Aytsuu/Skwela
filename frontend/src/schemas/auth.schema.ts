import { z } from 'zod'

export const loginSchema = z.object({
  email: z.email(),
  password: z.string().min(8, "Password must be at least 8 characters long"),
})

export const signupSchema = z.object({
  name: z.string()
    .min(1, "Name is required")
    .min(2, "Name must be atleast 2 characters"),
  email: z.email(),
  password: z.string().min(8, "Password must be at least 8 characters long"),
  confirmPassword: z.string()
}).refine(val => val.password == val.confirmPassword, {
  path: ['confirmPassword'],
  message: "Password doesn't match"
})