"use client";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "../../../components/ui/form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input } from "../../../components/ui/input";
import { loginSchema } from "../../../schemas/auth.schema";
import { useForm } from "react-hook-form";
import { Button } from "../../../components/ui/button";
import { useState } from "react";
import { useLogin } from "../../../hooks/useAuth";
import { useRouter } from "next/navigation";
import { FcGoogle } from "react-icons/fc";
import Link from "next/link";
import { api } from "@/services/api.service";
import { Separator } from "@/components/ui/separator";

export default function LoginPage() {
  const router = useRouter();
  const form = useForm<z.infer<typeof loginSchema>>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const [isSubmitting, setIsSubmitting] = useState(false);
  const { mutateAsync: login } = useLogin();

  // Handlers
  const handleLogin = async () => {
    if (!(await form.trigger())) {
      return;
    }

    try {
      setIsSubmitting(true);
      await login(form.getValues());
    } catch (err) {
      alert("Failed to login. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="w-screen h-screen flex justify-center items-center bg-gray-100">
      <Form {...form}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleLogin();
          }}
          className="flex flex-col gap-6 items-center w-sm border p-10 shadow-lg bg-white"
        >
          <h1 className="text-xl font-semibold">SIGN IN</h1>

          <div className="w-full flex flex-col gap-4">
            <FormField
            control={form.control}
            name="username"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Username</FormLabel>
                <FormControl>
                  <Input placeholder="Enter your username" {...field} />
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

          <span className="text-right cursor-pointer text-sm">Forgot Password?</span>
          </div>

          <div className="w-full flex flex-col gap-4">
            <Button className="h-10 cursor-pointer" type={"submit"}>
            Login
          </Button>
          <span className="mx-auto cursor-pointer">Create new account</span>
          </div>
          
          <div className="relative w-full">
            <Separator />
            <p className="absolute left-1/2 -translate-1/2 bg-white px-2 text-xs text-gray-500">OR</p>
          </div>

          <Link
            href={`${api.defaults.baseURL}/api/auth/login-google`}
            className="flex justify-center items-center gap-2 border w-full h-10 rounded-lg"
          >
            <FcGoogle />
            Sign in with Google
          </Link>
        </form>
      </Form>
    </div>
  );
}
