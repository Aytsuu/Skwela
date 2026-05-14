import type { Metadata } from "next";
import { Plus_Jakarta_Sans, JetBrains_Mono, Dancing_Script } from "next/font/google";
import "./globals.css";
import Provider from "./queryClientProvider";
import { ThemeProvider } from "@/components/wrapper/theme-provider";
import { TooltipProvider } from "@/components/ui/tooltip";
import { Toaster } from "@/components/ui/sonner";
import { Analytics } from "@vercel/analytics/next";
import { SpeedInsights } from "@vercel/speed-insights/next";
import { NotificationProvider } from "@/context/NotificationContext";
import { AuthProvider } from "@/context/AuthContext";

const fontSans = Plus_Jakarta_Sans({
  variable: "--font-sans",
  subsets: ["latin"],
});

const fontMono = JetBrains_Mono({
  variable: "--font-mono",
  subsets: ["latin"],
});

const fontScript = Dancing_Script({
  variable: "--font-script",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "esecai - AI Activity Checker",
  description: "",
  icons: {
    icon: "/assets/esecai_logo.svg",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body
        className={`${fontSans.variable} ${fontMono.variable} ${fontScript.variable} font-sans antialiased`}
      >
        <SpeedInsights />
        <Analytics />
        <ThemeProvider attribute={"class"} defaultTheme="light">
          <TooltipProvider>
            <Provider>
              <AuthProvider>
                <NotificationProvider>{children}</NotificationProvider>
              </AuthProvider>
            </Provider>
          </TooltipProvider>
          <Toaster />
        </ThemeProvider>
      </body>
    </html>
  );
}
