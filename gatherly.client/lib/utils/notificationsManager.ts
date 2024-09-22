import { showNotification } from '@mantine/notifications';
import {Notifiaction} from "@/lib/interfaces/types";

const NOTIFICATIONS_KEY = 'dashboard_notifications';
export const getNotifications = () => {
    const storedNotifications = sessionStorage.getItem(NOTIFICATIONS_KEY);
    return storedNotifications ? JSON.parse(storedNotifications) : [];
};

export const saveNotifications = (notifications : Notifiaction[]) => {
    sessionStorage.setItem(NOTIFICATIONS_KEY, JSON.stringify(notifications));
};
export const addNotification = (notification : Notifiaction) => {
    const currentNotifications = getNotifications();
    const updatedNotifications = [...currentNotifications, notification];
    saveNotifications(updatedNotifications);
    displayNotification(notification);
};

const displayNotification = (notification:Notifiaction) => {
    showNotification({
        title: notification.title,
        message: notification.message,
        color: notification.color,
        autoClose: 5000,
    });
};
export const clearNotifications = () => {
    sessionStorage.removeItem(NOTIFICATIONS_KEY);
};
