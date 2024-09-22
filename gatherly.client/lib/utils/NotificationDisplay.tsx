import { useEffect } from 'react';
import { getNotifications, clearNotifications } from './notificationsManager';
import { showNotification } from '@mantine/notifications';
import {Notifiaction} from "@/lib/interfaces/types";

const NotificationsDisplay = () => {
    useEffect(() => {
        const notifications : Notifiaction[] = getNotifications();
        notifications.forEach(notification => {
            showNotification({
                title: notification.title,
                message: notification.message,
                color: notification.color,
                autoClose: 5000,
            });
        });
        clearNotifications();
    }, []);

    return null;
};

export default NotificationsDisplay;
