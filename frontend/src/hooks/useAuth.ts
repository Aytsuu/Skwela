import { useMutation } from "@tanstack/react-query";
import { AuthService } from "../services/auth.service";
import { useRouter } from "next/navigation";
import { useAuth } from "../components/context/AuthContext";

export const useLogin = () => {
  const router = useRouter();
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