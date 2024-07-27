// ./test-utils/render.tsx
import { render as testingLibraryRender } from '@testing-library/react';
import { MantineProvider } from '@mantine/core';
import React from "react";
import userEvent from '@testing-library/user-event';
import {theme} from "../test/__mocks__/theme";

export function render(ui: React.ReactNode) {
    return testingLibraryRender(<>{ui}</>, {
    wrapper: ({ children }: { children: React.ReactNode }) => (
        <MantineProvider theme={theme}>{children}</MantineProvider>
    ),
});
}

export * from '@testing-library/react';
export { userEvent };
