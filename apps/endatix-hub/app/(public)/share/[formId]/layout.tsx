import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Endatix Form",
  description: "Generated by Endatix",
};

interface PublicLayoutProps {
  children: React.ReactNode
}

export default function PublicLayout({ children }: PublicLayoutProps) {
  return (
    <html lang="en">
      <body>
        <header></header>
        <main className="grid flex-1 items-start gap-4 p-4 sm:px-6 sm:py-0 md:gap-8">
          {children}
        </main>
      </body>
    </html>
  );
}