import { useEffect, useState } from 'react';

export const useWindowResize = (numberOfComponents: number) => {
    const [width, setWidth] = useState(0);

    useEffect(() => {
        const handleResize = () => {
            setWidth((window.innerWidth * 0.70) / numberOfComponents);
        };

        handleResize();
        window.addEventListener('resize', handleResize);
        return () => {
            window.removeEventListener('resize', handleResize);
        };
    }, [numberOfComponents]);

    return width;
};
