export function capitalize(text: string): string {
  return text.substring(0, 1).toUpperCase() + text.substring(1, text.length).toLowerCase();
}

export function filterObject<T extends Record<string, unknown>>(obj: T) {
  return Object.fromEntries(
    Object.entries(obj).filter(([, value]) => value !== undefined && value !== null)
  );
}

export function isEmptyObject(obj: Record<string, any>) {
  return Object.keys(obj).length === 0;
}
