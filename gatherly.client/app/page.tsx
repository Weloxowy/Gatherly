"use client"
import HomeNav from "@/components/homepage/navigation/HomeNav";
import Welcome from "@/components/homepage/Welcome";

export default function Home() {
    return (
        <main>
            <div
                className="z-0 relative min-h-screen w-full pb-40 overflow-hidden bg-[radial-gradient(97.14%_56.45%_at_51.63%_0%,_#7D56F4_0%,_#4517D7_30%,transparent_100%)]">
                <HomeNav/>
                <Welcome/>
            </div>
        </main>
);
}
