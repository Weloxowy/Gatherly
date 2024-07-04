"use client"
import GradientBackground from "@/components/app/GradientBackground";
import HomeNav from "@/components/app/HomeNav/HomeNav";
import Welcome from "@/components/app/Welcome";

export default function Home() {
    return (
        <main>
            <GradientBackground/>
            <HomeNav/>
            <Welcome />
        </main>
    );
}
