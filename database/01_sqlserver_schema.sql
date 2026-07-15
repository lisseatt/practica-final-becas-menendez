-- Creacion de la tabla catalogo en SQL Server
CREATE TABLE tipos_beca (
    id_tipo INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL UNIQUE,
    monto_mensual DECIMAL(8,2) NOT NULL,
    cupo_anual INT NOT NULL,
    activo BIT DEFAULT 1
);

-- Creacion de la tabla principal en SQL Server
CREATE TABLE aspirantes (
    id_aspirante INT IDENTITY(1,1) PRIMARY KEY,
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
    fecha_registro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Aspirantes_TiposBeca FOREIGN KEY (id_tipo) REFERENCES tipos_beca(id_tipo)
);