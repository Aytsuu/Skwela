"use client"

import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "../../../components/ui/form";
import z from 'zod';
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Input } from "../../../components/ui/input";
import { Button } from "../../../components/ui/button";
import { signupSchema } from "../../../schemas/auth.schema";
import { useSignup } from "../../../hooks/useAuth";
import { useState } from "react";

export default function SignupPage() {
  const form = useForm<z.infer<typeof signupSchema>>({
    resolver: zodResolver(signupSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  })

  const [isSubmitting, setIsSubmitting] = useState(false);
  const { mutateAsync: signup } = useSignup();

  // Handlers
  const handleSignup = async () => {
    if (!(await form.trigger())) {
      return;
    }

    try {
      setIsSubmitting(true)
      await signup(form.getValues())
    } catch (err) {
      alert("Signup failed. Please try again.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div>
      <h1>Signup Page</h1>
      <Form {...form}>
        <form onSubmit={(e) => {
          e.preventDefault();
          handleSignup()
        }}>
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
                  <Input type="password" placeholder="Enter your password" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <Button type="submit">Sign Up</Button>
        </form>
      </Form>
    </div>
  )
}