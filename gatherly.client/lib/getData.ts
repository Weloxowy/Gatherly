process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0'; // ONLY FOR DEVELOPMENT!

import { readFromLocalStorage } from "@/lib/auth/headers/readFromLocalStorage";

async function getData() {
    const token = readFromLocalStorage("Authorization");
    if (!token) {
        throw new Error('No token found in localStorage');
    }

    const res = await fetch('https://localhost:44329/api/auth/profile', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': token
        },
        next: {
            revalidate: 60,
        }
    });

    if (!res.ok) {
        throw new Error('Failed to fetch');
    }
    return res.text();
}

export default getData;
