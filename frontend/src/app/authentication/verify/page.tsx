"use client"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Field, FieldDescription, FieldError, FieldLabel } from "@/components/ui/field"
import { InputOTP, InputOTPGroup, InputOTPSeparator, InputOTPSlot } from "@/components/ui/input-otp"
import { useResendOtp, useVerifyEmail } from "@/hooks/useAuth"
import axios from "axios"
import { REGEXP_ONLY_DIGITS } from "input-otp"
import { Loader2, RefreshCwIcon } from "lucide-react"
import { useRouter } from "next/navigation"
import React from "react"
import { fa } from "zod/v4/locales"

export default function SignupPage() {
  // Hooks & States
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = React.useState<boolean>(false);
  const [isResendingOtp, setIsResendingOtp] = React.useState<boolean>(false);
  const [otpInput, setOtpInput] = React.useState<string>("")
  const [showError, setShowError] = React.useState<boolean>(false);
  const [errorMessage, setErrorMessage] = React.useState<string>("");
  const email = localStorage.getItem("otp_email");

  // Queries
  const { mutateAsync: verifyEmail } = useVerifyEmail();
  const { mutateAsync: resendOtp } = useResendOtp();

  // Effects
  React.useEffect(() => {
    setShowError(false);
  }, [otpInput])

  // Handlers
  const handleVerifyEmail = async () => {
    if(!email) return;
    try {
      setIsSubmitting(true);
      await verifyEmail({
        email: email,
        otpCode: otpInput
      })
      router.replace("/dashboard");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const errorData = err.response?.data
        setErrorMessage(errorData.message);
        setShowError(true);
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  const handleResendOtp = async () => {
    if (!email) return;
    try {
      setIsResendingOtp(true);
      await resendOtp(email);
    } catch (err) {
      if(axios.isAxiosError(err)) {

      }
    } finally {
      setIsResendingOtp(false);
    }
  }
  
  // Render
  return (
    <div className="w-screen h-screen flex justify-center items-center bg-custom-primary">
      <Card className="mx-auto max-w-md bg-custom-primary-contrast">
      <CardHeader>
        <CardTitle>Verify your login</CardTitle>
        <CardDescription>
          Enter the verification code we sent to your email address:{" "}
          <span className="font-medium">m@example.com</span>.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Field>
          <div className="flex items-center justify-between">
            <FieldLabel htmlFor="otp-verification">
              Verification code
            </FieldLabel>
            <Button disabled={isResendingOtp} onClick={handleResendOtp} variant="outline" size="xs">
              <RefreshCwIcon className={`${isResendingOtp && "animate-spin"}`}/>
              Resend Code
            </Button>
          </div>
          <InputOTP value={otpInput} onChange={(val) => setOtpInput(val)} maxLength={6} id="otp-verification" pattern={REGEXP_ONLY_DIGITS} required>
            <InputOTPGroup className="*:data-[slot=input-otp-slot]:h-12 *:data-[slot=input-otp-slot]:w-11 *:data-[slot=input-otp-slot]:text-xl">
              <InputOTPSlot index={0} />
              <InputOTPSlot index={1} />
              <InputOTPSlot index={2} />
            </InputOTPGroup>
            <InputOTPSeparator className="mx-2" />
            <InputOTPGroup className="*:data-[slot=input-otp-slot]:h-12 *:data-[slot=input-otp-slot]:w-11 *:data-[slot=input-otp-slot]:text-xl">
              <InputOTPSlot index={3} />
              <InputOTPSlot index={4} />
              <InputOTPSlot index={5} />
            </InputOTPGroup>
          </InputOTP>
          {showError && <FieldError>{errorMessage}</FieldError>}
          <FieldDescription>
            <a onClick={() => router.back()} href="#">I no longer have access to this email address.</a>
          </FieldDescription>
        </Field>
      </CardContent>
      <CardFooter>
        <Field>
          <Button 
            onClick={handleVerifyEmail} 
            className="w-full text-base"
            disabled={otpInput.length < 6 || isSubmitting}
          >
            {isSubmitting && <Loader2 className="animate-spin"/>}
            Verify
          </Button>
        </Field>
      </CardFooter>
    </Card>
    </div>
  )
}