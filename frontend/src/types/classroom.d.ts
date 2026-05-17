import { UserProfile } from "./auth";

export interface ClassroomCreateRequest {
  name: string;
  description: string;
  userId: string;
}

export interface StudentData {
  studentId: string;
  fname: string;
  mname: string;
  lname: string;
}

export interface ClassroomData {
  classId: string;
  className: string;
  classDescription: string;
  classCreatedAt: string;
  classBanner: string;
  creator?: UserProfile;
  students: StudentData[];
}
