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
import { FcGoogle } from "react-icons/fc";
import Link from "next/link";
import { api } from "@/services/api.service";
import { Separator } from "@/components/ui/separator";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Loader2 } from "lucide-react";
import axios from "axios";
import { useRouter } from "next/navigation";

export default function LoginPage() {
  const router = useRouter();
  const form = useForm<z.infer<typeof loginSchema>>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
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
      router.replace("/dashboard");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const status = err.response?.status;
        if (status === 403) {
          localStorage.setItem("otp_email", form.getValues().email);
          router.push("verify?type=login");
        }
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="w-screen h-screen flex justify-center items-center bg-custom-primary">
      <Form {...form}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleLogin();
          }}
          // className="rounded-sm flex flex-col gap-6 items-center w-110 border border-white/10 p-10 shadow-lg bg-custom-primary-contrast"
        >
          <Card className="mx-auto w-sm bg-custom-primary-contrast">
            <CardHeader>
              <CardTitle>Login to your account</CardTitle>
              <CardDescription>
                Enter your email below to login to your account
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="w-full flex flex-col gap-4">
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

                <span className="text-right cursor-pointer text-sm text-white/70 hover:underline">
                  Forgot Password?
                </span>
              </div>

              <div className="w-full flex flex-col gap-4">
                <Button
                  className="h-10 bg-blue-600 hover:bg-blue-700 text-white text-base cursor-pointer"
                  type={"submit"}
                  disabled={isSubmitting}
                >
                  {isSubmitting && <Loader2 className="animate-spin"/>}
                  Login
                </Button>
                <div className="mx-auto text-sm flex gap-1 text-white/70 items-center">
                  Don't have an account?
                  <Link
                    href={"signup"}
                    className="text-blue-500 font-medium hover:underline"
                  >
                    Create
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
