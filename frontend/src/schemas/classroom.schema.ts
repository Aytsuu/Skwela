import { z } from 'zod';

const bannerFileSchema = z.custom<string | File>(
  (value) =>
    value === undefined ||
    typeof value === "string" ||
    (typeof File !== "undefined" && value instanceof File),
  "Invalid banner file",
);

export const classroomSchema = z.object({
  name: z.string()
    .min(1, "Class Name is required")
    .min(2, "Must be atleaast 2 characters"),
  description: z.string(),
  bannerFile: bannerFileSchema.optional(),
})