export default function saveTokenToLocalStorage(itemName: string,  token: string): void {
    localStorage.setItem(itemName, token);
}
