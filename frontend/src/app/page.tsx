
import Link from "next/link";

export default function Home() {
  return (
      <div>
          <h1>Welcome!</h1>
          <Link href="authentication/login">Login</Link>
          <Link href="authentication/signup">Signup</Link>
          <Link href="https://api.paoloaraneta.dev/api/auth/login-google">Signin with Google</Link>
      </div>
  );
}
