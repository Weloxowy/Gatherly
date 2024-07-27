"use client"

export function readFromLocalStorage(itemName: string): string | null {
    return localStorage.getItem(itemName);
}
