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
import { useSignup } from "../../../hooks/use-auth";
import React from "react";
import { Separator } from "@/components/ui/separator";
import Link from "next/link";
import { api } from "@/services/api.service";
import { FcGoogle } from "react-icons/fc";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useRouter } from "next/navigation";
import { Eye, EyeClosed, Loader2, ArrowLeft } from "lucide-react";
import axios from "axios";
import Cookies from "js-cookie";
import Image from "next/image";

const PageComponent = () => {
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

  const [isSubmitting, setIsSubmitting] = React.useState<boolean>(false);
  const [showPassword, setShowPassword] = React.useState<boolean>(false);
  const [showConfirmPassword, setShowConfirmPassword] =
    React.useState<boolean>(false);
  const Icon1 = showPassword ? EyeClosed : Eye;
  const Icon2 = showConfirmPassword ? EyeClosed : Eye;

  // Queries
  const { mutateAsync: signup } = useSignup();

  // Handlers
  const handleSignup = async () => {
    if (!(await form.trigger())) {
      return;
    }

    try {
      setIsSubmitting(true);
      await signup(form.getValues());

      // Store the email temporarily to cookie for OTP verification
      const inFiveMinutes = new Date(new Date().getTime() + 5 * 60 * 1000);
      Cookies.set("otp_email", form.getValues().email, {
        expires: inFiveMinutes,
        path: "/",
      });

      router.push("verify?type=signup");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const errorData = err.response?.data;
        form.setError(errorData.field, {
          type: "server",
          message: errorData.message,
        });
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  // Render
  return (
    <div className="w-screen h-screen flex flex-col justify-center items-center bg-background relative selection-notion">
      <Link
        href="/"
        className="absolute top-8 left-8 flex items-center gap-2 text-xs font-bold uppercase tracking-widest text-muted-foreground hover:text-primary transition-colors"
      >
        <ArrowLeft className="w-4 h-4" />
        Back to Home
      </Link>
      <div className="mb-8 flex items-center justify-center gap-1 group">
        <Image
          src="/assets/esecai_logo.svg"
          alt="esecai logo"
          width={44}
          height={44}
          priority
          className="transition-transform duration-300 group-hover:rotate-12"
        />
        <span className="font-black text-3xl tracking-tighter">esecai</span>
      </div>
      <Form {...form}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleSignup();
          }}
        >
          <Card className="w-[450px] border-border bg-card shadow-soft">
            <CardHeader className="space-y-1">
              <CardTitle className="text-2xl font-black tracking-tight">
                Create an account
              </CardTitle>
              <CardDescription className="text-sm font-medium">
                Join our platform to start managing your classrooms
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="w-full flex flex-col gap-4">
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
                        Display Name
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder="John Doe"
                          className="bg-muted/5 border-border focus:border-indigo-500 transition-colors h-11"
                          {...field}
                        />
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
                      <FormLabel className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
                        Email Address
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder="name@school.edu"
                          className="bg-muted/5 border-border focus:border-indigo-500 transition-colors h-11"
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="password"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
                          Password
                        </FormLabel>
                        <FormControl>
                          <div className="relative">
                            <Input
                              type={showPassword ? "text" : "password"}
                              placeholder="••••••••"
                              className="bg-muted/5 border-border focus:border-indigo-500 transition-colors pr-10 h-11"
                              {...field}
                            />
                            <Icon1
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

                  <FormField
                    control={form.control}
                    name="confirmPassword"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
                          Confirm
                        </FormLabel>
                        <FormControl>
                          <div className="relative">
                            <Input
                              type={showConfirmPassword ? "text" : "password"}
                              placeholder="••••••••"
                              className="bg-muted/5 border-border focus:border-indigo-500 transition-colors pr-10 h-11"
                              {...field}
                            />
                            <Icon2
                              size={16}
                              className="absolute right-3 top-1/2 -translate-y-1/2 cursor-pointer text-muted-foreground hover:text-indigo-600 transition-colors"
                              onClick={() =>
                                setShowConfirmPassword(!showConfirmPassword)
                              }
                            />
                          </div>
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              <div className="w-full flex flex-col gap-4">
                <Button
                  className="w-full h-12 text-base font-bold bg-indigo-600 text-white hover:bg-indigo-700 rounded-full transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5"
                  type={"submit"}
                  disabled={isSubmitting}
                >
                  {isSubmitting ? (
                    <Loader2 className="animate-spin w-5 h-5" />
                  ) : (
                    "Create Account"
                  )}
                </Button>
                <div className="mx-auto text-xs font-medium flex gap-1.5 text-muted-foreground">
                  Already have an account?
                  <Link
                    href={"login"}
                    className="text-indigo-600 font-black hover:text-indigo-700"
                  >
                    Sign In
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
                href={`${api.defaults.baseURL}/api/auth/login-google`}
                className="bg-white hover:bg-muted/30 text-foreground font-bold flex justify-center items-center gap-3 border border-border w-full h-11 rounded-full transition-all shadow-sm hover:shadow-md"
              >
                <FcGoogle size={20} />
                <span className="text-sm">Google Account</span>
              </Link>
            </CardContent>
          </Card>
        </form>
      </Form>
    </div>
  );
};

export default PageComponent;
