"use client"
import React, {useEffect, useRef} from "react";
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';
import {Paper} from "@mantine/core";

type Props = {
    lon: number,
    lat: number
}

const MeetingMapWidget: React.FC<Props> = (props) => {
    const mapRef = useRef(null);

    const customIcon = new L.Icon({
        iconUrl: '/marker.svg',
        iconSize: [32, 32],
        iconAnchor: [16, 32],
        popupAnchor: [0, -32]
    });

    useEffect(() => {
        if (!mapRef.current) {
            //@ts-ignore
            mapRef.current = L.map('map', { zIndex: 3 }) // Ustaw zIndex mapy
                .setView([props.lon, props.lat], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; OpenStreetMap contributors'
                //@ts-ignore
            }).addTo(mapRef.current).setZIndex(3); // Ustaw zIndex warstwy

            L.marker([props.lon, props.lat], { icon: customIcon })
                //@ts-ignore
                .addTo(mapRef.current)
                .setZIndexOffset(3); // Ustaw zIndex markera
        }

        return () => {
            if (mapRef.current) {
                //@ts-ignore
                mapRef.current.remove();
                mapRef.current = null;
            }
        };
    }, [props.lat, props.lon]);

    return (
        <Paper
            radius="lg"
            shadow="lg"
            id="map"
            style={{
                height: '100%',
                width: "100%",
                left: 0,
                right: 0,
                top: 0,
                bottom: 0,
                zIndex: 3  // Ustawienie zIndex dla samego kontenera mapy
            }}>
        </Paper>
    );
};

export default MeetingMapWidget;
