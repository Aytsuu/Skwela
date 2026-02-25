'use client';

import { Plus } from "lucide-react";
import { useAuth } from "../../../components/context/AuthContext";
import { Dialog, DialogFooter, DialogHeader } from "../../../components/ui/dialog";
import { DialogContent, DialogDescription, DialogTitle, DialogTrigger } from "../../../components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../../../components/ui/form";
import { useForm } from "react-hook-form";
import z from "zod";
import { classroomSchema } from "../../../schemas/classroom.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input } from "../../../components/ui/input";
import { Button } from "../../../components/ui/button";
import { useEffect, useState } from "react";
import { useCreateClassroom, useGetClassroomsByUserId } from "../../../hooks/useClassroom";
import { ClassroomResponse } from "../../../types/classroom";
import { enrollmentSchema } from "../../../schemas/enrollment.schema";
import { useCreateEnrollment, useGetEnrolledClasses } from "../../../hooks/useEnrollment";
import Link from "next/link";

export default function Dashboard() {
  const { user } = useAuth();
  console.log(user)
  const createClassroomForm = useForm<z.infer<typeof classroomSchema>>({
    resolver: zodResolver(classroomSchema),
    defaultValues: {
      name: "",
      description: "",
    },
  })

  const enrollClassForm = useForm<z.infer<typeof enrollmentSchema>>({
    resolver: zodResolver(enrollmentSchema),
    defaultValues: {
      classId: "",
    },
  })

  const [isMounted, setIsMounted] = useState(false);
  const [isOpenCreateClassroom, setIsOpenCreateClassroom] = useState(false);
  const [isOpenEnrollClass, setIsOpenEnrollClass] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { mutateAsync: createClassroom } = useCreateClassroom();
  const { mutateAsync: createEnrollment } = useCreateEnrollment();
  const { data: classroomsByUserId } = useGetClassroomsByUserId(user?.userId, user?.role);
  const { data: enrollmentsByUserId } = useGetEnrolledClasses(user?.userId, user?.role);

  // Effects
  useEffect(() => {
    setIsMounted(true);
  }, [])

  // Handlers 
  const handleCreateClassroom = async () => {
    try {
      setIsSubmitting(true);
      await createClassroom({
        ...createClassroomForm.getValues(),
        userId: user?.userId
      })

    } catch (err) {
      alert('Failed to create classroom. Please try again.')
    } finally {
      setIsSubmitting(false);
      setIsOpenCreateClassroom(prev => !prev)
    }
  }

  const handleEnrollClass = async () => {
    try {
      setIsSubmitting(true);
      await createEnrollment({
        ...enrollClassForm.getValues(),
        userId: user?.userId
      })
    } catch (err) {
      alert('Failed to create classroom. Please try again.')
    } finally {
      setIsSubmitting(false);
      setIsOpenEnrollClass(false)
    }
  }

  const showDialog = () => {
    user?.role === 'teacher' ?
      setIsOpenCreateClassroom(true) :
      setIsOpenEnrollClass(true);
  }

  // Render Helpers
  const createDialog = (
    <Dialog open={isOpenCreateClassroom} onOpenChange={() => setIsOpenCreateClassroom(prev => !prev)}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Classroom</DialogTitle>
          <DialogDescription>Start a new classroom and invite your students</DialogDescription>
        </DialogHeader>
        <div className="">
          <Form {...createClassroomForm}>
            <form>
              <FormField
                control={createClassroomForm.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel></FormLabel>
                    <FormControl>
                      <Input placeholder="Class Name" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={createClassroomForm.control}
                name="description"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel></FormLabel>
                    <FormControl>
                      <Input placeholder="Class Description" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </form>
          </Form>
        </div>
        <DialogFooter>
          <Button onClick={handleCreateClassroom}>
            Create Classroom
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )

  const enrollDialog = (
    <Dialog open={isOpenEnrollClass} onOpenChange={() => setIsOpenEnrollClass(prev => !prev)}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Classroom</DialogTitle>
          <DialogDescription>Join a class, learn and connect with other students.</DialogDescription>
        </DialogHeader>
        <div className="">
          <Form {...enrollClassForm}>
            <form>
              <FormField
                control={enrollClassForm.control}
                name="classId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel></FormLabel>
                    <FormControl>
                      <Input placeholder="Class ID" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </form>
          </Form>
        </div>
        <DialogFooter>
          <Button onClick={handleEnrollClass}>
            Join Class
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )

  if (!isMounted) return null;

  return (
    <>
      {/* Dialog Render  */}
      {createDialog}
      {enrollDialog}

      {/* Main Render  */}
      <div>
        <div className="flex flex-wrap">
        
          <div className=" w-50 h-50 border border-black flex items-center justify-center cursor-pointer"
            onClick={showDialog}
          >
            <Plus />
          </div>

          {user?.role == 'teacher' && classroomsByUserId?.map((classroom: ClassroomResponse) => (
            <Link href={`/classrooms/${classroom.class_id}`} className="w-50 h-50 border border-black" key={classroom.class_id}>
              {classroom.class_name}
              {classroom.class_description}
              {classroom.class_created_at}
            </Link>
          ))}

          {user?.role == 'student' && enrollmentsByUserId?.map((enrollment: any) => (
            <Link href={`/classrooms/${enrollment.class_id}`} className="w-50 h-50 border border-black" key={enrollment.class_id}>
              {enrollment.class_name}
              {enrollment.class_description}
              {enrollment.enrolled_at}
            </Link>
          ))}
        </div>
      </div>
    </>
  )
}