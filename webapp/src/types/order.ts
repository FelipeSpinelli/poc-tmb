export type OrderStatus = 'Pendente' | 'Processando' | 'Finalizado';

export interface Order {
  id: string;
  cliente: string;
  produto: string;
  valor: number;
  status: OrderStatus;
  dataCriacao: string;
}

export interface CreateOrderRequest {
  cliente: string;
  produto: string;
  valor: number;
}
