import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { classroomSchema } from "@/schemas/classroom.schema";
import { BookOpen, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import React from "react";
import { UseFormReturn } from "react-hook-form";
import z from "zod";

export default function ClassroomForm({
  form,
}: {
  form: UseFormReturn<z.infer<typeof classroomSchema>>;
}) {
  const fileInputRef = React.useRef<HTMLInputElement>(null);
  const descriptionValue = form.watch("description") || "";

  return (
    <Form {...form}>
      <div className="grid grid-cols-1 gap-6 py-2 md:grid-cols-2">
        {/* Left — Image Upload */}
        <FormField
          control={form.control}
          name="bannerFile"
          render={({ field }) => (
            <FormItem className="flex flex-col">
              <div className="mb-1 flex items-center justify-between">
                <FormLabel>Class banner</FormLabel>
                {!!field.value && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    className="h-7 gap-1 px-2 text-xs"
                    onClick={() => {
                      field.onChange("");
                      if (fileInputRef.current) {
                        fileInputRef.current.value = "";
                      }
                    }}
                  >
                    <X size={12} />
                    Remove
                  </Button>
                )}
              </div>
              <FormControl>
                <label
                  className="flex h-44 flex-1 cursor-pointer flex-col items-center justify-center rounded-xl border-2 border-dashed border-muted-foreground/25 bg-muted/30 transition-colors hover:bg-muted/50 md:h-45"
                  onDragOver={(e) => e.preventDefault()}
                  onDrop={(e) => {
                    e.preventDefault();
                    const file = e.dataTransfer.files[0];
                      if (file) field.onChange(file);
                  }}
                >
                  {field.value ? (
                    <div className="relative w-full h-full rounded-xl overflow-hidden">
                      <img
                        src={
                          typeof field.value === "string"
                            ? field.value
                            : URL.createObjectURL(field.value)
                        }
                        className="absolute inset-0 w-full h-full object-cover rounded-xl"
                        alt="Preview"
                      />
                      <div className="absolute inset-0 bg-black/40 opacity-0 hover:opacity-100 transition-opacity rounded-xl flex items-center justify-center">
                        <p className="text-white text-xs font-medium">
                          Click to change
                        </p>
                      </div>
                    </div>
                  ) : (
                    <div className="flex flex-col items-center gap-2 px-4 text-center">
                      <div className="w-10 h-10 rounded-full bg-muted flex items-center justify-center">
                        <BookOpen size={18} className="text-muted-foreground" />
                      </div>
                      <p className="text-sm font-medium text-muted-foreground">
                        Click or drag & drop
                      </p>
                      <p className="text-xs text-muted-foreground/60">
                        SVG, PNG, JPG (max 800×400px)
                      </p>
                    </div>
                  )}
                  <input
                    type="file"
                    ref={fileInputRef}
                    className="hidden"
                    onChange={(e) => field.onChange(e.target.files?.[0])}
                    accept="image/*"
                  />
                </label>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Right — Text Fields */}
        <div className="flex flex-col gap-4 py-0.5">
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Class name</FormLabel>
                <FormControl>
                  <Input placeholder="e.g. Mathematics 101" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="description"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  Description{" "}
                  <span className="text-muted-foreground font-normal">
                    (optional)
                  </span>
                </FormLabel>
                <FormControl>
                  <Textarea
                    placeholder="Brief overview of the class"
                      rows={5}
                      maxLength={240}
                    {...field}
                  />
                </FormControl>
                <p className="text-right text-xs text-muted-foreground">
                  {descriptionValue.length}/240
                </p>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
      </div>
    </Form>
  );
}
