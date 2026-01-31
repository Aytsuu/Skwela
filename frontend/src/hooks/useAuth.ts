import { useMutation } from "@tanstack/react-query";
import { AuthService } from "../services/auth.service";
import Cookies from "js-cookie";
import { useRouter } from "next/navigation";
import { useAuth } from "../components/context/AuthContext";
import { UserProfile } from "../types/auth";

export const useLogin = () => {
  const router = useRouter();
  const { storeUser } = useAuth();
  return useMutation({
    mutationFn: AuthService.login,
    onSuccess: (data) => {
      const { accessToken, refreshToken, ...rest } = data;

      const sixtyMinutes = new Date(new Date().getTime() + 60 * 60 * 1000);
      Cookies.set("accessToken", accessToken, {
        expires: sixtyMinutes, // 60 mins
        path: "/",
      });

      Cookies.set("refreshToken", refreshToken, {
        expires: 7, // 7 days
        path: "/",
      });

      storeUser(rest as UserProfile);
      router.push("/dashboard");
    },
  });
};

export const useSignup = () => {
  return useMutation({
    mutationFn: AuthService.signup,
  });
};
