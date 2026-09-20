-- =========================================================
-- Sistema de Gestión de Taller Mecánico
-- Script de creación de base de datos (MySQL 8+)
-- =========================================================

CREATE DATABASE IF NOT EXISTS taller_mecanico
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE taller_mecanico;

-- ---------------------------------------------------------
-- CLIENTE
-- ---------------------------------------------------------
CREATE TABLE cliente (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(120) NOT NULL,
    telefono        VARCHAR(20)  NOT NULL,
    email           VARCHAR(120),
    direccion       VARCHAR(200),
    fecha_registro  DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- ---------------------------------------------------------
-- VEHICULO
-- ---------------------------------------------------------
CREATE TABLE vehiculo (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id      INT NOT NULL,
    placa           VARCHAR(15) NOT NULL UNIQUE,
    marca           VARCHAR(60) NOT NULL,
    modelo          VARCHAR(60) NOT NULL,
    anio            SMALLINT,
    color           VARCHAR(40),
    kilometraje     INT,
    creado_en       DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_vehiculo_cliente
        FOREIGN KEY (cliente_id) REFERENCES cliente(id)
        ON DELETE CASCADE
);

-- ---------------------------------------------------------
-- DANO_VEHICULO (historial de choques / daños previos)
-- ---------------------------------------------------------
CREATE TABLE dano_vehiculo (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    vehiculo_id     INT NOT NULL,
    descripcion     VARCHAR(255) NOT NULL,
    fecha           DATE NOT NULL,
    foto_url        VARCHAR(255),
    CONSTRAINT fk_dano_vehiculo
        FOREIGN KEY (vehiculo_id) REFERENCES vehiculo(id)
        ON DELETE CASCADE
);

-- ---------------------------------------------------------
-- MECANICO
-- ---------------------------------------------------------
CREATE TABLE mecanico (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(120) NOT NULL,
    especialidad    VARCHAR(80),
    telefono        VARCHAR(20),
    activo          BOOLEAN DEFAULT TRUE
);

-- ---------------------------------------------------------
-- ORDEN_TRABAJO
-- Ya no se liga directo al vehículo: se llega a ella a través
-- de SERVICIO (una orden agrupa uno o varios servicios solicitados
-- para un mismo vehículo, con sus mecánicos y materiales).
-- ---------------------------------------------------------
CREATE TABLE orden_trabajo (
    id                    INT AUTO_INCREMENT PRIMARY KEY,
    fecha_ingreso         DATETIME DEFAULT CURRENT_TIMESTAMP,
    estado                ENUM('recibido', 'diagnostico', 'cotizado', 'aprobado',
                                'en_proceso', 'terminado', 'entregado', 'pagado')
                          NOT NULL DEFAULT 'recibido',
    diagnostico           TEXT
);

-- ---------------------------------------------------------
-- SERVICIO (intermediario entre VEHICULO y ORDEN_TRABAJO)
-- Es lo que el cliente solicita para su vehículo; cuando el
-- taller lo atiende, se le asigna una orden de trabajo donde
-- se definen mecánicos, materiales y cotización.
-- ---------------------------------------------------------
CREATE TABLE servicio (
    id                    INT AUTO_INCREMENT PRIMARY KEY,
    vehiculo_id           INT NOT NULL,
    orden_trabajo_id      INT NULL,
    tipo_servicio         ENUM('preventivo', 'correctivo', 'revision_simple') NOT NULL,
    descripcion_solicitud TEXT,
    fecha_solicitud       DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_servicio_vehiculo
        FOREIGN KEY (vehiculo_id) REFERENCES vehiculo(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_servicio_orden
        FOREIGN KEY (orden_trabajo_id) REFERENCES orden_trabajo(id)
        ON DELETE SET NULL
);

-- ---------------------------------------------------------
-- ORDEN_MECANICO (relación N:M orden <-> mecánico)
-- ---------------------------------------------------------
CREATE TABLE orden_mecanico (
    orden_id        INT NOT NULL,
    mecanico_id     INT NOT NULL,
    es_responsable  BOOLEAN DEFAULT FALSE,
    parte_asignada  VARCHAR(150),
    PRIMARY KEY (orden_id, mecanico_id),
    CONSTRAINT fk_om_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_om_mecanico
        FOREIGN KEY (mecanico_id) REFERENCES mecanico(id)
        ON DELETE RESTRICT
);

-- ---------------------------------------------------------
-- COTIZACION (con versiones por orden)
-- ---------------------------------------------------------
CREATE TABLE cotizacion (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    orden_id        INT NOT NULL,
    version         INT NOT NULL DEFAULT 1,
    monto_total     DECIMAL(10,2) NOT NULL DEFAULT 0,
    estado          ENUM('pendiente', 'aprobada', 'rechazada') NOT NULL DEFAULT 'pendiente',
    fecha           DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_cotizacion_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE,
    UNIQUE KEY uq_orden_version (orden_id, version)
);

-- ---------------------------------------------------------
-- COTIZACION_DETALLE
-- ---------------------------------------------------------
CREATE TABLE cotizacion_detalle (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    cotizacion_id   INT NOT NULL,
    descripcion     VARCHAR(200) NOT NULL,
    tipo            ENUM('mano_obra', 'repuesto') NOT NULL,
    monto           DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_detalle_cotizacion
        FOREIGN KEY (cotizacion_id) REFERENCES cotizacion(id)
        ON DELETE CASCADE
);

-- ---------------------------------------------------------
-- PRODUCTO (repuestos / insumos: aceite, grasa, filtros, etc.)
-- ---------------------------------------------------------
CREATE TABLE producto (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(120) NOT NULL,
    categoria       VARCHAR(60),
    precio          DECIMAL(10,2) NOT NULL,
    stock           INT NOT NULL DEFAULT 0
);

-- ---------------------------------------------------------
-- ORDEN_PRODUCTO (repuestos usados en una orden)
-- ---------------------------------------------------------
CREATE TABLE orden_producto (
    orden_id        INT NOT NULL,
    producto_id     INT NOT NULL,
    cantidad        INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (orden_id, producto_id),
    CONSTRAINT fk_op_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_op_producto
        FOREIGN KEY (producto_id) REFERENCES producto(id)
        ON DELETE RESTRICT
);

-- ---------------------------------------------------------
-- VENTA_DIRECTA (venta de productos en mostrador, sin orden)
-- ---------------------------------------------------------
CREATE TABLE venta_directa (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id      INT NULL,
    fecha           DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_venta_cliente
        FOREIGN KEY (cliente_id) REFERENCES cliente(id)
        ON DELETE SET NULL
);

CREATE TABLE venta_directa_detalle (
    venta_id        INT NOT NULL,
    producto_id     INT NOT NULL,
    cantidad        INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (venta_id, producto_id),
    CONSTRAINT fk_vd_venta
        FOREIGN KEY (venta_id) REFERENCES venta_directa(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_vd_producto
        FOREIGN KEY (producto_id) REFERENCES producto(id)
        ON DELETE RESTRICT
);

-- ---------------------------------------------------------
-- PAGO
-- ---------------------------------------------------------
CREATE TABLE pago (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    orden_id        INT NOT NULL,
    monto           DECIMAL(10,2) NOT NULL,
    metodo          ENUM('efectivo', 'tarjeta', 'transferencia') NOT NULL,
    tipo            ENUM('anticipo_25', 'mitad_50', 'total_100', 'saldo_final') NOT NULL,
    fecha           DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_pago_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
);

-- ---------------------------------------------------------
-- MOVIMIENTO_INVENTARIO (kardex: entradas y salidas de stock)
-- ---------------------------------------------------------
CREATE TABLE movimiento_inventario (
    id                INT AUTO_INCREMENT PRIMARY KEY,
    producto_id       INT NOT NULL,
    tipo_movimiento   ENUM('entrada', 'salida') NOT NULL,
    cantidad          INT NOT NULL,
    motivo            ENUM('compra', 'ajuste', 'uso_orden', 'venta_mostrador') NOT NULL,
    referencia_id     INT NULL,
    fecha             DATETIME DEFAULT CURRENT_TIMESTAMP,
    stock_resultante  INT NOT NULL,
    CONSTRAINT fk_movimiento_producto
        FOREIGN KEY (producto_id) REFERENCES producto(id)
        ON DELETE RESTRICT
);

-- ---------------------------------------------------------
-- Índices adicionales útiles para búsquedas frecuentes
-- ---------------------------------------------------------
CREATE INDEX idx_vehiculo_placa ON vehiculo(placa);
CREATE INDEX idx_cliente_telefono ON cliente(telefono);
CREATE INDEX idx_orden_estado ON orden_trabajo(estado);
CREATE INDEX idx_servicio_vehiculo ON servicio(vehiculo_id);
CREATE INDEX idx_servicio_orden ON servicio(orden_trabajo_id);
CREATE INDEX idx_movimiento_producto ON movimiento_inventario(producto_id);
CREATE INDEX idx_movimiento_fecha ON movimiento_inventario(fecha);
-- =========================================================
-- Datos de ejemplo (inventados) para taller_mecanico
-- Respeta el orden de dependencias de las FK
-- =========================================================

USE taller_mecanico;

-- ---------------------------------------------------------
-- CLIENTE
-- ---------------------------------------------------------
INSERT INTO cliente (id, nombre, telefono, email, direccion) VALUES
(1, 'Carlos Mamani',   '71234567', 'carlos.mamani@gmail.com',   'Av. Heroínas 123, Cochabamba'),
(2, 'María Fernández', '70112233', 'maria.fernandez@hotmail.com','Calle España 456, Cochabamba'),
(3, 'Jorge Quispe',    '76543210', 'jorge.quispe@gmail.com',    'Av. América 789, Cochabamba'),
(4, 'Lucía Rocha',     '78912345', 'lucia.rocha@gmail.com',     'Calle Bolívar 234, Cochabamba'),
(5, 'Pedro Flores',    '79988776', 'pedro.flores@gmail.com',    'Av. Blanco Galindo 1500, Cochabamba'),
(6, 'Ana Castro',      '75544332', 'ana.castro@gmail.com',      'Calle 25 de Mayo 88, Cochabamba'),
(7, 'Miguel Torrez',   '77665544', 'miguel.torrez@gmail.com',   'Av. Circunvalación 300, Cochabamba'),
(8, 'Sofía Vargas',    '76321987', 'sofia.vargas@gmail.com',    'Calle Ayacucho 45, Cochabamba'),
(9, 'Ricardo Paz',     '71122334', 'ricardo.paz@gmail.com',     'Av. Melchor Pérez 700, Cochabamba'),
(10,'Daniela Rojas',   '78877665', 'daniela.rojas@gmail.com',   'Calle Junín 210, Cochabamba');

-- ---------------------------------------------------------
-- VEHICULO
-- ---------------------------------------------------------
INSERT INTO vehiculo (id, cliente_id, placa, marca, modelo, anio, color, kilometraje) VALUES
(1, 1,  '1234ABC', 'Toyota',     'Corolla',  2015, 'Blanco', 85000),
(2, 2,  '5678BCD', 'Nissan',     'Sentra',   2018, 'Gris',   42000),
(3, 3,  '4321CDE', 'Chevrolet',  'Spark',    2019, 'Rojo',   30000),
(4, 4,  '8765DEF', 'Toyota',     'Hilux',    2012, 'Plata',  150000),
(5, 5,  '2468EFG', 'Suzuki',     'Swift',    2020, 'Azul',   15000),
(6, 6,  '1357FGH', 'Ford',       'Fiesta',   2016, 'Negro',  60000),
(7, 7,  '9753GHI', 'Hyundai',    'Accent',   2017, 'Blanco', 55000),
(8, 8,  '8642HIJ', 'Kia',        'Rio',      2021, 'Rojo',   10000),
(9, 9,  '7531IJK', 'Mitsubishi', 'Lancer',   2014, 'Gris',   95000),
(10,10, '6420JKL', 'Toyota',     'Yaris',    2019, 'Blanco', 28000);

-- ---------------------------------------------------------
-- DANO_VEHICULO
-- ---------------------------------------------------------
INSERT INTO dano_vehiculo (id, vehiculo_id, descripcion, fecha) VALUES
(1, 1,  'Rayón en la puerta izquierda', '2023-05-10'),
(2, 1,  'Golpe leve en parachoques trasero', '2024-01-15'),
(3, 4,  'Abolladura en el capó', '2022-11-20'),
(4, 4,  'Fisura en el parabrisas', '2023-08-02'),
(5, 6,  'Retrovisor derecho roto', '2023-03-12'),
(6, 7,  'Rayones en la puerta trasera', '2024-02-28'),
(7, 9,  'Golpe en el guardabarros delantero', '2021-09-05'),
(8, 9,  'Faro delantero derecho fisurado', '2022-06-18'),
(9, 2,  'Rayón superficial en el techo', '2023-12-01'),
(10,10, 'Abolladura leve en la puerta del copiloto', '2024-04-22');

-- ---------------------------------------------------------
-- MECANICO
-- ---------------------------------------------------------
INSERT INTO mecanico (id, nombre, especialidad, telefono, activo) VALUES
(1, 'Juan Pérez',       'Motor',                    '70011122', TRUE),
(2, 'Luis Gutiérrez',   'Frenos y suspensión',      '70022233', TRUE),
(3, 'Pablo Sánchez',    'Electricidad automotriz',  '70033344', TRUE),
(4, 'Marcelo Choque',   'Transmisión',              '70044455', TRUE),
(5, 'Hugo Villca',      'Motor',                    '70055566', TRUE),
(6, 'Fernando Ríos',    'Aire acondicionado',       '70066677', TRUE),
(7, 'Oscar Mendoza',    'Frenos y suspensión',      '70077788', FALSE),
(8, 'Raúl Aguilar',     'Electricidad automotriz',  '70088899', TRUE),
(9, 'Víctor Salazar',   'Motor',                    '70099900', TRUE),
(10,'Adrián Cárdenas',  'Carrocería y pintura',     '70100011', TRUE);

-- ---------------------------------------------------------
-- ORDEN_TRABAJO
-- ---------------------------------------------------------
INSERT INTO orden_trabajo (id, fecha_ingreso, estado, diagnostico) VALUES
(1, '2024-05-02 08:30:00', 'terminado',   'Cambio de aceite y filtro, todo en orden'),
(2, '2024-05-03 09:15:00', 'entregado',   'Pastillas de freno desgastadas, se reemplazaron'),
(3, '2024-05-05 10:00:00', 'pagado',      'Fuga de refrigerante por manguera, reemplazada'),
(4, '2024-05-06 11:20:00', 'en_proceso',  'Revisión de sistema eléctrico en curso'),
(5, '2024-05-08 08:45:00', 'cotizado',    NULL),
(6, '2024-05-09 09:00:00', 'diagnostico', NULL),
(7, '2024-05-10 10:30:00', 'recibido',    NULL),
(8, '2024-05-11 07:50:00', 'aprobado',    'Kit de embrague dañado, requiere cambio completo'),
(9, '2024-05-12 12:00:00', 'terminado',   'Alineación y balanceo realizados'),
(10,'2024-05-14 08:10:00', 'en_proceso',  'Cambio de correa de distribución en curso');

-- ---------------------------------------------------------
-- SERVICIO
-- ---------------------------------------------------------
INSERT INTO servicio (id, vehiculo_id, orden_trabajo_id, tipo_servicio, descripcion_solicitud, fecha_solicitud) VALUES
(1, 1,  1,    'preventivo',      'Cambio de aceite y filtro', '2024-05-01 16:00:00'),
(2, 2,  2,    'correctivo',      'Ruido al frenar', '2024-05-02 17:30:00'),
(3, 3,  3,    'correctivo',      'Fuga de líquido bajo el motor', '2024-05-04 15:00:00'),
(4, 4,  4,    'correctivo',      'Luces del tablero fallando', '2024-05-05 18:00:00'),
(5, 5,  5,    'revision_simple', 'Revisión general antes de viaje', '2024-05-07 09:00:00'),
(6, 6,  6,    'correctivo',      'Ruido extraño en el motor', '2024-05-08 10:00:00'),
(7, 7,  NULL, 'preventivo',      'Cambio de aceite programado', '2024-05-10 11:00:00'),
(8, 8,  8,    'correctivo',      'Dificultad para cambiar marchas', '2024-05-10 14:00:00'),
(9, 9,  9,    'revision_simple', 'Vehículo se desvía al frenar', '2024-05-11 08:30:00'),
(10,10, 10,   'correctivo',      'Ruido en el motor al arrancar', '2024-05-13 09:45:00');

-- ---------------------------------------------------------
-- ORDEN_MECANICO
-- ---------------------------------------------------------
INSERT INTO orden_mecanico (orden_id, mecanico_id, es_responsable, parte_asignada) VALUES
(1, 1, TRUE,  'Cambio de aceite y filtro'),
(2, 2, TRUE,  'Cambio de pastillas de freno'),
(3, 1, TRUE,  'Reemplazo de manguera de refrigerante'),
(3, 3, FALSE, 'Revisión eléctrica asociada'),
(4, 3, TRUE,  'Diagnóstico eléctrico'),
(5, 5, TRUE,  'Diagnóstico de motor'),
(6, 5, TRUE,  'Revisión de motor'),
(6, 9, FALSE, 'Apoyo en desarme de motor'),
(8, 4, TRUE,  'Cambio de kit de embrague'),
(8, 2, FALSE, 'Apoyo en desmontaje de caja'),
(9, 2, TRUE,  'Alineación y balanceo'),
(10,5, TRUE,  'Cambio de correa de distribución');

-- ---------------------------------------------------------
-- COTIZACION
-- ---------------------------------------------------------
INSERT INTO cotizacion (id, orden_id, version, monto_total, estado, fecha) VALUES
(1, 1,  1, 250.00,  'aprobada',  '2024-05-02 09:00:00'),
(2, 2,  1, 480.00,  'aprobada',  '2024-05-03 10:00:00'),
(3, 3,  1, 620.00,  'aprobada',  '2024-05-05 11:00:00'),
(4, 3,  2, 750.00,  'aprobada',  '2024-05-06 09:30:00'),
(5, 4,  1, 300.00,  'pendiente', '2024-05-06 12:00:00'),
(6, 5,  1, 180.00,  'pendiente', '2024-05-08 09:15:00'),
(7, 8,  1, 1450.00, 'aprobada',  '2024-05-11 08:30:00'),
(8, 9,  1, 220.00,  'aprobada',  '2024-05-12 13:00:00'),
(9, 10, 1, 540.00,  'pendiente', '2024-05-14 09:00:00'),
(10,6,  1, 90.00,   'rechazada', '2024-05-09 10:30:00');

-- ---------------------------------------------------------
-- COTIZACION_DETALLE
-- ---------------------------------------------------------
INSERT INTO cotizacion_detalle (id, cotizacion_id, descripcion, tipo, monto) VALUES
(1, 1, 'Mano de obra cambio de aceite',              'mano_obra', 80.00),
(2, 1, 'Aceite y filtro',                            'repuesto',  170.00),
(3, 2, 'Mano de obra frenos',                        'mano_obra', 150.00),
(4, 2, 'Pastillas de freno',                         'repuesto',  330.00),
(5, 3, 'Mano de obra reparación de fuga',            'mano_obra', 200.00),
(6, 4, 'Repuestos adicionales por fuga mayor',       'repuesto',  550.00),
(7, 7, 'Kit de embrague completo',                   'repuesto',  1200.00),
(8, 7, 'Mano de obra instalación de embrague',       'mano_obra', 250.00),
(9, 8, 'Alineación y balanceo',                      'mano_obra', 220.00),
(10,9, 'Correa de distribución y mano de obra',      'repuesto',  540.00);

-- ---------------------------------------------------------
-- PRODUCTO
-- ---------------------------------------------------------
INSERT INTO producto (id, nombre, categoria, precio, stock) VALUES
(1, 'Aceite de motor 20W-50 (1L)',      'Lubricantes', 45.00,  120),
(2, 'Filtro de aceite',                 'Filtros',     25.00,  80),
(3, 'Filtro de aire',                   'Filtros',     30.00,  60),
(4, 'Pastillas de freno delanteras',    'Frenos',      180.00, 40),
(5, 'Pastillas de freno traseras',      'Frenos',      150.00, 35),
(6, 'Grasa multipropósito (400g)',      'Lubricantes', 20.00,  100),
(7, 'Refrigerante (1L)',                'Líquidos',    35.00,  70),
(8, 'Correa de distribución',           'Transmisión', 320.00, 15),
(9, 'Batería 12V 60Ah',                 'Eléctrico',   480.00, 10),
(10,'Bujías (juego x4)',                'Eléctrico',   90.00,  50);

-- ---------------------------------------------------------
-- ORDEN_PRODUCTO
-- ---------------------------------------------------------
INSERT INTO orden_producto (orden_id, producto_id, cantidad, precio_unitario) VALUES
(1, 1, 3, 45.00),
(1, 2, 1, 25.00),
(2, 4, 1, 180.00),
(3, 7, 2, 35.00),
(4, 10,1, 90.00),
(8, 8, 1, 320.00),
(8, 6, 2, 20.00),
(9, 3, 1, 30.00),
(10,8, 1, 320.00),
(10,7, 1, 35.00);

-- ---------------------------------------------------------
-- VENTA_DIRECTA
-- ---------------------------------------------------------
INSERT INTO venta_directa (id, cliente_id, fecha) VALUES
(1, 2,    '2024-05-01 12:00:00'),
(2, 5,    '2024-05-02 13:30:00'),
(3, NULL, '2024-05-03 09:00:00'),
(4, 7,    '2024-05-04 15:00:00'),
(5, 1,    '2024-05-05 10:20:00'),
(6, NULL, '2024-05-06 11:00:00'),
(7, 9,    '2024-05-07 16:40:00'),
(8, 3,    '2024-05-08 08:50:00'),
(9, NULL, '2024-05-09 14:10:00'),
(10,6,    '2024-05-10 09:30:00');

-- ---------------------------------------------------------
-- VENTA_DIRECTA_DETALLE
-- ---------------------------------------------------------
INSERT INTO venta_directa_detalle (venta_id, producto_id, cantidad, precio_unitario) VALUES
(1, 1, 2, 45.00),
(1, 6, 1, 20.00),
(2, 2, 1, 25.00),
(3, 6, 3, 20.00),
(4, 7, 1, 35.00),
(5, 1, 1, 45.00),
(6, 10,1, 90.00),
(7, 3, 1, 30.00),
(8, 4, 1, 180.00),
(9, 6, 2, 20.00);

-- ---------------------------------------------------------
-- PAGO
-- ---------------------------------------------------------
INSERT INTO pago (id, orden_id, monto, metodo, tipo, fecha) VALUES
(1, 1,  250.00,  'efectivo',      'total_100',   '2024-05-02 12:00:00'),
(2, 2,  120.00,  'tarjeta',       'anticipo_25', '2024-05-03 11:00:00'),
(3, 2,  360.00,  'tarjeta',       'saldo_final', '2024-05-04 17:00:00'),
(4, 3,  375.00,  'transferencia', 'mitad_50',    '2024-05-05 12:30:00'),
(5, 3,  375.00,  'transferencia', 'saldo_final', '2024-05-07 10:00:00'),
(6, 8,  1450.00, 'tarjeta',       'total_100',   '2024-05-11 09:15:00'),
(7, 9,  220.00,  'efectivo',      'total_100',   '2024-05-12 14:00:00'),
(8, 10, 135.00,  'efectivo',      'anticipo_25', '2024-05-14 10:00:00'),
(9, 4,  75.00,   'efectivo',      'anticipo_25', '2024-05-06 13:00:00'),
(10,5,  45.00,   'efectivo',      'anticipo_25', '2024-05-08 10:00:00');

-- ---------------------------------------------------------
-- MOVIMIENTO_INVENTARIO
-- ---------------------------------------------------------
INSERT INTO movimiento_inventario (id, producto_id, tipo_movimiento, cantidad, motivo, referencia_id, fecha, stock_resultante) VALUES
(1, 1, 'entrada', 50, 'compra',          NULL, '2024-04-25 09:00:00', 170),
(2, 1, 'salida',  3,  'uso_orden',       1,    '2024-05-02 10:15:00', 167),
(3, 2, 'salida',  1,  'uso_orden',       1,    '2024-05-02 10:15:00', 79),
(4, 4, 'salida',  1,  'uso_orden',       2,    '2024-05-03 11:00:00', 39),
(5, 7, 'salida',  2,  'uso_orden',       3,    '2024-05-05 09:30:00', 68),
(6, 10,'salida',  1,  'uso_orden',       4,    '2024-05-06 14:00:00', 49),
(7, 8, 'salida',  1,  'uso_orden',       8,    '2024-05-11 16:00:00', 14),
(8, 6, 'salida',  2,  'uso_orden',       8,    '2024-05-11 16:00:00', 98),
(9, 9, 'entrada', 5,  'compra',          NULL, '2024-05-01 08:00:00', 15),
(10,1, 'salida',  2,  'venta_mostrador', 1,    '2024-05-01 12:00:00', 118);
