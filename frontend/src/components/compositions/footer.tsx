import Link from "next/link";
import React from "react";
import Image from "next/image";

export const Footer = () => {
  return (
    <footer className="py-20 px-8 border-t border-border bg-background">
      <div className="max-w-6xl mx-auto">
        <div className="flex flex-col md:flex-row justify-between items-start gap-12 mb-20">
          <div className="space-y-4">
            <div className="flex items-center gap-2 group">
              <Image 
                src="/assets/esecai_logo.svg" 
                alt="esecai logo" 
                width={44} 
                height={44} 
                className="transition-transform duration-300 group-hover:rotate-12"
              />
              <span className="font-bold text-2xl tracking-tight">esecai</span>
            </div>
            <p className="text-muted-foreground max-w-xs leading-relaxed font-medium">
              Advancing academic integrity through intelligent, unobtrusive monitoring.
            </p>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-16 text-sm">
            <div className="space-y-4">
              <h4 className="font-bold text-foreground">Platform</h4>
              <ul className="space-y-3 text-muted-foreground">
                <li><Link href="#" className="hover:text-primary transition-colors">Features</Link></li>
                <li><Link href="#" className="hover:text-primary transition-colors">Security</Link></li>
                <li><Link href="#" className="hover:text-primary transition-colors">Pricing</Link></li>
              </ul>
            </div>
            <div className="space-y-4">
              <h4 className="font-bold text-foreground">Company</h4>
              <ul className="space-y-3 text-muted-foreground">
                <li><Link href="#" className="hover:text-primary transition-colors">About Us</Link></li>
                <li><Link href="#" className="hover:text-primary transition-colors">Contact</Link></li>
                <li><Link href="#" className="hover:text-primary transition-colors">Privacy</Link></li>
              </ul>
            </div>
          </div>
        </div>
        <div className="flex flex-col md:flex-row justify-between items-center gap-6 pt-8 border-t border-border text-xs font-bold uppercase tracking-widest text-muted-foreground/60">
          <p>© {new Date().getFullYear()} ESECAI PLATFORMS INC.</p>
          <div className="flex gap-8">
            <Link href="#" className="hover:text-primary transition-colors">Twitter</Link>
            <Link href="#" className="hover:text-primary transition-colors">Github</Link>
            <Link href="#" className="hover:text-primary transition-colors">LinkedIn</Link>
          </div>
        </div>
      </div>
    </footer>
  );
};
