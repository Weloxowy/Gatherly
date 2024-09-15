'use client'
import { createContext, useContext } from 'react';
import { notifications } from '@mantine/notifications';

// Typy powiadomień
interface NotificationPayload {
    title: string;
    message: string;
    type?: 'success' | 'error' | 'info' | 'loading';
    autoClose?: boolean | number;
}

// Kontekst powiadomień
const NotificationsContext = createContext<{
    showNotification: (payload: NotificationPayload) => void;
}>({
    showNotification: () => {},
});

// Hook do używania kontekstu powiadomień
export const useGlobalNotifications = () => useContext(NotificationsContext);

// Provider powiadomień
export const NotificationsProvider = ({ children }: { children: React.ReactNode }) => {
    // Funkcja do wyświetlania powiadomień
    const showNotification = (payload: NotificationPayload) => {
        notifications.show({
            title: payload.title,
            message: payload.message,
            color: payload.type || 'blue',
            autoClose: payload.autoClose ?? 5000, // domyślnie 5 sekund
        });
    };

    return (
        <NotificationsContext.Provider value={{ showNotification }}>
            {children}
        </NotificationsContext.Provider>
    );
};
