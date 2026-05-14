import { UserProfile } from "./auth";

export interface ClassroomCreateRequest {
  name: string;
  description: string;
  userId: string;
}

export interface ClassroomData {
  classId: string;
  className: string;
  classDescription: string;
  classCreatedAt: string;
  classBanner: string;
  creator?: UserProfile
}