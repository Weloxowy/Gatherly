"use client"
import GradientBackground from "@/components/homepage/GradientBackground";
import HomeNav from "@/components/homepage/navigation/HomeNav";
import Welcome from "@/components/homepage/Welcome";

export default function Home() {
    return (
        <main>
            <GradientBackground/>
            <HomeNav/>
            <Welcome />
        </main>
    );
}
