import OrdersPage from "./pages/OrdersPage";

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50 text-gray-900">
      <div className="max-w-5xl mx-auto p-6">
        <header className="mb-6">
          <h1 className="text-2xl font-bold">Order Management System</h1>
          <p className="text-sm text-gray-600">
            Criar, listar e visualizar pedidos
          </p>
        </header>
        <OrdersPage />
      </div>
    </div>
  );
}
