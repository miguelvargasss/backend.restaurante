# API Endpoints - Módulo de Registros

## ?? Tabla de Contenidos
1. [Categorías](#-categorías)
2. [Productos (Carta)](#-productos-carta)
3. [Reservas](#-reservas)
4. [Sugerencias](#-sugerencias)
5. [Reclamos](#-reclamos)
6. [Mesas](#-mesas)

---

## ??? Categorías

### GET `/api/categories`
Obtener lista paginada de categorías

**Query Params:**
- `page` (optional): Número de página (default: 1)
- `pageSize` (optional): Cantidad por página (default: 10, max: 100)
- `search` (optional): Búsqueda por nombre o descripción
- `isActive` (optional): Filtrar por estado (true/false)

**Response:**
```json
{
  "categories": [
    {
      "id": 1,
      "name": "Entradas",
      "description": "Platos de entrada",
      "isActive": true,
      "productCount": 15,
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "total": 10,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/categories/simple`
Lista simple de categorías activas para dropdowns

**Response:**
```json
[
  {
    "id": 1,
    "name": "Entradas",
    "description": "Platos de entrada"
  }
]
```

### GET `/api/categories/{id}`
Obtener una categoría por ID

### POST `/api/categories`
Crear nueva categoría

**Request:**
```json
{
  "name": "Postres",
  "description": "Postres caseros",
  "isActive": true
}
```

### PUT `/api/categories/{id}`
Actualizar categoría

### DELETE `/api/categories/{id}`
Eliminar categoría (solo si no tiene productos asociados)

### PATCH `/api/categories/{id}/toggle-status`
Activar/Desactivar categoría

---

## ??? Productos (Carta)

### GET `/api/products`
Obtener lista paginada de productos

**Query Params:**
- `page` (optional): Número de página
- `pageSize` (optional): Cantidad por página
- `search` (optional): Búsqueda por nombre o descripción
- `isActive` (optional): Filtrar por estado
- `categoryId` (optional): Filtrar por categoría

**Response:**
```json
{
  "products": [
    {
      "id": 1,
      "name": "Ceviche",
      "price": 25.50,
      "description": "Ceviche de pescado fresco",
      "imageUrl": "https://example.com/ceviche.jpg",
      "isActive": true,
      "categoryId": 1,
      "categoryName": "Entradas",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "total": 50,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/products/{id}`
Obtener un producto por ID

### POST `/api/products`
Crear nuevo producto

**Request:**
```json
{
  "name": "Lomo Saltado",
  "price": 32.00,
  "description": "Lomo saltado con papas fritas",
  "imageUrl": "https://example.com/lomo.jpg",
  "categoryId": 2,
  "isActive": true
}
```

### PUT `/api/products/{id}`
Actualizar producto

### DELETE `/api/products/{id}`
Eliminar producto (solo si no tiene órdenes)

### PATCH `/api/products/{id}/toggle-status`
Activar/Desactivar producto

---

## ?? Reservas

### GET `/api/reserves`
Obtener lista paginada de reservas

**Query Params:**
- `page` (optional)
- `pageSize` (optional)
- `search` (optional): Búsqueda por nombre o teléfono
- `isActive` (optional): Filtrar por estado
- `startDate` (optional): Fecha inicial
- `endDate` (optional): Fecha final

**Response:**
```json
{
  "reserves": [
    {
      "id": 1,
      "customerName": "Juan Pérez",
      "phone": "987654321",
      "numberOfPeople": 4,
      "advancePayment": true,
      "amount": 50.00,
      "reservationDate": "2024-02-01T19:00:00Z",
      "isActive": true,
      "tableId": 5,
      "tableName": "Mesa 5",
      "workerId": 2,
      "workerName": "María García",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "total": 25,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/reserves/{id}`
Obtener una reserva por ID

### POST `/api/reserves`
Crear nueva reserva

**Request:**
```json
{
  "customerName": "Juan Pérez",
  "phone": "987654321",
  "numberOfPeople": 4,
  "advancePayment": true,
  "amount": 50.00,
  "reservationDate": "2024-02-01T19:00:00",
  "tableId": 5,
  "workerId": 2,
  "isActive": true
}
```

### PUT `/api/reserves/{id}`
Actualizar reserva

### DELETE `/api/reserves/{id}`
Eliminar reserva

### PATCH `/api/reserves/{id}/toggle-status`
Activar/Desactivar reserva

---

## ?? Sugerencias

### GET `/api/suggestions`
Obtener lista paginada de sugerencias

**Query Params:**
- `page` (optional)
- `pageSize` (optional)
- `search` (optional)
- `isActive` (optional)
- `status` (optional): "Pendiente", "En Revisión", "Completada"

**Response:**
```json
{
  "suggestions": [
    {
      "id": 1,
      "name": "Mejorar atención",
      "details": "Sería bueno tener más meseros en horario punta",
      "contactEmail": "cliente@example.com",
      "status": "Pendiente",
      "suggestionDate": "2024-01-15T10:30:00Z",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "total": 15,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/suggestions/{id}`
Obtener una sugerencia por ID

### POST `/api/suggestions`
Crear nueva sugerencia

**Request:**
```json
{
  "name": "Ampliar menú vegetariano",
  "details": "Agregar más opciones vegetarianas al menú",
  "contactEmail": "cliente@example.com",
  "suggestionDate": "2024-01-15T10:30:00",
  "isActive": true
}
```

### PUT `/api/suggestions/{id}`
Actualizar sugerencia

### DELETE `/api/suggestions/{id}`
Eliminar sugerencia

### PATCH `/api/suggestions/{id}/toggle-status`
Activar/Desactivar sugerencia

### PATCH `/api/suggestions/{id}/status`
Actualizar solo el estado de la sugerencia

**Request Body:**
```json
"En Revisión"
```

---

## ?? Reclamos

### GET `/api/claims`
Obtener lista paginada de reclamos

**Query Params:**
- `page` (optional)
- `pageSize` (optional)
- `search` (optional)
- `isActive` (optional)
- `status` (optional): "Pendiente", "En Proceso", "Resuelto"

**Response:**
```json
{
  "claims": [
    {
      "id": 1,
      "name": "Demora en atención",
      "detail": "Esperamos 45 minutos por nuestra orden",
      "contactEmail": "cliente@example.com",
      "contactPhone": "987654321",
      "status": "Pendiente",
      "claimDate": "2024-01-15T10:30:00Z",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "total": 8,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/claims/{id}`
Obtener un reclamo por ID

### POST `/api/claims`
Crear nuevo reclamo

**Request:**
```json
{
  "name": "Producto en mal estado",
  "detail": "El pescado del ceviche no estaba fresco",
  "contactEmail": "cliente@example.com",
  "contactPhone": "987654321",
  "claimDate": "2024-01-15T10:30:00",
  "isActive": true
}
```

### PUT `/api/claims/{id}`
Actualizar reclamo

### DELETE `/api/claims/{id}`
Eliminar reclamo

### PATCH `/api/claims/{id}/toggle-status`
Activar/Desactivar reclamo

### PATCH `/api/claims/{id}/status`
Actualizar solo el estado del reclamo

**Request Body:**
```json
"Resuelto"
```

---

## ?? Mesas

### GET `/api/tables`
Obtener lista paginada de mesas

**Query Params:**
- `page` (optional)
- `pageSize` (optional)
- `search` (optional): Búsqueda por nombre o ambiente
- `isActive` (optional): Filtrar por estado
- `loungeId` (optional): Filtrar por salón

**Response:**
```json
{
  "tables": [
    {
      "id": 1,
      "name": "Mesa 1",
      "environment": "Terraza",
      "capacity": 4,
      "isActive": true,
      "loungeId": 1,
      "loungeName": "Salón Principal",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": null
    }
  ],
  "total": 20,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/tables/simple`
Lista simple de mesas activas para dropdowns

**Response:**
```json
[
  {
    "id": 1,
    "name": "Mesa 1",
    "capacity": 4,
    "environment": "Terraza"
  }
]
```

### GET `/api/tables/{id}`
Obtener una mesa por ID

### POST `/api/tables`
Crear nueva mesa

**Request:**
```json
{
  "name": "Mesa 10",
  "environment": "Terraza",
  "capacity": 6,
  "loungeId": 1,
  "isActive": true
}
```

### PUT `/api/tables/{id}`
Actualizar mesa

### DELETE `/api/tables/{id}`
Eliminar mesa (solo si no tiene órdenes o reservas)

### PATCH `/api/tables/{id}/toggle-status`
Activar/Desactivar mesa

---

## ?? Autenticación

Todos los endpoints requieren el token JWT en el header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ?? Códigos de Estado HTTP

- `200 OK`: Operación exitosa
- `201 Created`: Recurso creado exitosamente
- `400 Bad Request`: Error en la validación de datos
- `401 Unauthorized`: No autenticado o token inválido
- `404 Not Found`: Recurso no encontrado
- `500 Internal Server Error`: Error del servidor

---

## ?? Ejemplo de Uso (TypeScript)

```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7166/api',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});

// Obtener productos por categoría
const { data } = await api.get('/products', {
  params: { 
    page: 1, 
    pageSize: 10,
    categoryId: 1,
    isActive: true
  }
});

// Crear una reserva
const newReserve = await api.post('/reserves', {
  customerName: 'Juan Pérez',
  phone: '987654321',
  numberOfPeople: 4,
  advancePayment: true,
  amount: 50.00,
  reservationDate: '2024-02-01T19:00:00',
  tableId: 5,
  isActive: true
});

// Actualizar estado de sugerencia
await api.patch('/suggestions/1/status', '"En Revisión"', {
  headers: { 'Content-Type': 'application/json' }
});
```
