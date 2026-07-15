-- Creacion de la tabla catalogo en MySQL
CREATE TABLE tipos_beca (
    id_tipo INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL UNIQUE,
    monto_mensual DECIMAL(8,2) NOT NULL,
    cupo_anual INT NOT NULL,
    activo TINYINT(1) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Creacion de la tabla principal en MySQL
CREATE TABLE aspirantes (
    id_aspirante INT AUTO_INCREMENT PRIMARY KEY,
    dui VARCHAR(10) NOT NULL UNIQUE,
    nombres VARCHAR(60) NOT NULL,
    apellidos VARCHAR(60) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    sexo CHAR(1) NOT NULL,
    telefono VARCHAR(9) NULL,
    correo VARCHAR(80) NOT NULL,
    institucion_estudio VARCHAR(100) NOT NULL,
    promedio DECIMAL(4,2) NOT NULL,
    ingreso_familiar DECIMAL(8,2) NOT NULL,
    id_tipo INT NOT NULL,
    fecha_registro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_tipo) REFERENCES tipos_beca(id_tipo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;