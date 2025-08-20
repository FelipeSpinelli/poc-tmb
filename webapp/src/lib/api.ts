import type { Order, CreateOrderRequest } from "../types/order";

const BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? "").replace(/\/+$/, "");

async function http<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json", ...(init?.headers ?? {}) },
    ...init,
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const OrdersAPI = {
  list: (page_number: number, page_size: number) => http<Order[]>(`/orders?pageNumber=${page_number}&pageSize=${page_size}`),
  get: (id: string) => http<Order>(`/orders/${id}`),
  create: async (payload: CreateOrderRequest) => {
    const obj = await http<unknown>("/orders", {
      method: "POST",
      body: JSON.stringify(payload),
    });
    const anyObj = obj as Record<string, unknown>;
    const id = (anyObj.id ?? anyObj.Id) as string;
    return id;
  },
};
