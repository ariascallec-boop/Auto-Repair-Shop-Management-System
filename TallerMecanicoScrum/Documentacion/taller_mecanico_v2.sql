DROP DATABASE IF EXISTS taller_mecanico_v2;

CREATE DATABASE taller_mecanico_v2
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE taller_mecanico_v2;

-- ==========================================
-- 1. CATÁLOGOS BASE Y CLIENTES
-- ==========================================
CREATE TABLE cliente (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(80)  NOT NULL,
    apellido        VARCHAR(80)  NOT NULL,
    ci_nit          VARCHAR(20)  NOT NULL UNIQUE,
    telefono        VARCHAR(20)  NOT NULL,
    email           VARCHAR(120) NOT NULL,
    direccion       VARCHAR(200) NOT NULL,
    estado          BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_registro  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE vehiculo (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id      INT NOT NULL,
    placa           VARCHAR(15) NOT NULL UNIQUE,
    marca           VARCHAR(60) NOT NULL,
    modelo          VARCHAR(60) NOT NULL,
    anio            SMALLINT NOT NULL CHECK (anio > 1900),
    color           VARCHAR(40) NOT NULL,
    tipo            VARCHAR(40) NOT NULL, -- HU-02: Tipo de vehículo (Sedán, SUV, etc.)
    kilometraje     INT NOT NULL DEFAULT 0 CHECK (kilometraje >= 0),
    estado          BOOLEAN NOT NULL DEFAULT TRUE,
    creado_en       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_vehiculo_cliente
        FOREIGN KEY (cliente_id) REFERENCES cliente(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE mecanico (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(60) NOT NULL, -- HU-03: Nombre separado
    apellido        VARCHAR(60) NOT NULL, -- HU-03: Apellido separado
    documento       VARCHAR(20) NOT NULL UNIQUE, -- HU-03: Documento añadido
    especialidad    VARCHAR(80) NOT NULL,
    telefono        VARCHAR(20) NOT NULL,
    activo          BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE servicio (
    id                  INT AUTO_INCREMENT PRIMARY KEY,
    nombre              VARCHAR(120) NOT NULL,
    tipo                ENUM('preventivo','correctivo','revision_simple') NOT NULL,
    descripcion         VARCHAR(255) NOT NULL,
    precio              DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    duracion_estimada   INT NOT NULL CHECK (duracion_estimada > 0),
    unidad_duracion     ENUM('Minutos','Horas','Dias') NOT NULL,
    estado              BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE producto (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    nombre          VARCHAR(120) NOT NULL,
    descripcion     VARCHAR(255) NOT NULL, -- HU-05: Descripción añadida
    categoria       VARCHAR(60) NOT NULL,
    precio          DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    stock           INT NOT NULL DEFAULT 0 CHECK (stock >= 0), -- Prevención de stock negativo
    estado          BOOLEAN NOT NULL DEFAULT TRUE
);

-- ==========================================
-- 2. TRANSACCIONES PRINCIPALES (ÓRDENES)
-- ==========================================
CREATE TABLE orden_trabajo (
    id                      INT AUTO_INCREMENT PRIMARY KEY,
    vehiculo_id             INT NOT NULL,
    fecha_ingreso           DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_entrega_estimada  DATETIME NOT NULL,
    -- HU-14: Estados actualizados estrictamente según el documento
    estado                  ENUM(
                                'recibido', 'en_revision', 'cotizado', 
                                'esperando_aprobacion', 'aprobado', 'en_reparacion', 
                                'esperando_repuesto', 'esperando_pago', 'listo_para_prueba', 
                                'en_prueba', 'listo_para_entrega', 'entregado'
                            ) NOT NULL DEFAULT 'recibido',
    diagnostico             TEXT NOT NULL,
    
    -- HU-17: Campos para registro de notificaciones al cliente
    fecha_notificacion      DATETIME NULL,
    observaciones_notificacion VARCHAR(255) NULL,

    CONSTRAINT fk_orden_vehiculo
        FOREIGN KEY (vehiculo_id) REFERENCES vehiculo(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE dano_recepcion (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    orden_trabajo_id INT NOT NULL, -- HU-06: Vinculado a la orden, no solo al vehículo general
    descripcion     VARCHAR(255) NOT NULL,
    fecha           DATE NOT NULL,
    foto_url        VARCHAR(255) NOT NULL,

    CONSTRAINT fk_dano_orden
        FOREIGN KEY (orden_trabajo_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE orden_servicio (
    id                    INT AUTO_INCREMENT PRIMARY KEY,
    orden_trabajo_id      INT NOT NULL,
    servicio_id           INT NOT NULL,
    descripcion_solicitud VARCHAR(255) NOT NULL,
    cantidad              INT NOT NULL DEFAULT 1 CHECK (cantidad > 0),
    precio_aplicado       DECIMAL(10,2) NOT NULL CHECK (precio_aplicado >= 0), -- HU-08: Historial de precio
    subtotal              DECIMAL(10,2) GENERATED ALWAYS AS (cantidad * precio_aplicado) STORED, -- Autocalculado seguro
    fecha_solicitud       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_orden_servicio UNIQUE (orden_trabajo_id, servicio_id),

    CONSTRAINT fk_os_orden
        FOREIGN KEY (orden_trabajo_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    CONSTRAINT fk_os_servicio
        FOREIGN KEY (servicio_id) REFERENCES servicio(id)
        ON DELETE RESTRICT -- Previene dañar órdenes históricas si se borra un servicio
        ON UPDATE CASCADE
);

CREATE TABLE orden_producto (
    orden_id        INT NOT NULL,
    producto_id     INT NOT NULL,
    cantidad        INT NOT NULL CHECK (cantidad > 0),
    precio_unitario DECIMAL(10,2) NOT NULL CHECK (precio_unitario >= 0),
    subtotal        DECIMAL(10,2) GENERATED ALWAYS AS (cantidad * precio_unitario) STORED,

    PRIMARY KEY (orden_id, producto_id),

    CONSTRAINT fk_op_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    CONSTRAINT fk_op_producto
        FOREIGN KEY (producto_id) REFERENCES producto(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

CREATE TABLE orden_mecanico (
    orden_id        INT NOT NULL,
    mecanico_id     INT NOT NULL,
    es_responsable  BOOLEAN NOT NULL DEFAULT FALSE,
    parte_asignada  VARCHAR(150) NOT NULL,

    PRIMARY KEY (orden_id, mecanico_id),

    CONSTRAINT fk_om_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    CONSTRAINT fk_om_mecanico
        FOREIGN KEY (mecanico_id) REFERENCES mecanico(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- HU-15: Tabla explícita para registrar pruebas de vehículos
CREATE TABLE prueba_vehiculo (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    orden_id        INT NOT NULL,
    mecanico_id     INT NOT NULL,
    resultado       ENUM('aprobado', 'no_aprobado') NOT NULL,
    observaciones   TEXT,
    fecha_prueba    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_prueba_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    CONSTRAINT fk_prueba_mecanico
        FOREIGN KEY (mecanico_id) REFERENCES mecanico(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- ==========================================
-- 3. GESTIÓN FINANCIERA E INVENTARIO
-- ==========================================
CREATE TABLE cotizacion (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    orden_id        INT NOT NULL,
    version         INT NOT NULL DEFAULT 1,
    monto_total     DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (monto_total >= 0),
    estado          ENUM('pendiente','aprobada','rechazada') NOT NULL DEFAULT 'pendiente',
    fecha           DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_cotizacion_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    CONSTRAINT uq_orden_version UNIQUE (orden_id, version)
);

CREATE TABLE cotizacion_detalle (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    cotizacion_id   INT NOT NULL,
    descripcion     VARCHAR(200) NOT NULL,
    tipo            ENUM('mano_obra','repuesto') NOT NULL,
    monto           DECIMAL(10,2) NOT NULL CHECK (monto >= 0),

    CONSTRAINT fk_detalle_cotizacion
        FOREIGN KEY (cotizacion_id) REFERENCES cotizacion(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE pago (
    id              INT AUTO_INCREMENT PRIMARY KEY,
    orden_id        INT NOT NULL,
    monto           DECIMAL(10,2) NOT NULL CHECK (monto > 0),
    metodo          ENUM('efectivo','tarjeta','transferencia') NOT NULL,
    tipo            ENUM('anticipo_25','mitad_50','total_100','saldo_final', 'otro_monto') NOT NULL,
    fecha           DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_pago_orden
        FOREIGN KEY (orden_id) REFERENCES orden_trabajo(id)
        ON DELETE RESTRICT -- EXCEPCIÓN: Protege registros financieros ante borrados accidentales de órdenes
        ON UPDATE CASCADE
);

CREATE TABLE movimiento_inventario (
    id                INT AUTO_INCREMENT PRIMARY KEY,
    producto_id       INT NOT NULL,
    tipo_movimiento   ENUM('entrada','salida') NOT NULL,
    cantidad          INT NOT NULL CHECK (cantidad > 0),
    motivo            ENUM('compra','ajuste','uso_orden','venta_mostrador') NOT NULL,
    referencia_id     INT NULL,
    fecha             DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    stock_resultante  INT NOT NULL CHECK (stock_resultante >= 0),

    CONSTRAINT fk_movimiento_producto
        FOREIGN KEY (producto_id) REFERENCES producto(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);

-- ==========================================
-- ÍNDICES PARA OPTIMIZACIÓN
-- ==========================================
CREATE INDEX idx_cliente_telefono        ON cliente(telefono);
CREATE INDEX idx_vehiculo_placa          ON vehiculo(placa);
CREATE INDEX idx_orden_estado            ON orden_trabajo(estado);
CREATE INDEX idx_orden_entrega           ON orden_trabajo(fecha_entrega_estimada);
CREATE INDEX idx_movimiento_fecha        ON movimiento_inventario(fecha);

-- ========================================================
-- INSERCIÓN DE DATOS DE PRUEBA (REGISTROS)
-- ========================================================

INSERT INTO cliente (id, nombre, apellido, ci_nit, telefono, email, direccion, estado) VALUES
(1, 'Carlos', 'Mamani', '8456123', '71234567', 'carlos.mamani@gmail.com', 'Av. Heroínas 123', TRUE),
(2, 'María', 'Fernández', '6512389', '70112233', 'maria@hotmail.com', 'Calle España 456', TRUE);

INSERT INTO vehiculo (id, cliente_id, placa, marca, modelo, anio, color, tipo, kilometraje, estado) VALUES
(1, 1, '1234ABC', 'Toyota', 'Corolla', 2015, 'Blanco', 'Sedán', 85000, TRUE),
(2, 2, '5678BCD', 'Nissan', 'Sentra', 2018, 'Gris', 'Sedán', 42000, TRUE);

INSERT INTO mecanico (id, nombre, apellido, documento, especialidad, telefono, activo) VALUES
(1, 'Juan', 'Pérez', '5551234', 'Motor', '70011122', TRUE),
(2, 'Luis', 'Gutiérrez', '5555678', 'Frenos y suspensión', '70022233', TRUE);

INSERT INTO servicio (id, nombre, tipo, descripcion, precio, duracion_estimada, unidad_duracion, estado) VALUES
(1, 'Cambio de aceite y filtro', 'preventivo', 'Cambio de aceite de motor', 180.00, 45, 'Minutos', TRUE),
(2, 'Revisión y cambio de frenos', 'correctivo', 'Reemplazo de pastillas', 480.00, 2, 'Horas', TRUE);

INSERT INTO producto (id, nombre, descripcion, categoria, precio, stock, estado) VALUES
(1, 'Aceite 20W-50 (1L)', 'Aceite mineral multígrado', 'Lubricantes', 45.00, 120, TRUE),
(2, 'Pastillas de freno', 'Pastillas cerámicas delanteras', 'Frenos', 180.00, 40, TRUE);

INSERT INTO orden_trabajo (id, vehiculo_id, fecha_ingreso, fecha_entrega_estimada, estado, diagnostico) VALUES
(1, 1, NOW() - INTERVAL 3 DAY, TIMESTAMP(CURDATE(), '16:30:00'), 'entregado', 'Mantenimiento preventivo'),
(2, 2, NOW() - INTERVAL 2 DAY, TIMESTAMP(CURDATE(), '18:00:00'), 'en_reparacion', 'Pastillas desgastadas');

INSERT INTO dano_recepcion (id, orden_trabajo_id, descripcion, fecha, foto_url) VALUES
(1, 1, 'Rayón leve en puerta izquierda', CURDATE() - INTERVAL 3 DAY, '/img/dano1.jpg'),
(2, 2, 'Golpe en parachoques', CURDATE() - INTERVAL 2 DAY, '/img/dano2.jpg');

INSERT INTO orden_servicio (id, orden_trabajo_id, servicio_id, descripcion_solicitud, cantidad, precio_aplicado) VALUES
(1, 1, 1, 'Mantenimiento por kilometraje', 1, 180.00),
(2, 2, 2, 'Ruido al frenar', 1, 480.00);

INSERT INTO orden_mecanico (orden_id, mecanico_id, es_responsable, parte_asignada) VALUES
(1, 1, TRUE, 'Cambio de aceite'),
(2, 2, TRUE, 'Frenos');

INSERT INTO prueba_vehiculo (orden_id, mecanico_id, resultado, observaciones) VALUES
(1, 1, 'aprobado', 'Vehículo funciona perfectamente, niveles de aceite correctos.');

INSERT INTO pago (id, orden_id, monto, metodo, tipo, fecha) VALUES
(1, 1, 250.00, 'efectivo', 'total_100', NOW() - INTERVAL 2 DAY);

-- ==========================================
-- 4. VISTAS ACTUALIZADAS
-- ==========================================
CREATE VIEW vw_resumen_inicio AS
SELECT 
    ot.id AS orden_id,
    v.placa,
    CONCAT(c.nombre, ' ', c.apellido) AS cliente,
    CONCAT(v.marca, ' ', v.modelo) AS vehiculo,
    ot.estado,
    CONCAT(m.nombre, ' ', m.apellido) AS responsable,
    ot.fecha_entrega_estimada
FROM orden_trabajo AS ot
INNER JOIN vehiculo AS v ON ot.vehiculo_id = v.id
INNER JOIN cliente AS c ON v.cliente_id = c.id
INNER JOIN orden_mecanico AS om ON ot.id = om.orden_id AND om.es_responsable = TRUE
INNER JOIN mecanico AS m ON om.mecanico_id = m.id
WHERE c.estado = TRUE AND v.estado = TRUE;