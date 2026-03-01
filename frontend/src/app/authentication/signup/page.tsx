"use client";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "../../../components/ui/form";
import z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Input } from "../../../components/ui/input";
import { Button } from "../../../components/ui/button";
import { signupSchema } from "../../../schemas/auth.schema";
import { useSignup } from "../../../hooks/useAuth";
import { useState } from "react";
import { Separator } from "@/components/ui/separator";
import Link from "next/link";
import { api } from "@/services/api.service";
import { FcGoogle } from "react-icons/fc";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useRouter } from "next/navigation";
import { Loader2 } from "lucide-react";
import axios from "axios";

export default function SignupPage() {
  const router = useRouter();
  const form = useForm<z.infer<typeof signupSchema>>({
    resolver: zodResolver(signupSchema),
    defaultValues: {
      name: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const { mutateAsync: signup } = useSignup();

  // Handlers
  const handleSignup = async () => {  
    if (!(await form.trigger())) {
      return;
    }

    try {
      setIsSubmitting(true);
      await signup(form.getValues());
      localStorage.setItem("otp_email", form.getValues().email)
      router.push("verify?type=signup");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const errorData = err.response?.data;
        form.setError(errorData.field, {
          type: "server",
          message: errorData.message
        })
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  // Render
  return (
    <div className="w-screen h-screen flex justify-center items-center bg-custom-primary">
      <Form {...form}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleSignup();
          }}
        >
          <Card className="mx-auto w-sm bg-custom-primary-contrast">
            <CardHeader>
              <CardTitle>Create an account</CardTitle>
              <CardDescription>Enter your information below to create your account</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="w-full flex flex-col gap-4">
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Name</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter your name" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="email"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Email</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter your email" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Password</FormLabel>
                      <FormControl>
                        <Input
                          type="password"
                          placeholder="Enter your password"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
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
                        <Input
                          type="password"
                          placeholder="Re-enter your password"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <div className="w-full flex flex-col gap-4">
                <Button
                  className="h-10 bg-blue-600 hover:bg-blue-700 text-white text-base cursor-pointer"
                  type={"submit"}
                  disabled={isSubmitting}
                >
                  {isSubmitting && <Loader2 className="animate-spin"/>}
                  Sign up
                </Button>
                <div className="mx-auto text-sm flex gap-1 text-white/70 items-center">
                  Already have an account?
                  <Link
                    href={"login"}
                    className="text-blue-500 font-medium hover:underline"
                  >
                    Login
                  </Link>
                </div>
              </div>

              <div className="relative w-full">
                <Separator />
                <p className="absolute left-1/2 -translate-1/2 bg-custom-primary-contrast px-2 text-xs text-white/20">
                  OR
                </p>
              </div>

              <Link
                href={`${api.defaults.baseURL}/api/auth/login-google`}
                className="bg-primary text-primary-foreground font-medium flex justify-center items-center gap-2 border w-full h-10 rounded-lg"
              >
                <FcGoogle size={22} />
                Sign in with Google
              </Link>
            </CardContent>
          </Card>
        </form>
      </Form>
    </div>
  );
}
