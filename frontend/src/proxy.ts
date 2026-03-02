import { NextRequest, NextResponse } from "next/server";

export default function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl; 
  const token = request.cookies.get('accessToken')?.value;
  const otpEmail = request.cookies.get("otp_email")?.value;
  const isAuthPage = pathname.startsWith("/authentication");
  const isPublicPath = pathname === '/' || pathname.startsWith('/authentication') || pathname.startsWith("/404");
  const isOtpPath = pathname.startsWith("/authentication/verify")

  if (!otpEmail && isOtpPath) {
    return NextResponse.redirect(new URL('/404/not-found', request.url));
  }

  if (!isPublicPath && !token) {
    return NextResponse.redirect(new URL('/authentication/login', request.url));
  }

  if (token && isAuthPage) {
    return NextResponse.redirect(new URL('/dashboard', request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    '/((?!api|_next/static|_next/image|favicon.ico).*)'
  ] 
}