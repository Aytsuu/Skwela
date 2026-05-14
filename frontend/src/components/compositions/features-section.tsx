import React from "react";
import { Shield, Activity, BarChart3 } from "lucide-react";

export const FeaturesSection = () => {
  return (
    <section id="features" className="py-32 px-8 border-t border-border bg-muted/5">
      <div className="max-w-6xl mx-auto">
        <div className="mb-20">
          <h2 className="text-4xl md:text-5xl font-bold mb-6 tracking-tight">Built for Precision.</h2>
          <p className="text-muted-foreground text-xl max-w-2xl leading-relaxed">
            We&apos;ve stripped away the noise so you can focus on what matters: student success and academic integrity.
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {[
            {
              icon: <Shield className="w-6 h-6" />,
              title: "Secure by Default",
              description: "Enterprise-grade isolation protocols that protect student privacy and exam integrity without compromise."
            },
            {
              icon: <Activity className="w-6 h-6" />,
              title: "Actionable Insights",
              description: "Move beyond simple monitoring. Get deep, AI-driven analysis of engagement and behavioral patterns."
            },
            {
              icon: <BarChart3 className="w-6 h-6" />,
              title: "Automated Reporting",
              description: "Generate comprehensive audit trails and performance summaries with a single click. Save hours of manual work."
            }
          ].map((feature, i) => (
            <div key={i} className="group p-8 border border-border bg-card transition-all duration-300 rounded-2xl shadow-soft">
              <div className="w-14 h-14 rounded-2xl bg-primary/10 text-primary flex items-center justify-center mb-6 group-hover:bg-primary group-hover:text-primary-foreground transition-all duration-300 shadow-glow">
                {feature.icon}
              </div>
              <h3 className="text-xl font-extrabold mb-4">{feature.title}</h3>
              <p className="text-muted-foreground leading-relaxed font-medium">
                {feature.description}
              </p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};
