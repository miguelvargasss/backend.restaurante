# ?? Guía de Integración Frontend - API Registers

Esta guía detalla todos los endpoints disponibles en el módulo **Registers** del backend y los contratos de datos necesarios para una correcta integración con el frontend.

**Base URL:** `https://localhost:7166/api`  
**Autenticación:** Todos los endpoints requieren JWT Bearer Token

---

## ?? Configuración de Headers

```typescript
const headers = {
  'Authorization': `Bearer ${token}`,
  'Content-Type': 'application/json'
}
```

---

## ?? 1. CATEGORÍAS (`/api/categories`)

### 1.1 GET `/api/categories` - Listar categorías (paginado)

**Query Parameters:**
- `page` (int, default: 1) - Número de página
- `pageSize` (int, default: 10, max: 100) - Elementos por página
- `search` (string, opcional) - Búsqueda por nombre o descripción
- `isActive` (bool, opcional) - Filtrar por estado activo

**Respuesta (200 OK):**
```typescript
{
  categories: [
    {
      id: number,
      name: string,
      description: string | null,
      isActive: boolean,
      productCount: number,
      createdAt: string,  // ISO 8601 DateTime
      updatedAt: string | null
    }
  ],
  total: number,
  page: number,
  pageSize: number
}
```

### 1.2 GET `/api/categories/simple` - Listar categorías activas (sin paginación)

**Respuesta (200 OK):**
```typescript
[
  {
    id: number,
    name: string,
    description: string | null
  }
]
```

### 1.3 GET `/api/categories/{id}` - Obtener categoría por ID

**Respuesta (200 OK):**
```typescript
{
  id: number,
  name: string,
  description: string | null,
  isActive: boolean,
  productCount: number,
  createdAt: string,
  updatedAt: string | null
}
```

**Errores:**
- `404 Not Found`: `{ message: "Categoría no encontrada" }`

### 1.4 POST `/api/categories` - Crear categoría

**Body Request:**
```typescript
{
  name: string,          // Required, max 50 chars
  description?: string,  // Optional, max 200 chars
  isActive: boolean      // Required
}
```

**Respuesta (201 Created):**
```typescript
{
  id: number,
  name: string,
  description: string | null,
  isActive: boolean,
  productCount: 0,
  createdAt: string,
  updatedAt: null
}
```

**Errores:**
- `400 Bad Request`: `{ message: "Ya existe una categoría con ese nombre" }`

### 1.5 PUT `/api/categories/{id}` - Actualizar categoría

**Body Request:**
```typescript
{
  name: string,          // Required, max 50 chars
  description?: string,  // Optional, max 200 chars
  isActive: boolean      // Required
}
```

**Respuesta (200 OK):** Mismo formato que GET por ID

**Errores:**
- `404 Not Found`: `{ message: "Categoría no encontrada" }`
- `400 Bad Request`: `{ message: "Ya existe otra categoría con ese nombre" }`

### 1.6 DELETE `/api/categories/{id}` - Eliminar categoría

**Respuesta (200 OK):**
```typescript
{ message: "Categoría eliminada exitosamente" }
```

**Errores:**
- `404 Not Found`: `{ message: "Categoría no encontrada" }`
- `400 Bad Request`: `{ message: "No se puede eliminar la categoría porque tiene productos asociados. Considere desactivar la categoría en su lugar." }`

### 1.7 PATCH `/api/categories/{id}/toggle-status` - Activar/Desactivar

**Respuesta (200 OK):** Mismo formato que GET por ID

---

## ?? 2. PRODUCTOS (`/api/products`)

### 2.1 GET `/api/products` - Listar productos (paginado)

**Query Parameters:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10, max: 100)
- `search` (string, opcional) - Búsqueda por nombre o descripción
- `isActive` (bool, opcional)
- `categoryId` (int, opcional) - Filtrar por categoría

**Respuesta (200 OK):**
```typescript
{
  products: [
    {
      id: number,
      name: string,
      price: number,
      description: string,
      imageUrl: string | null,
      isActive: boolean,
      categoryId: number,
      categoryName: string | null,
      createdAt: string,
      updatedAt: string | null
    }
  ],
  total: number,
  page: number,
  pageSize: number
}
```

### 2.2 GET `/api/products/{id}` - Obtener producto por ID

**Respuesta (200 OK):**
```typescript
{
  id: number,
  name: string,
  price: number,
  description: string,
  imageUrl: string | null,
  isActive: boolean,
  categoryId: number,
  categoryName: string | null,
  createdAt: string,
  updatedAt: string | null
}
```

### 2.3 POST `/api/products` - Crear producto

**Body Request:**
```typescript
{
  name: string,          // Required, max 100 chars
  price: number,         // Required, > 0.01
  description?: string,  // Optional, max 500 chars
  imageUrl?: string,     // Optional, max 200 chars, must be valid URL
  categoryId: number,    // Required, debe existir
  isActive: boolean      // Required
}
```

**Respuesta (201 Created):** Mismo formato que GET por ID

**Errores:**
- `400 Bad Request`: 
  - `{ message: "La categoría especificada no existe" }`
  - `{ message: "Ya existe un producto con ese nombre" }`

### 2.4 PUT `/api/products/{id}` - Actualizar producto

**Body Request:** Mismo formato que POST

**Respuesta (200 OK):** Mismo formato que GET por ID

**Errores:**
- `404 Not Found`: `{ message: "Producto no encontrado" }`
- `400 Bad Request`: 
  - `{ message: "La categoría especificada no existe" }`
  - `{ message: "Ya existe otro producto con ese nombre" }`

### 2.5 DELETE `/api/products/{id}` - Eliminar producto

**Respuesta (200 OK):**
```typescript
{ message: "Producto eliminado exitosamente" }
```

**Errores:**
- `404 Not Found`: `{ message: "Producto no encontrado" }`
- `400 Bad Request`: `{ message: "No se puede eliminar el producto porque tiene órdenes asociadas. Considere desactivar el producto en su lugar." }`

### 2.6 PATCH `/api/products/{id}/toggle-status` - Activar/Desactivar

**Respuesta (200 OK):** Mismo formato que GET por ID

---

## ?? 3. RESERVAS (`/api/reserves`)

### 3.1 GET `/api/reserves` - Listar reservas (paginado)

**Query Parameters:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10, max: 100)
- `search` (string, opcional) - Búsqueda por nombre o teléfono
- `isActive` (bool, opcional)
- `startDate` (DateTime, opcional) - Filtrar desde fecha
- `endDate` (DateTime, opcional) - Filtrar hasta fecha

**Respuesta (200 OK):**
```typescript
{
  reserves: [
    {
      id: number,
      customerName: string,
      phone: string,
      numberOfPeople: number,
      advancePayment: boolean,
      amount: number,
      reservationDate: string,  // ISO 8601 DateTime
      isActive: boolean,
      tableId: number | null,
      tableName: string | null,
      workerId: number | null,
      workerName: string | null,
      createdAt: string,
      updatedAt: string | null
    }
  ],
  total: number,
  page: number,
  pageSize: number
}
```

### 3.2 GET `/api/reserves/{id}` - Obtener reserva por ID

**Respuesta (200 OK):** Mismo formato que elemento individual de GET

### 3.3 POST `/api/reserves` - Crear reserva

**Body Request:**
```typescript
{
  customerName: string,      // Required, max 100 chars
  phone: string,             // Required, max 9 chars, Phone format
  numberOfPeople: number,    // Required, 1-100
  advancePayment: boolean,   // Required
  amount: number,            // Required, >= 0
  reservationDate: string,   // Required, ISO 8601 DateTime
  tableId?: number,          // Optional, debe existir
  workerId?: number,         // Optional, debe existir
  isActive: boolean          // Required
}
```

**Respuesta (201 Created):** Mismo formato que GET por ID

**Errores:**
- `400 Bad Request`:
  - `{ message: "La mesa especificada no existe" }`
  - `{ message: "El trabajador especificado no existe" }`

### 3.4 PUT `/api/reserves/{id}` - Actualizar reserva

**Body Request:** Mismo formato que POST

**Respuesta (200 OK):** Mismo formato que GET por ID

**Errores:**
- `404 Not Found`: `{ message: "Reserva no encontrada" }`
- `400 Bad Request`:
  - `{ message: "La mesa especificada no existe" }`
  - `{ message: "El trabajador especificado no existe" }`

### 3.5 DELETE `/api/reserves/{id}` - Eliminar reserva

**Respuesta (200 OK):**
```typescript
{ message: "Reserva eliminada exitosamente" }
```

### 3.6 PATCH `/api/reserves/{id}/toggle-status` - Activar/Desactivar

**Respuesta (200 OK):** Mismo formato que GET por ID

---

## ?? 4. SUGERENCIAS (`/api/suggestions`)

### 4.1 GET `/api/suggestions` - Listar sugerencias (paginado)

**Query Parameters:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10, max: 100)
- `search` (string, opcional) - Búsqueda por nombre o detalles
- `isActive` (bool, opcional)

**Respuesta (200 OK):**
```typescript
{
  suggestions: [
    {
      id: number,
      name: string,
      details: string,
      suggestionDate: string,  // ISO 8601 DateTime
      isActive: boolean,
      createdAt: string,
      updatedAt: string | null
    }
  ],
  total: number,
  page: number,
  pageSize: number
}
```

### 4.2 GET `/api/suggestions/{id}` - Obtener sugerencia por ID

**Respuesta (200 OK):** Mismo formato que elemento individual de GET

### 4.3 POST `/api/suggestions` - Crear sugerencia

**Body Request:**
```typescript
{
  name: string,           // Required, max 100 chars
  details: string,        // Required, max 500 chars
  suggestionDate: string, // Required, ISO 8601 DateTime
  isActive: boolean       // Required
}
```

**Respuesta (201 Created):** Mismo formato que GET por ID

### 4.4 PUT `/api/suggestions/{id}` - Actualizar sugerencia

**Body Request:**
```typescript
{
  name: string,           // Required, max 100 chars
  details: string,        // Required, max 500 chars
  isActive: boolean       // Required
}
```

**Respuesta (200 OK):** Mismo formato que GET por ID

### 4.5 DELETE `/api/suggestions/{id}` - Eliminar sugerencia

**Respuesta (200 OK):**
```typescript
{ message: "Sugerencia eliminada exitosamente" }
```

### 4.6 PATCH `/api/suggestions/{id}/toggle-status` - Activar/Desactivar

**Respuesta (200 OK):** Mismo formato que GET por ID

---

## ?? 5. RECLAMOS (`/api/claims`)

### 5.1 GET `/api/claims` - Listar reclamos (paginado)

**Query Parameters:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10, max: 100)
- `search` (string, opcional) - Búsqueda por nombre o detalles
- `isActive` (bool, opcional)

**Respuesta (200 OK):**
```typescript
{
  claims: [
    {
      id: number,
      name: string,
      detail: string,
      claimDate: string,  // ISO 8601 DateTime
      isActive: boolean,
      createdAt: string,
      updatedAt: string | null
    }
  ],
  total: number,
  page: number,
  pageSize: number
}
```

### 5.2 GET `/api/claims/{id}` - Obtener reclamo por ID

**Respuesta (200 OK):** Mismo formato que elemento individual de GET

### 5.3 POST `/api/claims` - Crear reclamo

**Body Request:**
```typescript
{
  name: string,           // Required, max 100 chars
  detail: string,         // Required, max 500 chars
  claimDate: string,      // Required, ISO 8601 DateTime
  isActive: boolean       // Required
}
```

**Respuesta (201 Created):** Mismo formato que GET por ID

### 5.4 PUT `/api/claims/{id}` - Actualizar reclamo

**Body Request:**
```typescript
{
  name: string,           // Required, max 100 chars
  detail: string,         // Required, max 500 chars
  isActive: boolean       // Required
}
```

**Respuesta (200 OK):** Mismo formato que GET por ID

### 5.5 DELETE `/api/claims/{id}` - Eliminar reclamo

**Respuesta (200 OK):**
```typescript
{ message: "Reclamo eliminado exitosamente" }
```

### 5.6 PATCH `/api/claims/{id}/toggle-status` - Activar/Desactivar

**Respuesta (200 OK):** Mismo formato que GET por ID

---

## ?? 6. MESAS (`/api/tables`)

### 6.1 GET `/api/tables` - Listar mesas (paginado)

**Query Parameters:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10, max: 100)
- `search` (string, opcional) - Búsqueda por nombre o ambiente
- `isActive` (bool, opcional)
- `loungeId` (int, opcional) - Filtrar por salón

**Respuesta (200 OK):**
```typescript
{
  tables: [
    {
      id: number,
      name: string,
      environment: string,
      capacity: number,
      isActive: boolean,
      loungeId: number | null,
      loungeName: string | null,
      createdAt: string,
      updatedAt: string | null
    }
  ],
  total: number,
  page: number,
  pageSize: number
}
```

### 6.2 GET `/api/tables/simple` - Listar mesas activas (sin paginación)

**Respuesta (200 OK):**
```typescript
[
  {
    id: number,
    name: string,
    capacity: number,
    environment: string
  }
]
```

### 6.3 GET `/api/tables/{id}` - Obtener mesa por ID

**Respuesta (200 OK):** Mismo formato que elemento individual de GET

### 6.4 POST `/api/tables` - Crear mesa

**Body Request:**
```typescript
{
  name: string,        // Required, max 20 chars
  environment: string, // Required, max 50 chars
  capacity: number,    // Required, 1-50
  loungeId?: number,   // Optional, debe existir
  isActive: boolean    // Required
}
```

**Respuesta (201 Created):** Mismo formato que GET por ID

**Errores:**
- `400 Bad Request`:
  - `{ message: "El salón especificado no existe" }`
  - `{ message: "Ya existe una mesa con ese nombre" }`

### 6.5 PUT `/api/tables/{id}` - Actualizar mesa

**Body Request:** Mismo formato que POST

**Respuesta (200 OK):** Mismo formato que GET por ID

**Errores:**
- `404 Not Found`: `{ message: "Mesa no encontrada" }`
- `400 Bad Request`:
  - `{ message: "El salón especificado no existe" }`
  - `{ message: "Ya existe otra mesa con ese nombre" }`

### 6.6 DELETE `/api/tables/{id}` - Eliminar mesa

**Respuesta (200 OK):**
```typescript
{ message: "Mesa eliminada exitosamente" }
```

**Errores:**
- `404 Not Found`: `{ message: "Mesa no encontrada" }`
- `400 Bad Request`: `{ message: "No se puede eliminar la mesa porque tiene órdenes o reservas asociadas. Considere desactivar la mesa en su lugar." }`

### 6.7 PATCH `/api/tables/{id}/toggle-status` - Activar/Desactivar

**Respuesta (200 OK):** Mismo formato que GET por ID

---

## ?? Ejemplo de Implementación en TypeScript/React

### Configuración del Cliente API

```typescript
import axios from 'axios';

const API_BASE_URL = 'https://localhost:7166/api';

// Crear instancia de axios
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para agregar el token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor para manejar errores
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Redirigir al login
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
```

### Servicio de Categorías

```typescript
import apiClient from './apiClient';

export interface Category {
  id: number;
  name: string;
  description: string | null;
  isActive: boolean;
  productCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateCategoryDto {
  name: string;
  description?: string;
  isActive: boolean;
}

export interface PaginatedResponse<T> {
  categories?: T[];
  products?: T[];
  reserves?: T[];
  suggestions?: T[];
  claims?: T[];
  tables?: T[];
  total: number;
  page: number;
  pageSize: number;
}

export const categoriesService = {
  // Obtener categorías paginadas
  getAll: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
  }): Promise<PaginatedResponse<Category>> => {
    const response = await apiClient.get('/categories', { params });
    return response.data;
  },

  // Obtener categorías simples (sin paginación)
  getSimple: async (): Promise<Array<{id: number, name: string, description: string | null}>> => {
    const response = await apiClient.get('/categories/simple');
    return response.data;
  },

  // Obtener por ID
  getById: async (id: number): Promise<Category> => {
    const response = await apiClient.get(`/categories/${id}`);
    return response.data;
  },

  // Crear categoría
  create: async (data: CreateCategoryDto): Promise<Category> => {
    const response = await apiClient.post('/categories', data);
    return response.data;
  },

  // Actualizar categoría
  update: async (id: number, data: CreateCategoryDto): Promise<Category> => {
    const response = await apiClient.put(`/categories/${id}`, data);
    return response.data;
  },

  // Eliminar categoría
  delete: async (id: number): Promise<{message: string}> => {
    const response = await apiClient.delete(`/categories/${id}`);
    return response.data;
  },

  // Activar/Desactivar
  toggleStatus: async (id: number): Promise<Category> => {
    const response = await apiClient.patch(`/categories/${id}/toggle-status`);
    return response.data;
  }
};
```

### Servicio de Productos

```typescript
import apiClient from './apiClient';

export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  imageUrl: string | null;
  isActive: boolean;
  categoryId: number;
  categoryName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateProductDto {
  name: string;
  price: number;
  description?: string;
  imageUrl?: string;
  categoryId: number;
  isActive: boolean;
}

export const productsService = {
  getAll: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
    categoryId?: number;
  }) => {
    const response = await apiClient.get('/products', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Product> => {
    const response = await apiClient.get(`/products/${id}`);
    return response.data;
  },

  create: async (data: CreateProductDto): Promise<Product> => {
    const response = await apiClient.post('/products', data);
    return response.data;
  },

  update: async (id: number, data: CreateProductDto): Promise<Product> => {
    const response = await apiClient.put(`/products/${id}`, data);
    return response.data;
  },

  delete: async (id: number) => {
    const response = await apiClient.delete(`/products/${id}`);
    return response.data;
  },

  toggleStatus: async (id: number): Promise<Product> => {
    const response = await apiClient.patch(`/products/${id}/toggle-status`);
    return response.data;
  }
};
```

### Servicio de Reservas

```typescript
import apiClient from './apiClient';

export interface Reserve {
  id: number;
  customerName: string;
  phone: string;
  numberOfPeople: number;
  advancePayment: boolean;
  amount: number;
  reservationDate: string;
  isActive: boolean;
  tableId: number | null;
  tableName: string | null;
  workerId: number | null;
  workerName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateReserveDto {
  customerName: string;
  phone: string;
  numberOfPeople: number;
  advancePayment: boolean;
  amount: number;
  reservationDate: string;
  tableId?: number;
  workerId?: number;
  isActive: boolean;
}

export const reservesService = {
  getAll: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
    startDate?: string;
    endDate?: string;
  }) => {
    const response = await apiClient.get('/reserves', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Reserve> => {
    const response = await apiClient.get(`/reserves/${id}`);
    return response.data;
  },

  create: async (data: CreateReserveDto): Promise<Reserve> => {
    const response = await apiClient.post('/reserves', data);
    return response.data;
  },

  update: async (id: number, data: CreateReserveDto): Promise<Reserve> => {
    const response = await apiClient.put(`/reserves/${id}`, data);
    return response.data;
  },

  delete: async (id: number) => {
    const response = await apiClient.delete(`/reserves/${id}`);
    return response.data;
  },

  toggleStatus: async (id: number): Promise<Reserve> => {
    const response = await apiClient.patch(`/reserves/${id}/toggle-status`);
    return response.data;
  }
};
```

### Servicio de Sugerencias (SIMPLIFICADO ?)

```typescript
import apiClient from './apiClient';

export interface Suggestion {
  id: number;
  name: string;
  details: string;
  suggestionDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateSuggestionDto {
  name: string;
  details: string;
  suggestionDate: string;
  isActive: boolean;
}

export interface UpdateSuggestionDto {
  name: string;
  details: string;
  isActive: boolean;
}

export const suggestionsService = {
  getAll: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
  }) => {
    const response = await apiClient.get('/suggestions', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Suggestion> => {
    const response = await apiClient.get(`/suggestions/${id}`);
    return response.data;
  },

  create: async (data: CreateSuggestionDto): Promise<Suggestion> => {
    const response = await apiClient.post('/suggestions', data);
    return response.data;
  },

  update: async (id: number, data: UpdateSuggestionDto): Promise<Suggestion> => {
    const response = await apiClient.put(`/suggestions/${id}`, data);
    return response.data;
  },

  delete: async (id: number) => {
    const response = await apiClient.delete(`/suggestions/${id}`);
    return response.data;
  },

  toggleStatus: async (id: number): Promise<Suggestion> => {
    const response = await apiClient.patch(`/suggestions/${id}/toggle-status`);
    return response.data;
  }
};
```

### Servicio de Reclamos (SIMPLIFICADO ?)

```typescript
import apiClient from './apiClient';

export interface Claim {
  id: number;
  name: string;
  detail: string;
  claimDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateClaimDto {
  name: string;
  detail: string;
  claimDate: string;
  isActive: boolean;
}

export interface UpdateClaimDto {
  name: string;
  detail: string;
  isActive: boolean;
}

export const claimsService = {
  getAll: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
  }) => {
    const response = await apiClient.get('/claims', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Claim> => {
    const response = await apiClient.get(`/claims/${id}`);
    return response.data;
  },

  create: async (data: CreateClaimDto): Promise<Claim> => {
    const response = await apiClient.post('/claims', data);
    return response.data;
  },

  update: async (id: number, data: UpdateClaimDto): Promise<Claim> => {
    const response = await apiClient.put(`/claims/${id}`, data);
    return response.data;
  },

  delete: async (id: number) => {
    const response = await apiClient.delete(`/claims/${id}`);
    return response.data;
  },

  toggleStatus: async (id: number): Promise<Claim> => {
    const response = await apiClient.patch(`/claims/${id}/toggle-status`);
    return response.data;
  }
};
```

### Servicio de Mesas

```typescript
import apiClient from './apiClient';

export interface Table {
  id: number;
  name: string;
  environment: string;
  capacity: number;
  isActive: boolean;
  loungeId: number | null;
  loungeName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateTableDto {
  name: string;
  environment: string;
  capacity: number;
  loungeId?: number;
  isActive: boolean;
}

export const tablesService = {
  getAll: async (params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    isActive?: boolean;
    loungeId?: number;
  }) => {
    const response = await apiClient.get('/tables', { params });
    return response.data;
  },

  getSimple: async () => {
    const response = await apiClient.get('/tables/simple');
    return response.data;
  },

  getById: async (id: number): Promise<Table> => {
    const response = await apiClient.get(`/tables/${id}`);
    return response.data;
  },

  create: async (data: CreateTableDto): Promise<Table> => {
    const response = await apiClient.post('/tables', data);
    return response.data;
  },

  update: async (id: number, data: CreateTableDto): Promise<Table> => {
    const response = await apiClient.put(`/tables/${id}`, data);
    return response.data;
  },

  delete: async (id: number) => {
    const response = await apiClient.delete(`/tables/${id}`);
    return response.data;
  },

  toggleStatus: async (id: number): Promise<Table> => {
    const response = await apiClient.patch(`/tables/${id}/toggle-status`);
    return response.data;
  }
};
```

---

## ?? Manejo de Errores Común

Todos los endpoints pueden retornar estos códigos de error:

### 400 Bad Request
```typescript
{
  message: string  // Descripción específica del error
}
// O validación de ModelState:
{
  type: string,
  title: string,
  status: 400,
  errors: {
    [fieldName: string]: string[]
  }
}
```

### 401 Unauthorized
```typescript
// Sin body, token inválido o expirado
```

### 404 Not Found
```typescript
{
  message: string  // "Recurso no encontrado"
}
```

### 500 Internal Server Error
```typescript
{
  message: string
}
```

---

## ?? Notas Importantes

1. **Fechas:** Todas las fechas deben enviarse en formato ISO 8601: `"2024-02-01T19:00:00"`
2. **Paginación:** Los valores de `page` y `pageSize` tienen límites automáticos
3. **Búsqueda:** No distingue mayúsculas/minúsculas
4. **Token JWT:** Debe incluirse en todas las peticiones después del login
5. **CORS:** El backend permite conexiones desde `http://localhost:5173`
6. **Sugerencias y Reclamos:** Ya NO requieren campos de contacto (email/phone) ni status - fueron simplificados ?

---

## ?? Flujo de Trabajo Recomendado

### 1. Obtener datos para dropdowns:
```typescript
// Cargar categorías para selector
const categories = await categoriesService.getSimple();

// Cargar mesas para selector
const tables = await tablesService.getSimple();
```

### 2. Listar con filtros:
```typescript
// Productos de una categoría específica
const products = await productsService.getAll({
  page: 1,
  pageSize: 20,
  categoryId: 3,
  isActive: true
});
```

### 3. Crear recurso simplificado:
```typescript
try {
  // Crear sugerencia (ahora más simple)
  const newSuggestion = await suggestionsService.create({
    name: "Mejorar el menú",
    details: "Agregar más opciones vegetarianas",
    suggestionDate: new Date().toISOString(),
    isActive: true
  });
  console.log('Sugerencia creada:', newSuggestion);
} catch (error) {
  if (error.response?.status === 400) {
    console.error('Error de validación:', error.response.data);
  }
}
```

### 4. Crear reclamo simplificado:
```typescript
try {
  const newClaim = await claimsService.create({
    name: "Servicio lento",
    detail: "La comida tardó más de 45 minutos",
    claimDate: new Date().toISOString(),
    isActive: true
  });
  toast.success('Reclamo registrado');
} catch (error) {
  if (error.response?.data?.message) {
    toast.error(error.response.data.message);
  }
}
```

---

## ? CAMBIOS IMPORTANTES - v2.0

### Sugerencias (Suggestions):
**? Campos eliminados:**
- `contactEmail`
- `status`

**? Ahora solo requiere:**
- `name` (string, max 100 chars)
- `details` (string, max 500 chars)
- `suggestionDate` (DateTime)
- `isActive` (boolean)

### Reclamos (Claims):
**? Campos eliminados:**
- `contactEmail`
- `contactPhone`
- `status`

**? Ahora solo requiere:**
- `name` (string, max 100 chars)
- `detail` (string, max 500 chars)
- `claimDate` (DateTime)
- `isActive` (boolean)

### Beneficios:
- ? Frontend más simple
- ? Menos validaciones
- ? Formularios más cortos
- ? Mejor UX

---

Esta guía cubre todos los endpoints del módulo Registers actualizado. Para más información sobre autenticación y configuración, consultar la documentación de `/api/auth`.
