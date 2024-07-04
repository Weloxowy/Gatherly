import React from 'react';

const GradientBackground: React.FC = () => {
    return (
<>
        <style>
            {`
          .body {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            opacity: 0.35;
            background: linear-gradient(
              to bottom,
              var(--mantine-color-violet-6),
              var(--mantine-color-indigo-2),
              transparent
            );
            z-index: -1;
          }
        `}
        </style>
    <div className="body" />
</>
)
    ;
};

export default GradientBackground;
