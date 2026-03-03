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
import { useResetPassword } from "@/hooks/useAuth";
import { resetPassSchema } from "@/schemas/auth.schema";
import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import { ChevronLeft, Eye, EyeClosed, Loader2 } from "lucide-react";
import { useRouter } from "next/navigation";
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
      confirmPassword: "",
    },
  });

  const [showPassword, setShowPassword] = React.useState<boolean>(false);
  const [showConfirmPassword, setShowConfirmPassword] =
    React.useState<boolean>(false);
  const Icon1 = showPassword ? EyeClosed : Eye;
  const Icon2 = showConfirmPassword ? EyeClosed : Eye;

  // Queries
  const { mutateAsync: resetPassword } = useResetPassword();

  // Watch
  const password = form.watch("password");
  const confirmPassword = form.watch("confirmPassword");

  // Effects
  React.useEffect(() => {
    form.clearErrors();
  }, [password, confirmPassword]);

  // Handlers
  const handleResetPassword = async () => {
    if (!email) return;
    if (!(await form.trigger())) return;

    try {
      setIsSubmitting(true);
      const values = form.getValues();
      await resetPassword({
        email: email,
        password: values.password,
      });
      Cookies.remove("otp_email");
      router.push("/login");
    } catch (err) {
    } finally {
      setIsSubmitting(false);
    }
  };

  // Render
  return (
    <div className="w-screen h-screen flex items-center justify-center bg-custom-primary">
      <Form {...form}>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleResetPassword();
          }}
        >
          <Card className="w-sm bg-custom-primary-contrast">
            <CardHeader>
              <CardTitle>Reset Password</CardTitle>
              <CardDescription>
                Retrieve your account by setting new password
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <FormField
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>New Password</FormLabel>
                    <FormControl>
                      <div className="relative">
                        <Input
                          type={showPassword ? "text" : "password"}
                          placeholder="Enter your password"
                          className="pr-10"
                          {...field}
                        />
                        <Icon1
                          size={18}
                          className="absolute right-3 top-1/2 -translate-y-1/2 cursor-pointer opacity-60 hover:opacity-80"
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
                    <FormLabel>Confirm Password</FormLabel>
                    <FormControl>
                      <div className="relative">
                        <Input
                          type={showConfirmPassword ? "text" : "password"}
                          placeholder="Re-enter your password"
                          className="pr-10"
                          {...field}
                        />
                        <Icon2
                          size={18}
                          className="absolute right-3 top-1/2 -translate-y-1/2 cursor-pointer opacity-60 hover:opacity-80"
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
                    Confirm Reset
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
