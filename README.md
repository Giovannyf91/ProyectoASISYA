# Arquitectura – Sistema de Asistencias Vehiculares (ASISYA)

## 1. Introducción

Este documento describe la **arquitectura de microservicios** para el módulo **“Asignación Inteligente de Proveedores”**, parte del sistema de asistencias vehiculares ASISYA.  

Incluye:

- Diagramas C4 Nivel 1–3  
- Bounded contexts  
- Entidades (DDD)  
- Eventos de dominio  
- Contratos (APIs REST y mensajes de eventos)  
- Consideraciones de escalabilidad y seguridad (resumen)

---
## 2. Diagramas C4

### Nivel 1 – System Context

- Clientes: App móvil y App proveedor  
- Plataforma ASISYA: recibe solicitudes, valida ubicación, asigna proveedores, notifica y registra casos  
- Integración con servicios externos: Identity Provider (JWT/OAuth2), Servicio de Mapas, Servicio Push  

**Archivo:** `document\Arquitecture\C4 diagramas.drawio`

---

### Nivel 2 – Container

- Frontend: React (cliente y proveedor)  
- ALB → API Gateway → Microservicios (.NET 8, Docker, ECS)  
- Microservicios: AssistanceRequestService, ProviderOptimizerService, LocationService, NotificationsService  
- Persistencia: RDS PostgreSQL, Redis Cache  
- Comunicación: Message Broker (SQS/EventBridge)  
- Observabilidad: CloudWatch, logging, metrics  

**Archivo:** `document\Arquitecture\C4 diagramas.drawio`

---

### Nivel 3 – Componentes (AssistanceRequestService)
- Integración externa: LocationService, Message Broker, RDS PostgreSQL  

**Archivo:** `document\Arquitecture\C4 diagramas.drawio`

---

## 3. Bounded Contexts

| Contexto                  | Descripción                                                                 |
|----------------------------|---------------------------------------------------------------------------|
| **AssistanceRequest**      | Gestión de solicitudes de asistencia: creación, validación y seguimiento. |
| **ProviderOptimization**   | Algoritmo que selecciona el proveedor óptimo según ETA, disponibilidad y rating. |
| **Notifications**          | Envío de notificaciones a proveedores y clientes.                         |
| **Location**               | Validación y servicios de ubicación geográfica.                           |

---

## 4. Entidades (DDD)

### AssistanceRequest Context

| Entidad             | Descripción                                         | Atributos clave                                 |
|--------------------|---------------------------------------------------|------------------------------------------------|
| **AssistanceRequest** | Solicitud de asistencia vehicular                 | Id, ClienteId, TipoAsistencia, Ubicación, Estado, FechaHoraSolicitud |
| **Cliente**          | Datos del cliente solicitante                      | Id, Nombre, Teléfono, Email                     |
| **AsistenciaEstado** | Enum: Pendiente, EnProceso, Finalizada, Cancelada | —                                              |

### ProviderOptimization Context

| Entidad       | Descripción                               | Atributos clave                               |
|---------------|------------------------------------------|-----------------------------------------------|
| **Proveedor** | Proveedor disponible                       | Id, Nombre, TipoServicio, UbicaciónActual, Rating, EstadoDisponibilidad |
| **Asignación** | Resultado de la selección de proveedor   | Id, AssistanceRequestId, ProveedorId, ETA, FechaHoraAsignación |

### Notifications Context

| Entidad       | Descripción                               | Atributos clave                               |
|---------------|------------------------------------------|-----------------------------------------------|
| **Notificación** | Registro de notificaciones enviadas     | Id, DestinatarioId, TipoNotificación, Estado, FechaHoraEnvio |

### Location Context

| Entidad       | Descripción                               | Atributos clave                               |
|---------------|------------------------------------------|-----------------------------------------------|
| **Ubicación** | Coordenadas geográficas                   | Latitud, Longitud, Dirección, RadioDeCobertura |

---

## 5. Eventos de Dominio

| Evento                   | Descripción                                | Publicador                   | Consumidor                 |
|---------------------------|-------------------------------------------|------------------------------|----------------------------|
| **AssistanceRequested**   | Cliente crea una solicitud                 | AssistanceRequestService     | ProviderOptimizerService    |
| **ProviderAssigned**      | Proveedor asignado a solicitud            | ProviderOptimizerService     | NotificationsService        |
| **NotificationSent**      | Notificación enviada                       | NotificationsService         | Auditoría / opcional       |
| **LocationValidated**     | Ubicación del cliente validada            | LocationService              | AssistanceRequestService   |

**Ejemplo de evento JSON: AssistanceRequested**

```json
{
  "requestId": "uuid-1234",
  "clientId": "uuid-5678",
  "type": "Grua",
  "location": {
    "lat": -34.6037,
    "lng": -58.3816
  },
  "timestamp": "2026-02-24T15:00:00Z"
}

```
## 6. Contratos / APIs REST

| Servicio                  | Endpoint                         | Método | Descripción                                |
|---------------------------|---------------------------------|--------|--------------------------------------------|
| AssistanceRequestService  | `/assistance-requests`           | POST   | Crear nueva solicitud                       |
| AssistanceRequestService  | `/assistance-requests/{id}`      | GET    | Obtener estado y seguimiento               |
| ProviderOptimizerService  | `/providers/optimal`             | POST   | Obtener proveedor óptimo para la solicitud |
| NotificationsService      | `/notifications/send`            | POST   | Enviar notificación                        |

---

## 7. Notas de Arquitectura

- Microservicios desacoplados, **event-driven** mediante **Message Broker (SQS / EventBridge)**  
- Base de datos **RDS PostgreSQL por servicio**  
- **Cache Redis** para optimización de geolocalización y ETA  
- **API Gateway** con JWT, rate limiting y logging centralizado  
- Escalabilidad: Auto Scaling en ECS, procesamiento asíncrono, retries y fallback  
- Seguridad: IAM Roles, JWT, WAF, Secrets Manager, auditoría, OWASP  
- Observabilidad: CloudWatch, logs, métricas y tracing distribuido

---

## 8. Estrategia de Escalabilidad

- **Microservicios:** contenedores Docker orquestados en ECS/Kubernetes para escalabilidad horizontal  
- **Auto Scaling:** ajuste dinámico según métricas de CPU, memoria o latencia  
- **Colas y Pub/Sub:** uso de SQS / EventBridge para desacoplar productores y consumidores  
- **Procesamiento asíncrono:** tareas en background para notificaciones y cálculo de proveedores óptimos  
- **Cache Redis:** para resultados frecuentes (geolocalización, ETA) y reducción de consultas a base de datos  
- **Rate limits / throttling:** API Gateway para limitar requests y proteger endpoints  
- **Fallback y retries:** patrón retry con backoff exponencial y fallback ante errores de servicios externos  

---

## 9. Estrategia de Seguridad

- **Autenticación:** JWT + OAuth2 para clientes y proveedores  
- **Autorización:** roles y claims en JWT, gestión de permisos mediante IAM Roles  
- **API Gateway:** control de acceso, rate limiting y logging centralizado  
- **Firewall / WAF:** protección contra ataques OWASP comunes y DDoS básico  
- **Gestión de secretos:** AWS Secrets Manager para credenciales y claves sensibles  
- **Auditoría y trazabilidad:** logging centralizado en CloudWatch y tracing distribuido con X-Ray  
- **Buenas prácticas OWASP:** sanitización de inputs, validación de datos y protección de endpoints  

---

## 10. Checklist de Revisión Técnica (Code Review)

### Buenas prácticas
- Entidades y servicios separados (DDD + Clean Architecture) ✅  
- Controllers sin lógica de negocio (solo exponen endpoints) ✅  
- Logging de eventos y errores implementado ✅  

### Calidad
- Manejo de excepciones parcial ❌ → agregar `try/catch` y retornos HTTP adecuados  
- Validaciones de input incompletas ❌ → asegurar que `TipoAsistencia` y `Ubicacion` no sean nulos  

### Escalabilidad
- Publicación de eventos simulada → reemplazar por SQS / EventBridge real  
- Uso de lista en memoria → reemplazar por RDS/PostgreSQL para persistencia  

### Seguridad
- Falta JWT en endpoints → agregar `[Authorize]`  
- No hay sanitización de datos → validar inputs para proteger de inyección  

### Testing
- Crear Unit Tests para `AssistanceRequestService`  
- Mock del Message Broker para pruebas de integración  

---