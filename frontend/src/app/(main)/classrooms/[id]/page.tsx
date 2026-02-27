'use client';

import { useParams, useRouter } from "next/navigation";
import { useAuth } from "../../../../components/context/AuthContext";
import { useDeleteClassroom, useGetClassroomData } from "../../../../hooks/useClassroom";
import { Button } from "../../../../components/ui/button";
import { useUpdateStatus } from "../../../../hooks/useEnrollment";
import { useEffect, useState } from "react";
import Protected from "@/components/route/protected";

export default function ClassroomPage() {
  const { user } = useAuth();
  const router = useRouter();
  const params = useParams();
  const classId = params.id as string;

  const [isMounted, setIsMounted] = useState(false);

  const { mutateAsync: updateStatus } = useUpdateStatus();
  const { mutateAsync: deleteClassroom } = useDeleteClassroom();
  const { data: classroomData, error } = useGetClassroomData(classId, user?.userId, user?.role);
  
  // Flags
  const isTeacher = user?.role == "teacher";

  // Effects
  useEffect(() => {
    setIsMounted(true);
  }, [])

  // Handlers
  const handleLeaveClass = async () => {
    try {
      await updateStatus({
        classId: classId,
        userId: user?.userId
      });
      router.back();
    } catch (err) {
      alert("Failed to update enrollment status. Please try again.");
    } finally {

    }
  }

  const handleRemoveClassroom = async () => {
    try {
      await deleteClassroom(classId);
      router.back();
    } catch (err) {
      alert("Failed to remove classroom. Please try again.");
    } finally {

    }
  }
 
  if (!isMounted) return null;

  return (
    <Protected error={error}>
      <div>
        {classroomData && !isTeacher && (
          <Button onClick={handleLeaveClass}>Leave Classroom</Button>
        )}

        {classroomData && isTeacher && (
          <Button onClick={handleRemoveClassroom}>Remove Classroom</Button>
        )}
      </div>
    </Protected>
  )
}   