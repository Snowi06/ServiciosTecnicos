--Si la base esxite

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'servicios_tecnicos_db')
BEGIN
    ALTER DATABASE servicios_tecnicos_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE servicios_tecnicos_db;
END
GO
--Creacion de la base de datos
CREATE DATABASE servicios_tecnicos_db;
GO

USE servicios_tecnicos_db;
GO

--Tabla roles

CREATE TABLE roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(30) NOT NULL UNIQUE,
    created_at DATETIME DEFAULT GETDATE()
);


--Tabla users

CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    role_id INT NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_users_roles
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);


--Tabla clientes

CREATE TABLE clients (
    client_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT UNIQUE NOT NULL,
    address VARCHAR(200),
    city VARCHAR(100),

    CONSTRAINT fk_clients_users
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);


--Tabala technicians(tecnicos)

CREATE TABLE technicians (
    technician_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT UNIQUE NOT NULL,
    experience_years INT DEFAULT 0,
    average_rating DECIMAL(3,2) DEFAULT 0,
    available BIT DEFAULT 1,

    CONSTRAINT fk_technicians_users
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);


--Tabla categorias de servicios(service categories)

CREATE TABLE service_categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(200)
);


--Tabla specialites(Especialidades)

CREATE TABLE specialties (
    specialty_id INT IDENTITY(1,1) PRIMARY KEY,
    category_id INT NOT NULL,
    specialty_name VARCHAR(100) NOT NULL,

    CONSTRAINT fk_specialties_categories
    FOREIGN KEY (category_id) REFERENCES service_categories(category_id)
);


--Tabla technicians specialities(especialidad de tecnicos)

CREATE TABLE technician_specialties (
    technician_id INT NOT NULL,
    specialty_id INT NOT NULL,

    PRIMARY KEY (technician_id, specialty_id),

    CONSTRAINT fk_ts_technicians
    FOREIGN KEY (technician_id) REFERENCES technicians(technician_id),

    CONSTRAINT fk_ts_specialties
    FOREIGN KEY (specialty_id) REFERENCES specialties(specialty_id)
);


--Tabla service request(Servicios/Solicitar pedidos)

CREATE TABLE service_requests (
    request_id INT IDENTITY(1,1) PRIMARY KEY,
    client_id INT NOT NULL,
    category_id INT NOT NULL,

    title VARCHAR(150) NOT NULL,
    description VARCHAR(500) NOT NULL,

    address VARCHAR(200) NOT NULL,

    request_status VARCHAR(30) DEFAULT 'pending',

    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_requests_clients
    FOREIGN KEY (client_id) REFERENCES clients(client_id),

    CONSTRAINT fk_requests_categories
    FOREIGN KEY (category_id) REFERENCES service_categories(category_id)
);


--Tabla solicitudes asignadas

CREATE TABLE request_assignments (
    assignment_id INT IDENTITY(1,1) PRIMARY KEY,
    request_id INT UNIQUE NOT NULL,
    technician_id INT NOT NULL,

    assigned_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_assignments_requests
    FOREIGN KEY (request_id) REFERENCES service_requests(request_id),

    CONSTRAINT fk_assignments_technicians
    FOREIGN KEY (technician_id) REFERENCES technicians(technician_id)
);


--Tabla services(servicios historial)
CREATE TABLE services (
    service_id INT IDENTITY(1,1) PRIMARY KEY,
    request_id INT UNIQUE NOT NULL,
    technician_id INT NOT NULL,

    start_date DATETIME,
    end_date DATETIME,

    final_status VARCHAR(30) DEFAULT 'completed',

    CONSTRAINT fk_services_requests
    FOREIGN KEY (request_id) REFERENCES service_requests(request_id),

    CONSTRAINT fk_services_technicians
    FOREIGN KEY (technician_id) REFERENCES technicians(technician_id)
);


--tabla ratings(Calificaciones)

CREATE TABLE ratings (
    rating_id INT IDENTITY(1,1) PRIMARY KEY,

    service_id INT UNIQUE NOT NULL,

    score INT NOT NULL CHECK (score BETWEEN 1 AND 5),

    comment VARCHAR(500),

    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT fk_ratings_services
    FOREIGN KEY (service_id) REFERENCES services(service_id)
);



--Inserciones de datos iniciales(opcionales a eliminar)

INSERT INTO roles (role_name)
VALUES
('admin'),
('client'),
('technician');


INSERT INTO service_categories (category_name, description)
VALUES
('electricidad', 'Servicios eléctricos'),
('plomeria', 'Servicios de plomería'),
('refrigeracion', 'Aires acondicionados'),
('mecanica', 'Reparaciones mecánicas'),
('electrodomesticos', 'Lavadoras, refrigeradoras');


INSERT INTO specialties (category_id, specialty_name)
VALUES
(1, 'instalacion_electrica'),
(1, 'mantenimiento_electrico'),
(2, 'reparacion_tuberias'),
(3, 'aire_acondicionado'),
(5, 'reparacion_lavadora');



select * from specialties;

select * from service_categories;

select * from roles;


-- =========================
-- Tecnico 1
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Pedro', 'Gonzalez', 'pedro.gonzalez@email.com', 'hash123', '7100-0001');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 5, 4.5, 1);


-- =========================
-- Tecnico 2
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Jose', 'Castro', 'jose.castro@email.com', 'hash123', '7100-0002');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 8, 4.8, 1);


-- =========================
-- Tecnico 3
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Luis', 'Mendoza', 'luis.mendoza@email.com', 'hash123', '7100-0003');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 3, 4.2, 1);


-- =========================
-- Tecnico 4
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Ricardo', 'Vargas', 'ricardo.vargas@email.com', 'hash123', '7100-0004');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 10, 4.9, 1);


-- =========================
-- TECNICO 5
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Mario', 'Rivas', 'mario.rivas@email.com', 'hash123', '7100-0005');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 6, 4.6, 1);


-- =========================
-- Tecnico 6
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Carlos', 'Navarro', 'carlos.navarro@email.com', 'hash123', '7100-0006');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 4, 4.3, 1);


-- =========================
-- Tecnico 7
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Fernando', 'Lopez', 'fernando.lopez@email.com', 'hash123', '7100-0007');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 7, 4.7, 1);


-- =========================
-- TECNICO 8
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (3, 'Oscar', 'Reyes', 'oscar.reyes@email.com', 'hash123', '7100-0008');

INSERT INTO technicians (user_id, experience_years, average_rating, available)
VALUES (SCOPE_IDENTITY(), 2, 4.0, 1);

-- Cliente 1
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Juan', 'Perez', 'juan.perez@email.com', 'hash123', '7000-0001');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Colonia Escalon', 'San Salvador');


-- =========================
-- Cliente 2
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Maria', 'Lopez', 'maria.lopez@email.com', 'hash123', '7000-0002');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Santa Elena', 'La Libertad');


-- =========================
-- Cliente 3
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Carlos', 'Ramirez', 'carlos.ramirez@email.com', 'hash123', '7000-0003');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Soyapango Centro', 'San Salvador');


-- =========================
-- Cliente 4
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Ana', 'Martinez', 'ana.martinez@email.com', 'hash123', '7000-0004');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Mejicanos Norte', 'San Salvador');


-- =========================
-- Cliente 5
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Luis', 'Hernandez', 'luis.hernandez@email.com', 'hash123', '7000-0005');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Apopa Sur', 'San Salvador');


-- =========================
-- Cliente 6
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Sofia', 'Gomez', 'sofia.gomez@email.com', 'hash123', '7000-0006');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Antiguo Cuscatlan', 'La Libertad');


-- =========================
-- Cliente 7
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Miguel', 'Torres', 'miguel.torres@email.com', 'hash123', '7000-0007');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'San Marcos', 'San Salvador');


-- =========================
-- Cliente 8
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Valeria', 'Cruz', 'valeria.cruz@email.com', 'hash123', '7000-0008');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Zaragoza', 'La Libertad');


-- =========================
-- Cliente 9
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Andres', 'Morales', 'andres.morales@email.com', 'hash123', '7000-0009');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Ilopango', 'San Salvador');


-- =========================
-- Cliente 10
-- =========================
INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone)
VALUES (2, 'Daniela', 'Flores', 'daniela.flores@email.com', 'hash123', '7000-0010');

INSERT INTO clients (user_id, address, city)
VALUES (SCOPE_IDENTITY(), 'Santa Tecla', 'La Libertad');

-- =========================
-- Admin 1
-- =========================

INSERT INTO users (role_id, first_name, last_name, email, password_hash, phone, is_active)
VALUES (
    (SELECT role_id FROM roles WHERE role_name = 'admin'),
    'Admin',
    'Sistema',
    'admin@email.com',
    'hash123',
    '5555-0000',
    1
)

--Para saber el nombre de tu servior de sql server
SELECT @@SERVERNAME 
