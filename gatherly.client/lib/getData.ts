process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0'; // ONLY FOR DEVELOPMENT!


async function getData() {
    const res = await fetch('https://localhost:44329/api/auth/profile', {
        method: 'GET',
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
