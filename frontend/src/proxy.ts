import { NextRequest, NextResponse } from "next/server";

const STATIC_FILE_PATTERN = /\.[^/]+$/;

export default function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const token = request.cookies.get("accessToken")?.value;
  const otpEmail = request.cookies.get("otp_email")?.value;

  const isStaticAsset =
    pathname.startsWith("/assets/") ||
    pathname.startsWith("/_next/") ||
    pathname === "/favicon.ico" ||
    STATIC_FILE_PATTERN.test(pathname);

  if (isStaticAsset) {
    return NextResponse.next();
  }

  const isAuthPage = pathname.startsWith("/authentication");
  const isOtpPath = pathname.startsWith("/authentication/verify");
  const isPublicPath =
    pathname === "/" ||
    pathname.startsWith("/authentication") ||
    pathname.startsWith("/404");

  if (!otpEmail && isOtpPath) {
    return NextResponse.redirect(new URL("/404/not-found", request.url));
  }

  if (!isPublicPath && !token) {
    return NextResponse.redirect(new URL("/authentication/login", request.url));
  }

  if (token && isAuthPage) {
    return NextResponse.redirect(new URL("/dashboard", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|favicon.ico|assets|.*\\..*).*)"],
};
