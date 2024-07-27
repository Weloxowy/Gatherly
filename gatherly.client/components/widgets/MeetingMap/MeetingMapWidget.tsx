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
        iconUrl: '/marker.svg', // Ścieżka do własnego obrazu
        iconSize: [32, 32], // Rozmiar ikony
        iconAnchor: [16, 32], // Punkt kotwiczenia ikony
        popupAnchor: [0, -32] // Punkt kotwiczenia pop-up
    });
    useEffect(() => {
        if (!mapRef.current) {
            mapRef.current = L.map('map').setView([props.lon, props.lat], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; OpenStreetMap contributors'
            }).addTo(mapRef.current);
            L.marker([props.lon, props.lat], { icon: customIcon }).addTo(mapRef.current);
        }

        return () => {
            if (mapRef.current) {
                mapRef.current.remove();
                mapRef.current = null;
            }
        };
    }, [props.lat, props.lon]);

    return <Paper radius="lg" shadow="lg" id="map" style={{ height: '100%', width:"100%", left:0, right: 0, top:0, bottom:0 }}></Paper>;
};


export default MeetingMapWidget;
