"use client";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { useResetPassword } from "@/hooks/useAuth";
import { resetPassSchema } from "@/schemas/auth.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import { Loader2, MoveLeft } from "lucide-react";
import { useRouter } from "next/navigation"
import React from "react";
import { useForm } from "react-hook-form";
import z from "zod";
import Cookies from "js-cookie";

export default () => {
  // Hooks & States
  const router = useRouter();
  const email = Cookies.get("otp_email");
  const [isSubmitting, setIsSubmitting] = React.useState<boolean>(false);
  const form = useForm<z.infer<typeof resetPassSchema>>({
    resolver: zodResolver(resetPassSchema),
    defaultValues: {
      password: "",
      confirmPassword: ""
    }
  });
  
  // Queries
  const { mutateAsync: resetPassword } = useResetPassword();

  // Watch
  const password = form.watch('password');
  const confirmPassword = form.watch('confirmPassword');

   // Effects
  React.useEffect(() => {
    form.clearErrors();
  }, [password, confirmPassword])

  // Handlers
  const handleResetPassword = async () => {
    if (!email) return;
    if (!(await form.trigger())) return;
    
    try {
      setIsSubmitting(true);
      const values = form.getValues();
      await resetPassword({
        email: email,
        password: values.password
      });
      Cookies.remove("otp_email");
      router.push("/login");
    } catch (err) {

    } finally {
      setIsSubmitting(false);
    }
  }

  // Render
  return (
    <div className="w-screen h-screen flex items-center justify-center bg-custom-primary">
      <Form {...form}>
        <form onSubmit={(e) => {
          e.preventDefault();
          handleResetPassword();
        }}>
          <Card className="w-sm bg-custom-primary-contrast">
            <CardHeader>
              <CardTitle>Reset Password</CardTitle>
              <CardDescription>Retrieve your account by setting new password</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <FormField 
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>New Password</FormLabel>
                    <FormControl>
                      <Input type="password" {...field} placeholder="Enter your new password"/>
                    </FormControl>
                    <FormMessage/>
                  </FormItem>
                )}
              />
              <FormField 
                control={form.control}
                name="confirmPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Confirm Password</FormLabel>
                    <FormControl>
                      <Input type="password" {...field} placeholder="Re-enter your new password"/>
                    </FormControl>
                    <FormMessage/>
                  </FormItem>
                )}
              />
            </CardContent>
            <CardFooter>
              <div className="flex gap-2 items-center w-full">
                <Button type="button" variant={"secondary"} className="shrink-0"
                  onClick={() => router.back()}
                >
                  <MoveLeft  className="opacity-50"/>
                </Button>
                <div className="flex-1 min-w-0">
                  <Button
                    type="submit"
                    className="w-full"
                    disabled={isSubmitting}
                  >
                    {isSubmitting && <Loader2 className="animate-spin" />}
                    Confirm Reset
                  </Button>
                </div>
              </div>
            </CardFooter>
          </Card>
        </form>
      </Form>
    </div>
  )
}