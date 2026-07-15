<?php
// Define los parametros de persistencia y conexion de datos mediante el driver PDO
try {
    // Para tus pruebas locales usamos el archivo local SQLite
    $base_datos_local = __DIR__ . '/becas.db';
    $conexion = new PDO("sqlite:" . $base_datos_local);
    
    // Configuracion de control de excepciones e integridad de codificacion
    $conexion->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    
    // Inicializacion dinamica de las tablas locales con los campos exactos del diseño
    $conexion->exec("
        CREATE TABLE IF NOT EXISTS tipos_beca (
            id_tipo INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre TEXT NOT NULL UNIQUE,
            monto_mensual REAL NOT NULL,
            cupo_anual INTEGER NOT NULL,
            activo INTEGER DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS aspirantes (
            id_aspirante INTEGER PRIMARY KEY AUTOINCREMENT,
            dui TEXT NOT NULL UNIQUE,
            nombres TEXT NOT NULL,
            apellidos TEXT NOT NULL,
            fecha_nacimiento TEXT NOT NULL,
            sexo TEXT NOT NULL,
            telefono TEXT,
            correo TEXT NOT NULL,
            institucion_estudio TEXT NOT NULL,
            promedio REAL NOT NULL,
            ingreso_familiar REAL NOT NULL,
            id_tipo INTEGER NOT NULL,
            fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (id_tipo) REFERENCES tipos_beca(id_tipo)
        );
    ");

 // Poblado automatico de los registros del catalogo con datos oficiales si se encuentra vacio
    $check = $conexion->query("SELECT COUNT(*) FROM tipos_beca")->fetchColumn();
    if ($check == 0) {
        $conexion->exec("
            INSERT INTO tipos_beca (nombre, monto_mensual, cupo_anual, activo) VALUES ('Técnica', 50.00, 30, 1);
            INSERT INTO tipos_beca (nombre, monto_mensual, cupo_anual, activo) VALUES ('Universitaria', 100.00, 20, 1);
            INSERT INTO tipos_beca (nombre, monto_mensual, cupo_anual, activo) VALUES ('Complementaria', 30.00, 50, 1);
            INSERT INTO tipos_beca (nombre, monto_mensual, cupo_anual, activo) VALUES ('Excelencia', 150.00, 10, 1);
        ");
    }

} catch (PDOException $e) {
    die("Fallo critico de conexion con el motor de base de datos: " . $e->getMessage());
}
?>