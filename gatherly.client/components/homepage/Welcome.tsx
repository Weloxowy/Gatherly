import { Avatar, Button, Text } from "@mantine/core";
import Image from "next/image";
import "./style.css";
import React from "react";
import { motion } from "framer-motion";
import BlurIn from "@/lib/utils/BlurIn";
import Link from "next/link";
import {IconCoffee, IconHeart} from "@tabler/icons-react";

const Welcome: React.FC = () => {
    const MIN_SIZE = 50;
    const MAX_SIZE = 100;
    const MAX_RADIUS = 100; // Maksymalny promień rozproszenia
    const CONTAINER_WIDTH = 1500; // 100% szerokości ekranu
    const CONTAINER_HEIGHT = 200; // Maksymalna wysokość rozproszenia w px

    const getRandomSize = (min: number, max: number) => Math.floor(Math.random() * (max - min + 1)) + min;

    // Generowanie URLi avatarów
    const avatarUrls = Array.from({ length: 15 }, (_, i) => `/avatars/avatar${i + 1}.png`);

    const containerVariants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: {
                staggerChildren: 0.1,
                delayChildren: 0.2,
            },
        },
    };

    const itemVariants = {
        hidden: { opacity: 0, y: 20 },
        visible: {
            opacity: 1,
            y: 0,
            transition: {
                duration: 0.5,
            },
        },
    };

    const imageVariants = {
        hidden: { opacity: 0, y: 20 },
        visible: {
            opacity: 1,
            y: 0,
            transition: {
                type: "spring",
                stiffness: 500,
                damping: 20,
                duration: 0.6,
            },
        },
    };

    const imageContainerVariants = {
        hidden: {},
        visible: {
            transition: {
                staggerChildren: 0.3,
            },
        },
    };

    const chatBubbleContainerVariants = {
        hidden: {},
        visible: {
            transition: {
                staggerChildren: 1,
            },
        },
    };

    const iMessageBubbleVariants = {
        hidden: { opacity: 0, scale: 0.8, y: 20 },
        visible: {
            opacity: 1,
            scale: 1,
            y: 0,
            transition: {
                type: "spring",
                stiffness: 500,
                damping: 20,
                duration: 0.6,
            },
        },
    };

    // Funkcja do generowania pozycji avatarów z uwzględnieniem kolizji
    const getPosition = (index: number, maxAttempts: number = 100) => {
        const radius = MAX_RADIUS;
        const angleStep = 2 * Math.PI / avatarUrls.length;
        const size = getRandomSize(MIN_SIZE, MAX_SIZE);
        let attempts = 0;
        let position = { x: 0, y: 0 };
        let collisionDetected;

        do {
            const angle = index * angleStep;
            const x = radius * Math.cos(angle) + Math.random() * CONTAINER_WIDTH - (CONTAINER_WIDTH / 2);
            const y = radius * Math.sin(angle) + Math.random() * CONTAINER_HEIGHT - (CONTAINER_HEIGHT / 2);
            position = { x, y };

            collisionDetected = false;
            // Sprawdź kolizję z innymi avatarami
            for (let i = 0; i < index; i++) {
                const otherPosition = getPosition(i, 0); // Sprawdź pozycje wcześniejszych avatarów
                const distance = Math.sqrt((position.x - otherPosition.x) ** 2 + (position.y - otherPosition.y) ** 2);
                if (distance < (size + getRandomSize(MIN_SIZE, MAX_SIZE)) / 2) {
                    collisionDetected = true;
                    break;
                }
            }
            attempts++;
        } while (collisionDetected && attempts < maxAttempts);

        return position;
    };

    return (
        <motion.div id={"home"}
                    className="relative z-10 flex flex-col items-center justify-start min-h-screen space-y-6 px-4 pt-32"
                    variants={containerVariants}
                    animate="visible"
        >
            <div>
                <motion.div variants={itemVariants}>
                    <BlurIn
                        word="Planowanie spotkań nigdy nie było tak proste!"
                        className="font-display text-center pt-20 text-5xl font-bold text-white w-full lg:w-auto max-w-5xl mx-auto z-20"
                        duration={1}
                    />
                    <BlurIn
                        word="Przedstawiamy Gatherly"
                        className="font-display text-center text-5xl font-bold text-white w-full xl:w-auto max-w-5xl mx-auto z-20 p-1"
                        duration={2}
                    />
                </motion.div>
                <motion.h2
                    className="text-xl text-white text-opacity-60 tracking-normal text-center max-w-2xl mx-auto z-10 p-3"
                    variants={itemVariants}
                >
                    Zaplanuj spotkanie, zaproś znajomych i wspólnie wybierzcie idealny czas oraz miejsce.
                    Z Gatherly, organizowanie spotkań to sama przyjemność!
                </motion.h2>

                <motion.div variants={itemVariants} className="z-20 text-center">
                    <Button component={Link} href={"/auth"} size="lg" radius="md" color={"dark"}
                            className="shadow-2xl mb-10">
                        Dołącz teraz i zacznij planować!
                    </Button>
                </motion.div>

                <motion.div
                    className="flex justify-center space-x-6"
                    variants={imageContainerVariants}
                    initial="hidden"
                    animate="visible"
                >
                    <motion.div variants={imageVariants} className="flex-shrink-0 p-20 z-0">
                        <div>
                            <Image
                                src={"/screenshotPlaceholder.png"}
                                alt={"Placeholder"}
                                width={1300}
                                height={800}
                                style={{boxShadow: "0 4px 200px rgba(190, 24, 93, 0.5)", borderRadius: "10px"}}
                            />
                        </div>
                    </motion.div>
                </motion.div>
            </div>
            <div id="chat" className="overflow-hidden space-y-10">
                <motion.div
                    className="relative z-10 flex flex-col items-start justify-start min-h-screen space-y-6 px-4 pt-32"
                    variants={itemVariants}
                >
                    <BlurIn
                        word="Pisz, planuj, działaj!"
                        className="font-display text-center text-5xl font-bold w-full lg:w-auto max-w-5xl mx-auto z-20"
                        duration={1}
                    />
                    <motion.h2
                        className="text-xl text-opacity-60 tracking-normal text-center max-w-2xl mx-auto z-10 p-3"
                        variants={itemVariants}
                    >
                        Czat, ustalanie szczegółów, szybkie decyzje – wszystko w jednym miejscu.

                    </motion.h2>
                    <motion.div
                        initial="hidden"
                        animate="visible"
                        variants={chatBubbleContainerVariants}
                    >
                        <motion.h1
                            variants={iMessageBubbleVariants}
                            className="p-6 bg-[#7d62d1] text-[#f4f3f3] text-2xl rounded-xl rounded-br-none my-3 break-words max-w-2xl ml-60"
                        >
                            Cześć! Jak tam przygotowania do spotkania?
                        </motion.h1>
                        <motion.h1
                            variants={iMessageBubbleVariants}
                            className="p-6 bg-[#f4f3f3] text-[#000] text-2xl rounded-xl rounded-bl-none my-3 break-words max-w-2xl mr-60"
                        >
                            Hej, wszystko zgodnie z planem!
                        </motion.h1>
                        <motion.h1
                            variants={iMessageBubbleVariants}
                            className="p-6 bg-[#7d62d1] text-[#f4f3f3] text-2xl rounded-xl rounded-br-none my-3 break-words max-w-2xl ml-60"
                        >
                            Super, już się nie mogę doczekać!
                        </motion.h1>
                        <motion.h1
                            variants={iMessageBubbleVariants}
                            className="p-6 bg-[#f4f3f3] text-[#000] text-2xl rounded-xl rounded-bl-none my-3 break-words max-w-2xl mr-60"
                        >
                            Ja też! To będzie świetne spotkanie.
                        </motion.h1>
                    </motion.div>
                </motion.div>
            </div>
            <div id="plan">
                <motion.div
                    className="relative z-10 flex flex-col items-center justify-start min-h-screen space-y-6 px-4 pt-32"
                    variants={containerVariants}
                >
                    <BlurIn
                        word="Moduł planowania spotkań"
                        className="font-display text-center text-5xl font-bold w-full lg:w-auto max-w-5xl mx-auto z-20"
                        duration={1}
                    />
                    <motion.h2
                        className="text-xl text-opacity-60 tracking-normal text-center max-w-2xl mx-auto z-10 p-3"
                        variants={itemVariants}
                    >
                        Każdy z użytkowników może wybrać pasującą datę spotkania, algorytm zaproponuje kilka najlepszych
                        dat, właściciel spotkania wybiera najlepszą.
                    </motion.h2>
                    <motion.div
                        className="relative w-full h-full flex items-center justify-center"
                        variants={imageVariants}
                        style={{width: "100%", height: "100%"}}
                    >
                        <Image
                            src={"/avatars.webp"}
                            alt={`Avatars`}
                            width={1000}
                            height={500}
                            className="rounded-full"
                            style={{objectFit: "cover"}}
                        />
                    </motion.div>
                </motion.div>
            </div>
            <div id="footer">
                    <motion.p
                        className="text-lg text-center inline-flex"
                    >
                        Made with <IconCoffee className="text-[var(--mantine-primary-color-7)] ml-2 mr-2" stroke={1.5} />by&nbsp;<Link href={"https://github.com/Weloxowy"} target={"_blank"}>Weloxowy </Link>
                    </motion.p>
            </div>
        </motion.div>
    );
};

export default Welcome;
