import axios, { AxiosError } from "axios";

export const queryError = (error: unknown) => {
  if (process.env.NODE_ENV !== "development") {
    return;
  }

  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError;
    const payload = axiosError.response?.data;
    const message = axiosError.message || "Axios request failed";

    console.warn(payload ?? message);
    return;
  }

  if (error instanceof Error) {
    console.warn(error.message);
    return;
  }

  console.warn("Unknown error");
};
