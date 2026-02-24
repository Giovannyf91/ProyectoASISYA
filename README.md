# Entregables ASISYA – Squad Técnico

## 4.3. Estándares técnicos del squad

### Convenciones de código

**.NET**
- Clases y métodos en PascalCase, variables locales en camelCase.  
- Aplicar Clean Architecture y principios SOLID en todos los microservicios.  
- Usar `async/await` siempre que haya operaciones de I/O.  
- Validaciones y DTOs para todos los endpoints públicos.  

**React**
- Componentes funcionales y hooks.  
- JSX con indentación de 2 espacios.  
- Manejo de estado mínimo con Context o Redux según la complejidad.  

**Docker**
- Construcciones optimizadas con `multi-stage builds`.  
- Nunca incluir secretos en las imágenes.  
- Contenedores ligeros y reproducibles.  

---

### Políticas del squad

- **Branch strategy:** `main` protegido, `feature/*` para nuevas funcionalidades, `hotfix/*` para correcciones urgentes.  
- **Code reviews:** obligatorio al menos 1 reviewer; incluir checklist de seguridad y estilo.  
- **CI/CD:** pipeline automatizado en GitHub Actions: build, tests, lint, docker build.  
- **Definition of Done:** código funcional, tests con cobertura > 80%, documentación actualizada.  
- **Secret management:** usar AWS Secrets Manager o `.env` no versionado.  
- **Arquitectura base:** microservicios desacoplados, event-driven, clean architecture.  
- **Control de deuda técnica:** issues etiquetados y revisiones periódicas para mantener calidad.  

---

## 4.4. Revisión de código – AssignProvider

### Código original defectuoso

```csharp
[HttpPost("assign")]
public async Task<IActionResult> AssignProvider(Request request)
{
    var providers = await _db.Providers.ToListAsync();

    var selected = providers.FirstOrDefault(); // escoger el primero nomás
    if(selected == null) return BadRequest("No providers");

    selected.IsBusy = true;
    _db.SaveChanges();

    return Ok(selected);
}
```

### Problemas detectados

1. **Selección de proveedor poco realista**  
   Siempre se elige el primer proveedor sin evaluar disponibilidad, ubicación, ETA o rating.  

2. **Falta de validaciones**  
   No se comprueba que `request` sea válido ni que `ServiceType` esté presente.  

3. **Riesgo de concurrencia**  
   Solicitudes simultáneas podrían asignar el mismo proveedor a varios clientes.  

4. **Sin transacciones**  
   `_db.SaveChanges()` se ejecuta directamente, dejando el sistema vulnerable a inconsistencias si falla algo en medio.  

5. **Mezcla de responsabilidades**  
   El Controller realiza tanto la selección del proveedor como la persistencia, violando SOLID.  

6. **Exposición directa de entidades**  
   Retorna la entidad `Provider` completa, lo que puede filtrar información sensible.  

7. **Uso incorrecto de async**  
   `_db.SaveChanges()` es síncrono dentro de un método `async`, reduciendo escalabilidad.  

8. **Lógica incompleta**  
   No aplica criterios de optimización para elegir al proveedor más adecuado.  

---

### Código sugerido

```csharp
[HttpPost("assign")]
public async Task<IActionResult> AssignProviderAsync([FromBody] AssignProviderRequest request)
{
    if(request == null || string.IsNullOrWhiteSpace(request.ServiceType))
        return BadRequest("ServiceType es obligatorio");

    var provider = await _providerUseCase.GetOptimalProviderAsync(request.ServiceType);
    if(provider == null) return NotFound("No hay proveedores disponibles");
    
    using var transaction = await _db.Database.BeginTransactionAsync();
    provider.IsBusy = true;
    _db.Providers.Update(provider);
    await _db.SaveChangesAsync();
    await transaction.CommitAsync();
  
    var result = new ProviderDto { Id = provider.Id, Name = provider.Name };
    return Ok(result);
}
```

### Mejoras aplicadas

- ✅ **Async/await correctamente implementado**  
  El método ahora es completamente asíncrono, incluyendo `SaveChangesAsync`, evitando bloqueos innecesarios.  

- ✅ **Transacción y control de concurrencia**  
  Uso de `BeginTransactionAsync()` para asegurar consistencia si falla alguna operación durante la asignación del proveedor.  

- ✅ **Validaciones de entrada**  
  Se verifica que el `request` no sea nulo y que `ServiceType` sea obligatorio antes de procesar la solicitud.  

- ✅ **Uso de DTO para salida segura**  
  El controller ya no retorna la entidad `Provider` directamente; se expone un `ProviderDto` con los datos mínimos necesarios.  

- ✅ **Separación de responsabilidades (SOLID)**  
  La lógica de selección de proveedor se delega al UseCase (`_providerUseCase`), manteniendo el Controller limpio.  

- ✅ **Política de selección extensible**  
  Ahora se puede implementar selección óptima considerando disponibilidad, rating, ubicación, y otros criterios, en lugar de tomar siempre el primero.  