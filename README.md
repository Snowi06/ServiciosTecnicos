# ServiciosTecnicos

Aplicación desarrollada en ASP.NET MVC que permite gestionar solicitudes de servicios técnicos, asignar técnicos disponibles y dar seguimiento al estado del servicio.

---

## Requisitos

Para ejecutar el proyecto se necesita:

- Visual Studio 2022
- SQL Server
- .NET 6 o superior

---

## Instalación

1. Clonar el repositorio:

git clone https://github.com/Snowi06/ServiciosTecnicos.git

2. Abrir el proyecto en Visual Studio 2022:

abrir el archivo:
ServiciosTecnicos.sln

3. Crear la base de datos en SQL Server:

Ejecutar el script .SQL incluido en el proyecto.

Nombre de la base de datos:
servicios_tecnicos_db

4. Configurar la cadena de conexión en el archivo:

   La conexion ya esta establecidad para que funcione al clonar el repositorio, si en caso esta fallara en el proyecto se direje al apartado de la solucion y buscara "appsettings.json",
luego de haberlo encontrado, encontrara esto:
"Server=localhost;Database=servicios_tecnicos_db;Trusted_Connection=True;TrustServerCertificate=True;"

Debera cambiarlo por el nombre especifico de tu servidor, por ejemplo:
"Server=CESAR;Database=servicios_tecnicos_db;Trusted_Connection=True;TrustServerCertificate=True;"

---

## Ejecución del programa

1. Abrir el proyecto en Visual Studio 2022
2. Presionar F5 o el botón Run
3. El sistema se abrirá en el navegador

---

## Funcionalidad implementada

Módulo de Solicitudes que permite:

- Crear solicitudes de servicio
- Ver listado de solicitudes
- Asignar técnicos
- Iniciar servicio
- Eliminar solicitudes
