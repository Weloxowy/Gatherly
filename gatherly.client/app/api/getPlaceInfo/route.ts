import { NextResponse } from 'next/server';
import axios from 'axios';

export async function GET(request: Request) {
    const { searchParams } = new URL(request.url);
    const query = searchParams.get('query');

    if (!query) {
        return NextResponse.json({ error: "Query parameter is required" }, { status: 400 });
    }

    try {
        const response = await axios.get('https://api.openrouteservice.org/geocode/search', {
            params: {
                api_key: process.env.MAP_KEY,
                text: query,
            },
        });

        const results = response.data.features.map((item: any) => ({
            value: item.properties.label,
            key: item.properties.gid,
            lat: item.geometry.coordinates[1],
            lon: item.geometry.coordinates[0],
        }));

        const seen = new Set();
        const uniqueResults = results.filter((result: any) => {
            const duplicate = seen.has(result.value);
            seen.add(result.value);
            return !duplicate;
        });

        return NextResponse.json(uniqueResults, { status: 200 });
    } catch (error) {
        console.error("Error fetching place info:", error);
        return NextResponse.json({ error: 'Internal Server Error' }, { status: 500 });
    }
}
