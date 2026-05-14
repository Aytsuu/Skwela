"use client";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { emailVerifySchema } from "@/schemas/auth.schema";
import { AuthService } from "@/services/auth.service";
import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import { ChevronLeft, Loader2, ArrowLeft } from "lucide-react";
import { useRouter } from "next/navigation";
import React from "react";
import Link from "next/link";
import Image from "next/image";
import { useForm } from "react-hook-form";
import z from "zod";
import Cookies from "js-cookie";

const PageComponent = () => {
  // Hooks & States
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = React.useState<boolean>(false);
  const form = useForm<z.infer<typeof emailVerifySchema>>({
    resolver: zodResolver(emailVerifySchema),
    defaultValues: {
      email: "",
    },
  });

  // Handlers
  const handleEmailVerification = async () => {
    if (!(await form.trigger())) return;

    try {
      setIsSubmitting(true);
      await AuthService.validateEmail(form.getValues().email);
      const inFiveMinutes = new Date(new Date().getTime() + 5 * 60 * 1000);
      Cookies.set("otp_email", form.getValues().email, {
        expires: inFiveMinutes,
      });
      router.push("/authentication/verify?type=forgot_password");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const errorData = err.response?.data;
        form.setError("email", {
          type: "server",
          message: errorData,
        });
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="w-screen h-screen flex flex-col justify-center items-center bg-custom-primary relative">
      <Link
        href="/"
        className="absolute top-8 left-8 flex items-center gap-2 text-sm font-medium text-slate-500 hover:text-slate-900 transition-colors"
      >
        <ArrowLeft className="w-4 h-4" />
        Back
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
            handleEmailVerification();
          }}
        >
          <Card className="mx-auto w-110 bg-custom-primary-contrast">
            <CardHeader>
              <CardTitle>Recover Account</CardTitle>
              <CardDescription>
                Enter your email to receive a one-time verification code.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Email</FormLabel>
                    <FormControl>
                      <Input {...field} placeholder="Enter a valid email" />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
            <CardFooter>
              <div className="flex gap-2 items-center w-full">
                <Button
                  type="button"
                  variant={"secondary"}
                  className="shrink-0"
                  onClick={() => router.back()}
                >
                  <ChevronLeft className="opacity-50" />
                </Button>
                <div className="flex-1 min-w-0">
                  <Button
                    type="submit"
                    className="w-full"
                    disabled={isSubmitting}
                  >
                    {isSubmitting && <Loader2 className="animate-spin" />}
                    Continue
                  </Button>
                </div>
              </div>
            </CardFooter>
          </Card>
        </form>
      </Form>
    </div>
  );
};

export default PageComponent;
