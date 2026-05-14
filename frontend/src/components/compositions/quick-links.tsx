import React from "react";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ArrowRight } from "lucide-react";
import Link from "next/link";

export interface QuickLinkItem {
  label: string;
  href: string;
}

export interface QuickLinksProps {
  links: QuickLinkItem[];
}

export const QuickLinks = ({ links }: QuickLinksProps) => {
  return (
    <Card className="border border-border bg-card shadow-soft">
      <CardHeader>
        <CardTitle className="text-xl font-bold tracking-tight">Quick Links</CardTitle>
        <CardDescription className="text-sm font-medium">Commonly used actions.</CardDescription>
      </CardHeader>
      <CardContent className="space-y-2">
        {links.map((link, i) => (
          <Button key={i} variant="ghost" className="w-full justify-between group hover:bg-muted transition-all h-10 px-4" asChild>
            <Link href={link.href}>
              <span className="font-semibold text-sm tracking-tight">{link.label}</span>
              <ArrowRight size={14} className="opacity-0 group-hover:opacity-100 transition-all group-hover:translate-x-1 text-primary" />
            </Link>
          </Button>
        ))}
      </CardContent>
    </Card>
  );
};
