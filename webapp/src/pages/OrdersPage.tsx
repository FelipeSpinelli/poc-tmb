import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { OrdersAPI } from "../lib/api";
import type { CreateOrderRequest, Order } from "../types/order";
import { useState } from "react";

export default function OrdersPage() {
  const qc = useQueryClient();
  const [selected, setSelected] = useState<Order | null>(null);

  // paginação
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const ordersQuery = useQuery({
    queryKey: ["orders", page, pageSize],
    queryFn: () => OrdersAPI.list(page, pageSize)
  });

  const createMutation = useMutation({
    mutationFn: (payload: CreateOrderRequest) => OrdersAPI.create(payload),
    onSuccess: async (id) => {
      // Após criar um pedido, volta para página 1 para mostrar o novo
      setPage(1);
      await qc.invalidateQueries({ queryKey: ["orders"] });

      try {
        const order = await OrdersAPI.get(id);
        setSelected(order);
      } catch {
        // opcional: tratar erro
      }
    },
  });

  return (
    <div className="space-y-8">
      {/* FORMULÁRIO */}
      <section className="bg-white rounded-2xl shadow p-6">
        <h2 className="font-semibold mb-4">Novo Pedido</h2>
        <OrderForm
          onSubmit={(data) => createMutation.mutate(data)}
          isSubmitting={createMutation.isPending}
        />
        {createMutation.isError && (
          <p className="text-sm text-red-600 mt-2">
            {(createMutation.error as Error).message}
          </p>
        )}
      </section>

      {/* LISTAGEM COM PAGINAÇÃO */}
      <section className="bg-white rounded-2xl shadow p-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="font-semibold">Pedidos</h2>
          <button
            className="text-sm underline"
            onClick={() => ordersQuery.refetch()}
          >
            Atualizar
          </button>
        </div>

        {ordersQuery.isLoading ? (
          <p>Carregando...</p>
        ) : ordersQuery.isError ? (
          <p className="text-red-600">
            {(ordersQuery.error as Error).message}
          </p>
        ) : (
          <>
            <OrdersTable
              orders={ordersQuery.data ?? []}
              onSelect={(o) => setSelected(o)}
            />

            {/* Controles de paginação */}
            <div className="flex justify-between items-center mt-4">
              <button
                disabled={page === 1}
                onClick={() => setPage((p) => Math.max(p - 1, 1))}
                className="px-3 py-1 rounded border disabled:opacity-50"
              >
                Anterior
              </button>

              <span className="text-sm text-gray-600">
                Página {page}
              </span>

              <button
                disabled={(ordersQuery.data?.length ?? 0) < pageSize}
                onClick={() => setPage((p) => p + 1)}
                className="px-3 py-1 rounded border disabled:opacity-50"
              >
                Próxima
              </button>
            </div>
          </>
        )}
      </section>

      <OrderDetailsModal order={selected} onClose={() => setSelected(null)} />
    </div>
  );
}

function OrderForm({
  onSubmit,
  isSubmitting,
}: {
  onSubmit: (data: CreateOrderRequest) => void;
  isSubmitting?: boolean;
}) {
  const [cliente, setCliente] = useState("");
  const [produto, setProduto] = useState("");
  const [valor, setValor] = useState<string>("");

  const canSubmit =
    cliente.trim() && produto.trim() && !Number.isNaN(Number(valor));

  return (
    <form
      className="grid gap-4 md:grid-cols-3"
      onSubmit={(e) => {
        e.preventDefault();
        if (!canSubmit) return;
        onSubmit({ cliente, produto, valor: Number(valor) });
        setCliente("");
        setProduto("");
        setValor("");
      }}
    >
      <div className="flex flex-col">
        <label className="text-sm text-gray-600">Cliente</label>
        <input
          className="mt-1 rounded-xl border px-3 py-2"
          value={cliente}
          onChange={(e) => setCliente(e.target.value)}
          placeholder="Fulano de Tal"
          required
        />
      </div>

      <div className="flex flex-col">
        <label className="text-sm text-gray-600">Produto</label>
        <input
          className="mt-1 rounded-xl border px-3 py-2"
          value={produto}
          onChange={(e) => setProduto(e.target.value)}
          placeholder="Camiseta"
          required
        />
      </div>

      <div className="flex flex-col">
        <label className="text-sm text-gray-600">Valor</label>
        <input
          className="mt-1 rounded-xl border px-3 py-2"
          value={valor}
          onChange={(e) => setValor(e.target.value)}
          placeholder="99.90"
          type="number"
          step="0.01"
          min="0"
          required
        />
      </div>

      <div className="md:col-span-3">
        <button
          disabled={!canSubmit || isSubmitting}
          className="rounded-xl bg-black text-white px-4 py-2 disabled:opacity-50"
        >
          {isSubmitting ? "Criando..." : "Criar pedido"}
        </button>
      </div>
    </form>
  );
}

function OrdersTable({
  orders,
  onSelect,
}: {
  orders: Order[];
  onSelect: (o: Order) => void;
}) {
  if (orders.length === 0) {
    return <p className="text-gray-600">Nenhum pedido nesta página.</p>;
  }

  return (
    <div className="overflow-auto">
      <table className="min-w-full text-sm">
        <thead>
          <tr className="text-left border-b">
            <th className="py-2 pr-4">Cliente</th>
            <th className="py-2 pr-4">Produto</th>
            <th className="py-2 pr-4">Valor</th>
            <th className="py-2 pr-4">Status</th>
            <th className="py-2 pr-4">Criado em</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((o) => (
            <tr
              key={o.id}
              className="border-b hover:bg-gray-50 cursor-pointer"
              onClick={() => onSelect(o)}
              title="Ver detalhes"
            >
              <td className="py-2 pr-4">{o.cliente}</td>
              <td className="py-2 pr-4">{o.produto}</td>
              <td className="py-2 pr-4">
                {o.valor.toLocaleString(undefined, {
                  style: "currency",
                  currency: "BRL",
                })}
              </td>
              <td className="py-2 pr-4">{o.status}</td>
              <td className="py-2 pr-4">
                {new Date(o.dataCriacao).toLocaleDateString()}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function OrderDetailsModal({
  order,
  onClose,
}: {
  order: Order | null;
  onClose: () => void;
}) {
  if (!order) return null;

  return (
    <div className="fixed inset-0 z-50 bg-black/40 flex items-end md:items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">Detalhes do Pedido</h3>
          <button
            onClick={onClose}
            className="rounded-lg px-3 py-1 border hover:bg-gray-50"
          >
            Fechar
          </button>
        </div>

        <dl className="grid grid-cols-3 gap-3 text-sm">
          <dt className="text-gray-600">ID</dt>
          <dd className="col-span-2 break-all">{order.id}</dd>

          <dt className="text-gray-600">Cliente</dt>
          <dd className="col-span-2">{order.cliente}</dd>

          <dt className="text-gray-600">Produto</dt>
          <dd className="col-span-2">{order.produto}</dd>

          <dt className="text-gray-600">Valor</dt>
          <dd className="col-span-2">
            {order.valor.toLocaleString(undefined, {
              style: "currency",
              currency: "BRL",
            })}
          </dd>

          <dt className="text-gray-600">Status</dt>
          <dd className="col-span-2">{order.status}</dd>

          <dt className="text-gray-600">Criado em</dt>
          <dd className="col-span-2">
            {new Date(order.dataCriacao).toLocaleDateString()}
          </dd>
        </dl>
      </div>
    </div>
  );
}
