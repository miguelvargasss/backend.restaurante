# API Endpoints - Restaurant Management System

## ?? Autenticación

### POST `/api/auth/login`
Login de usuario
```json
Request:
{
  "email": "usuario@example.com",
  "password": "password123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "email": "usuario@example.com",
    "name": "Miguel",
    "firstName": "Miguel",
    "role": "Admin"
  }
}
```

---

## ?? Usuarios - `/api/users`

### GET `/api/users`
Obtener lista paginada de usuarios

**Query Params:**
- `page` (optional): Número de página (default: 1)
- `pageSize` (optional): Cantidad por página (default: 10, max: 100)
- `search` (optional): Búsqueda por nombre, apellido o email
- `isActive` (optional): Filtrar por estado (true/false)
- `profilId` (optional): Filtrar por ID de perfil

**Response:**
```json
{
  "users": [
    {
      "id": 1,
      "name": "Miguel",
      "lastName": "García",
      "email": "miguel@example.com",
      "isActive": true,
      "profilId": 1,
      "profilName": "Administrador",
      "lastLogin": "2024-01-15T10:30:00Z",
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-15T10:30:00Z"
    }
  ],
  "total": 50,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/users/{id}`
Obtener un usuario por ID

**Response:**
```json
{
  "id": 1,
  "name": "Miguel",
  "lastName": "García",
  "email": "miguel@example.com",
  "isActive": true,
  "profilId": 1,
  "profilName": "Administrador",
  "lastLogin": "2024-01-15T10:30:00Z",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

### POST `/api/users`
Crear un nuevo usuario

**Request:**
```json
{
  "name": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "password": "Password123",
  "profilId": 2,
  "isActive": true
}
```

**Response:**
```json
{
  "id": 5,
  "name": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "isActive": true,
  "profilId": 2,
  "profilName": "Mesero",
  "lastLogin": null,
  "createdAt": "2024-01-16T15:20:00Z",
  "updatedAt": null
}
```

### PUT `/api/users/{id}`
Actualizar un usuario existente

**Request:**
```json
{
  "name": "Juan",
  "lastName": "Pérez Actualizado",
  "email": "juan.updated@example.com",
  "profilId": 2,
  "isActive": true,
  "password": "NewPassword123" // opcional
}
```

### DELETE `/api/users/{id}`
Eliminar un usuario

**Response:**
```json
{
  "message": "Usuario eliminado exitosamente"
}
```

**Error si tiene relaciones:**
```json
{
  "message": "No se puede eliminar el usuario porque tiene registros asociados. Considere desactivar el usuario en su lugar."
}
```

### PATCH `/api/users/{id}/toggle-status`
Activar/Desactivar un usuario

**Response:**
```json
{
  "id": 1,
  "name": "Miguel",
  "lastName": "García",
  "email": "miguel@example.com",
  "isActive": false,
  "profilId": 1,
  "profilName": "Administrador",
  "lastLogin": "2024-01-15T10:30:00Z",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-16T16:00:00Z"
}
```

---

## ?? Perfiles - `/api/profiles`

### GET `/api/profiles`
Obtener lista paginada de perfiles

**Query Params:**
- `page` (optional): Número de página (default: 1)
- `pageSize` (optional): Cantidad por página (default: 10, max: 100)
- `search` (optional): Búsqueda por nombre o descripción
- `isActive` (optional): Filtrar por estado (true/false)

**Response:**
```json
{
  "profiles": [
    {
      "id": 1,
      "name": "Administrador",
      "description": "Acceso completo al sistema",
      "createdAt": "2024-01-01",
      "hasAdminAccess": true,
      "isActive": true
    },
    {
      "id": 2,
      "name": "Mesero",
      "description": "Gestión de pedidos y mesas",
      "createdAt": "2024-01-01",
      "hasAdminAccess": false,
      "isActive": true
    }
  ],
  "total": 5,
  "page": 1,
  "pageSize": 10
}
```

### GET `/api/profiles/simple`
Obtener lista simple de perfiles activos (para dropdowns)

**Response:**
```json
[
  {
    "id": 1,
    "name": "Administrador",
    "description": "Acceso completo al sistema"
  },
  {
    "id": 2,
    "name": "Mesero",
    "description": "Gestión de pedidos y mesas"
  }
]
```

### GET `/api/profiles/{id}`
Obtener un perfil por ID

**Response:**
```json
{
  "id": 1,
  "name": "Administrador",
  "description": "Acceso completo al sistema",
  "createdAt": "2024-01-01",
  "hasAdminAccess": true,
  "isActive": true
}
```

### POST `/api/profiles`
Crear un nuevo perfil

**Request:**
```json
{
  "name": "Cocinero",
  "description": "Gestión de cocina y preparación",
  "hasAdminAccess": false,
  "isActive": true
}
```

**Response:**
```json
{
  "id": 6,
  "name": "Cocinero",
  "description": "Gestión de cocina y preparación",
  "createdAt": "2024-01-16",
  "hasAdminAccess": false,
  "isActive": true
}
```

### PUT `/api/profiles/{id}`
Actualizar un perfil existente

**Request:**
```json
{
  "name": "Cocinero",
  "description": "Gestión de cocina y preparación de alimentos",
  "hasAdminAccess": false,
  "isActive": true
}
```

### DELETE `/api/profiles/{id}`
Eliminar un perfil

**Response:**
```json
{
  "message": "Perfil eliminado exitosamente"
}
```

**Error si tiene usuarios:**
```json
{
  "message": "No se puede eliminar el perfil porque tiene usuarios asignados. Considere desactivar el perfil en su lugar."
}
```

### PATCH `/api/profiles/{id}/toggle-status`
Activar/Desactivar un perfil

**Response:**
```json
{
  "id": 2,
  "name": "Mesero",
  "description": "Gestión de pedidos y mesas",
  "createdAt": "2024-01-01",
  "hasAdminAccess": false,
  "isActive": false
}
```

---

## ?? Autenticación en Headers

Todos los endpoints (excepto `/api/auth/login` y `/api/auth/register`) requieren el token JWT en el header:

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

## ?? Ejemplos de Uso con Fetch/Axios

### Ejemplo con Fetch (JavaScript):

```javascript
// Obtener perfiles con paginación
const response = await fetch('https://localhost:7166/api/profiles?page=1&pageSize=10', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
const data = await response.json();
console.log(data);

// Crear un usuario
const newUser = await fetch('https://localhost:7166/api/users', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    name: 'Juan',
    lastName: 'Pérez',
    email: 'juan@example.com',
    password: 'Password123',
    profilId: 2,
    isActive: true
  })
});
const userData = await newUser.json();
console.log(userData);
```

### Ejemplo con Axios (React/TypeScript):

```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7166/api',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});

// Obtener perfiles
const { data } = await api.get('/profiles', {
  params: { page: 1, pageSize: 10 }
});

// Crear usuario
const newUser = await api.post('/users', {
  name: 'Juan',
  lastName: 'Pérez',
  email: 'juan@example.com',
  password: 'Password123',
  profilId: 2,
  isActive: true
});
```
