# ServiciosTecnicos
Aplicación desarrollada en ASP.NET MVC que permite gestionar solicitudes de servicios técnicos, asignar técnicos disponibles y dar seguimiento al estado del servicio.

## Requisitos

Para ejecutar el proyecto se necesita:

- Visual Studio 2022
- SQL Server
- .NET 6 o superior

## Instalación

### 1. Clonar el repositorio

git clone https://github.com/Snowi06/ServiciosTecnicos.git


### 2. Abrir el proyecto

Abrir el archivo:

ServiciosTecnicos.sln

desde Visual Studio 2022.


### 3. Crear la base de datos

Ejecutar el script .SQL incluido en el proyecto.

Nombre de la base de datos:

servicios_tecnicos_db


### 4. Configurar la conexión a la base de datos

La conexión ya está configurada para funcionar automáticamente al clonar el repositorio.

Si la conexión falla:

1. Ir al archivo:

appsettings.json

2. Buscar la cadena de conexión:

Server=localhost;Database=servicios_tecnicos_db;Trusted_Connection=True;TrustServerCertificate=True;

3. Cambiar "localhost" por el nombre de tu servidor SQL.

Ejemplo:

Server=CESAR;Database=servicios_tecnicos_db;Trusted_Connection=True;TrustServerCertificate=True;


## Ejecución del programa

1. Abrir el proyecto en Visual Studio 2022
2. Presionar F5 o el botón Run
3. El sistema se abrirá en el navegador


## Funcionalidad implementada

Módulo de Solicitudes que permite:

- Crear solicitudes de servicio
- Ver listado de solicitudes
- Asignar técnicos
- Iniciar servicio
- Eliminar solicitudes
