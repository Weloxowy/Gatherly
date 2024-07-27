jest.mock("next/font/local", () => ({
    Rubik: () => ({
        style: {
            fontFamily: "mocked",
        },
    }),
}));
