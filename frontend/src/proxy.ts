import { NextRequest, NextResponse } from "next/server";

export default function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl; 
  const token = request.cookies.get('accessToken')?.value;
  const isAuthPage = request.nextUrl.pathname.includes("/authentication");
  const isPublicPath = pathname === '/' || pathname.startsWith('/authentication');

  if (!isPublicPath && !token) {
    return NextResponse.redirect(new URL('/authentication/login', request.url));
  }

  if (token && isAuthPage) {
    return NextResponse.redirect(new URL('/dashboard', request.url))
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    '/((?!api|_next/static|_next/image|favicon.ico).*)'
  ] 
}