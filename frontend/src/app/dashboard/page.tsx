'use client';

import { Plus } from "lucide-react";
import { useAuth } from "../../components/context/AuthContext";
import { Dialog, DialogFooter, DialogHeader } from "../../components/ui/dialog";
import { DialogContent, DialogDescription, DialogTitle, DialogTrigger } from "../../components/ui/dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../../components/ui/form";
import { useForm } from "react-hook-form";
import z from "zod";
import { classroomSchema } from "../../schemas/classroom.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input } from "../../components/ui/input";
import { Button } from "../../components/ui/button";
import { useState } from "react";
import { useCreateClassroom, useGetClassroomsByUserId } from "../../hooks/useClassroom";
import { ClassroomResponse } from "../../types/classroom";

export default function Dashboard() {
  const { user } = useAuth();
  const form = useForm<z.infer<typeof classroomSchema>>({
    resolver: zodResolver(classroomSchema),
    defaultValues: {
      name: undefined,
      description: undefined,
    },
  })

  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { mutateAsync: createClassroom } = useCreateClassroom();
  const { data: classroomsByUserId } = useGetClassroomsByUserId(user?.userId);

  // Handlers 
  const handleCreateClassroom = async () => {
    try {
      setIsSubmitting(true);
      await createClassroom({
        ...form.getValues(),  
        userId: user?.userId
      })

    } catch (err) {
      alert('Failed to create classroom. Please try again.')
    } finally {
      setIsSubmitting(false);
      setIsDialogOpen(prev => !prev)
    }
  }

  const createDialog = (
    <Dialog open={isDialogOpen} onOpenChange={() => setIsDialogOpen(prev => !prev)}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Classroom</DialogTitle>
          <DialogDescription>Start a new classroom and invite your students</DialogDescription>
        </DialogHeader>
        <div className="">
          <Form {...form}>
            <form>
              <FormField
                control={form.control}
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
                control={form.control}
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

  return (
    <>
      {/* Dialog Render  */ }
      {createDialog}

      {/* Main Render  */}
      <div>
        <div className="flex flex-wrap">
          {user?.role === 'Teacher' && (
            <div className=" w-50 h-50 border border-black flex items-center justify-center cursor-pointer"
              onClick={() => setIsDialogOpen(prev => !prev)}
            >
              <Plus />
            </div>
          )}

          {classroomsByUserId?.map((classroom: ClassroomResponse) => (
            <div className="w-50 h-50 border border-black" key={classroom.class_id}>
              {classroom.class_name}
              {classroom.class_description}
              {classroom.class_created_at}
            </div>
          ))}
        </div>
      </div>
    </>
  )
}