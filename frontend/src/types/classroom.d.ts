export interface ClassroomCreateRequest {
  name: string;
  description: string;
  userId: string;
}

export interface ClassroomResponse {
  class_id: string;
  class_name: string;
  class_description: string;
  class_created_at: string;
}