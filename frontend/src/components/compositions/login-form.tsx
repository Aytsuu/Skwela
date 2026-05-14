"use client";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Input } from "@/components/ui/input";
import { loginSchema } from "@/schemas/auth.schema";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import React from "react";
import { useLogin } from "@/hooks/use-auth";
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
import { Eye, EyeClosed, Loader2, LucideIcon } from "lucide-react";
import axios from "axios";
import { useRouter } from "next/navigation";
import Cookies from 'js-cookie';

export const LoginForm = () => {
  const router = useRouter();
  const form = useForm<z.infer<typeof loginSchema>>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const [isSubmitting, setIsSubmitting] = React.useState<boolean>(false);
  const [showPassword, setShowPassword] = React.useState<boolean>(false);
  const Icon: LucideIcon = showPassword ? EyeClosed : Eye;

  const { mutateAsync: login } = useLogin();

  const email = form.watch('email');
  const password = form.watch('password');

  React.useEffect(() => {
    form.clearErrors(["email", "password"]);
  }, [email, password, form]);

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
        const errorData = err.response?.data;
        if (status === 403) {
          const inFiveMinutes = new Date(new Date().getTime() + 5 * 60 * 1000);
          Cookies.set("otp_email", form.getValues().email, { expires: inFiveMinutes, path: "/" })
          router.push("verify?type=login");
        } else if (status === 404) {
          form.setError("email", {
            type: "server",
            message: errorData
          })
        } else {
          form.setError("password", {
            type: "server",
            message: errorData
          })
          form.setError("email", {})
        }
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Form {...form}>
      <form
        onSubmit={(e) => {
          e.preventDefault();
          handleLogin();
        }}
      >
        <Card className="w-[400px] border-border bg-card shadow-soft">
          <CardHeader className="space-y-1">
            <CardTitle className="text-2xl font-black tracking-tight">Login</CardTitle>
            <CardDescription className="text-sm font-medium">
              Enter your credentials to access your dashboard
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="w-full flex flex-col gap-4">
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">Email Address</FormLabel>
                    <FormControl>
                      <Input placeholder="name@school.edu" className="bg-muted/5 border-border focus:border-indigo-500 transition-colors h-11" {...field} />
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
                    <div className="flex justify-between items-end mb-1">
                      <FormLabel className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">Password</FormLabel>
                      <span className="cursor-pointer text-[10px] font-black text-indigo-600 hover:text-indigo-700 uppercase tracking-widest"
                        onClick={() => router.push("login/identify")}
                      >
                        Forgot?
                      </span>
                    </div>
                    <FormControl>
                      <div className="relative">
                        <Input
                          type={showPassword ? "text" : "password"}
                          placeholder="••••••••"
                          className="bg-muted/5 border-border focus:border-indigo-500 transition-colors pr-10 h-11"
                          {...field}
                        />
                        <Icon 
                          size={16} 
                          className="absolute right-3 top-1/2 -translate-y-1/2 cursor-pointer text-muted-foreground hover:text-indigo-600 transition-colors"
                          onClick={() => setShowPassword(!showPassword)}
                        />
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="w-full flex flex-col gap-4">
              <Button
                className="w-full h-12 text-base font-bold bg-indigo-600 text-white hover:bg-indigo-700 rounded-full transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5"
                type={"submit"}
                disabled={isSubmitting}
              >
                {isSubmitting ? <Loader2 className="animate-spin w-5 h-5"/> : "Sign In"}
              </Button>
              <div className="mx-auto text-xs font-medium flex gap-1.5 text-muted-foreground">
                Need an account?
                <Link
                  href={"signup"}
                  className="text-indigo-600 font-black hover:text-indigo-700"
                >
                  Create one
                </Link>
              </div>
            </div>

            <div className="relative w-full flex items-center justify-center pt-2">
              <Separator className="bg-border/60" />
              <p className="absolute bg-card px-4 text-[9px] font-black tracking-widest text-muted-foreground/60 uppercase">
                OR CONTINUE WITH
              </p>
            </div>

            <Link
              href={`${api.defaults.baseURL}/api/auth/login-google?returnUrl=${process.env.NODE_ENV == "development" ? "http://localhost:3000" : process.env.NEXT_PUBLIC_URL}`}
              className="bg-white hover:bg-muted/30 text-foreground font-bold flex justify-center items-center gap-3 border border-border w-full h-11 rounded-full transition-all shadow-sm hover:shadow-md"
            >
              <FcGoogle size={20} />
              <span className="text-sm">Google Account</span>
            </Link>
          </CardContent>
        </Card>
      </form>
    </Form>
  );
};
