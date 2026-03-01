import { useMutation } from "@tanstack/react-query";
import { AuthService } from "../services/auth.service";
import { useAuth } from "../components/context/AuthContext";

export const useLogin = () => {
  const { storeUser } = useAuth();
  return useMutation({
    mutationFn: AuthService.login,
    onSuccess: (data) => {
      storeUser(data);
    },
  });
};

export const useSignup = () => {
  return useMutation({
    mutationFn: AuthService.signup
  });
}

export const useVerifyEmail = () => {
  const { storeUser } = useAuth();
  return useMutation({
    mutationFn: AuthService.verifyEmail,
    onSuccess: (data) => {
      storeUser(data);
    },
  })
}

export const useResendOtp = () => {
  return useMutation({
    mutationFn: AuthService.resendOtp
  })
}