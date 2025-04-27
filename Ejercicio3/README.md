# Ejercicio 3: Control de tráfico en el puente

## Pregunta Teórica 1: 
### ¿Ventajas e inconvenientes de programar el control de paso por el túnel, en el cliente o en el servidor?

#### Control en el Cliente:
**Ventajas:**
- **Descentralización:** Cada cliente tiene control sobre su estado y puede gestionar el proceso de cruce del puente localmente, reduciendo la carga en el servidor.
- **Escalabilidad:** Al delegar el control en el cliente, el servidor solo tiene que gestionar las solicitudes de conexión y no el procesamiento del flujo de tráfico, lo que puede ser beneficioso si hay muchos vehículos.
  
**Inconvenientes:**
- **Sincronización difícil:** La sincronización entre los diferentes clientes puede volverse compleja. Si los vehículos no se comunican correctamente entre sí, puede haber problemas de concurrencia, como dos vehículos intentado cruzar al mismo tiempo.
- **Dependencia del cliente:** Si un cliente falla o se desconecta, la lógica del tráfico en ese cliente se detendría y podría generar inconsistencias en el sistema, ya que no habría un control centralizado.

#### Control en el Servidor:
**Ventajas:**
- **Control centralizado:** El servidor puede gestionar la lógica de cruce de los vehículos, asegurando que solo un vehículo cruce el puente a la vez. Esto facilita la implementación de reglas de negocio y la sincronización entre los clientes.
- **Menos fallos:** Si el servidor gestiona todo el tráfico, los fallos en un cliente no afectan el control global, ya que el servidor puede mantener la sincronización y el flujo de tráfico.
  
**Inconvenientes:**
- **Mayor carga en el servidor:** Al centralizar todo el control en el servidor, se incrementa su carga de trabajo. Esto puede ser un problema si el número de clientes crece significativamente.
- **Posible cuello de botella:** El servidor podría convertirse en un cuello de botella si no es capaz de manejar múltiples clientes simultáneamente, afectando la latencia y el rendimiento del sistema.

---

## Pregunta Teórica 2: 
### ¿Cómo gestionarías las colas de espera en el servidor? ¿Qué estructura de datos usarías para priorizar vehículos según su dirección? Justifica tu respuesta.

Para gestionar las colas de espera en el servidor, utilizaría **colas (Queues)** para almacenar las solicitudes de paso de los vehículos. La estructura más adecuada sería usar dos colas separadas para los vehículos que se dirigen al **Norte** y al **Sur**. Esta separación de las colas garantizaría que los vehículos en una misma dirección puedan cruzar el puente sin tener que esperar por vehículos en la dirección opuesta.

#### Justificación:

- **Estructura de datos: Queue (FIFO)**: Las colas son estructuras de datos **FIFO (First In, First Out)** que permiten procesar las solicitudes de manera ordenada. Los vehículos que llegan primero son los primeros en cruzar, lo que cumple con el principio de equidad y da un orden lógico a las solicitudes.
  
- **Colas separadas por dirección**: 
  - **Ventaja**: Esta estrategia ayuda a gestionar las direcciones de manera independiente. Si hay vehículos esperando para cruzar hacia el norte, los vehículos en la dirección sur deberán esperar hasta que el puente esté libre.
  - **Desventaja**: En el caso de que haya vehículos en una dirección pero no en la otra, se debe implementar una lógica que permita a los vehículos de la dirección opuesta cruzar el puente si no hay vehículos esperando en su propia cola.

#### Ejemplo de implementación:

```csharp
Queue<Vehiculo> colaNorte = new Queue<Vehiculo>();
Queue<Vehiculo> colaSur = new Queue<Vehiculo>();

// Si el puente está libre y hay vehículos esperando en el Norte, cruza el primero de la cola norte
if (puenteLibre && colaNorte.Count > 0)
{
    Vehiculo vehiculoNorte = colaNorte.Dequeue();
    // Procesar el cruce del vehículo norte
}

// Si el puente está libre y hay vehículos esperando en el Sur, cruza el primero de la cola sur
if (puenteLibre && colaSur.Count > 0)
{
    Vehiculo vehiculoSur = colaSur.Dequeue();
    // Procesar el cruce del vehículo sur
}