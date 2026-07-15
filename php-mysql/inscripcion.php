<?php
// Configuración de la conexión a la base de datos SQLite
require_once 'conexion.php';
require_once 'funciones.php';

// Inicialización de variables para el manejo de mensajes y datos del formulario
$errores = [];
$dui = $nombres = $apellidos = $fecha_nacimiento = $sexo = $telefono = $correo = $institucion_estudio = $promedio = $ingreso_familiar = $id_tipo = "";

// Procesamiento de los datos cuando el usuario envía el formulario
if ($_SERVER["REQUEST_METHOD"] == "POST") {

    // Sanitización y captura de datos enviados desde el navegador
    $dui = trim($_POST['dui'] ?? '');
    $nombres = trim($_POST['nombres'] ?? '');
    $apellidos = trim($_POST['apellidos'] ?? '');
    $fecha_nacimiento = trim($_POST['fecha_nacimiento'] ?? '');
    $sexo = trim($_POST['sexo'] ?? '');
    $telefono = trim($_POST['telefono'] ?? '');
    $correo = trim($_POST['correo'] ?? '');
    $institucion_estudio = trim($_POST['institucion_estudio'] ?? '');
    $promedio = trim($_POST['promedio'] ?? '');
    $ingreso_familiar = trim($_POST['ingreso_familiar'] ?? '');
    $id_tipo = trim($_POST['id_tipo'] ?? '');

    // Validación básica de campos obligatorios en el servidor
    if (!validarDUI($dui)) $errores['dui'] = "Formato inválido (00000000-0).";
    if (!validarRequerido($nombres)) $errores['nombres'] = "Campo requerido.";
    if (!validarRequerido($apellidos)) $errores['apellidos'] = "Campo requerido.";
    if (!validarRequerido($fecha_nacimiento)) $errores['fecha_nacimiento'] = "Campo requerido.";
    if (!in_array($sexo, ['M', 'F'])) $errores['sexo'] = "Selección inválida.";
    if (!validarCorreo($correo)) $errores['correo'] = "Correo no válido.";
    if (!validarRequerido($institucion_estudio)) $errores['institucion_estudio'] = "Campo requerido.";
    if (!validarPromedio($promedio)) $errores['promedio'] = "Rango de 0.00 a 10.00.";
    if (!validarMontoPositivo($ingreso_familiar)) $errores['ingreso_familiar'] = "Monto inválido.";
    if (!validarRequerido($id_tipo)) $errores['id_tipo'] = "Requerido.";

    // Guardado seguro en la base de datos si no hay errores de validación
    if (empty($errores)) {
        try {
            // Verificar si el DUI ya está registrado previamente
            $stmtCheck = $conexion->prepare("SELECT COUNT(*) FROM aspirantes WHERE dui = :dui");
            $stmtCheck->execute([':dui' => $dui]);
            if ($stmtCheck->fetchColumn() > 0) {
                $errores['dui'] = "Este DUI ya existe.";
            } else {
                // Insertar nuevo aspirante de manera segura con PDO
                $sql = "INSERT INTO aspirantes (dui, nombres, apellidos, fecha_nacimiento, sexo, telefono, correo, institucion_estudio, promedio, ingreso_familiar, id_tipo) 
                        VALUES (:dui, :nombres, :apellidos, :fecha_nacimiento, :sexo, :telefono, :correo, :institucion_estudio, :promedio, :ingreso_familiar, :id_tipo)";
                $stmt = $conexion->prepare($sql);
                $stmt->execute([
                    ':dui' => $dui, ':nombres' => $nombres, ':apellidos' => $apellidos,
                    ':fecha_nacimiento' => $fecha_nacimiento, ':sexo' => $sexo,
                    ':telefono' => !empty($telefono) ? $telefono : null, ':correo' => $correo,
                    ':institucion_estudio' => $institucion_estudio, ':promedio' => floatval($promedio),
                    ':ingreso_familiar' => floatval($ingreso_familiar), ':id_tipo' => intval($id_tipo)
                ]);
                // Redirigir al finalizar el registro exitoso
                header("Location: gracias.php");
                exit();
            }
        } catch (PDOException $e) {
            $errores['global'] = "Error de base de datos: " . $e->getMessage();
        }
    }
}

// Obtener los tipos de beca activos para llenar el menú de selección (select)
$queryBecas = $conexion->query("SELECT id_tipo, nombre, monto_mensual FROM tipos_beca WHERE activo = 1 ORDER BY id_tipo ASC");
$becas = $queryBecas->fetchAll(PDO::FETCH_ASSOC);
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Inscripción al Sistema de Becas</title>
    <link rel="stylesheet" href="assets/style.css">
</head>
<body>
<div class="contenedor-formulario">
    <h2>Beca Joven Lourdes 2026</h2>
    <h4>Completa tus datos para inscribirte. Todos los campos marcados con * son obligatorios.</h4>
    
    <?php if (isset($errores['global'])): ?>
        <p class="error-mensaje global"><?php echo htmlspecialchars($errores['global']); ?></p>
    <?php endif; ?>

    <form action="inscripcion.php" method="POST">
        <div class="seccion-bloque">
            <div class="fila-triple">
                <div class="grupo-campo">
                    <label for="dui">DUI <span class="req">*</span></label>
                    <input type="text" id="dui" name="dui" placeholder="00000000-0" value="<?php echo htmlspecialchars($dui); ?>">
                    <?php if (isset($errores['dui'])): ?><span class="error-texto"><?php echo $errores['dui']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label for="nombres">Nombres <span class="req">*</span></label>
                    <input type="text" id="nombres" name="nombres" value="<?php echo htmlspecialchars($nombres); ?>">
                    <?php if (isset($errores['nombres'])): ?><span class="error-texto"><?php echo $errores['nombres']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label for="apellidos">Apellidos <span class="req">*</span></label>
                    <input type="text" id="apellidos" name="apellidos" value="<?php echo htmlspecialchars($apellidos); ?>">
                    <?php if (isset($errores['apellidos'])): ?><span class="error-texto"><?php echo $errores['apellidos']; ?></span><?php endif; ?>
                </div>
            </div>

            <div class="fila-triple">
                <div class="grupo-campo">
                    <label for="fecha_nacimiento">Fecha Nacimiento <span class="req">*</span></label>
                    <input type="date" id="fecha_nacimiento" name="fecha_nacimiento" value="<?php echo htmlspecialchars($fecha_nacimiento); ?>">
                    <?php if (isset($errores['fecha_nacimiento'])): ?><span class="error-texto"><?php echo $errores['fecha_nacimiento']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label>Sexo <span class="req">*</span></label>
                    <div class="opciones-radio">
                        <label><input type="radio" name="sexo" value="M" <?php echo ($sexo === 'M') ? 'checked' : ''; ?>> Masculino</label>
                        <label><input type="radio" name="sexo" value="F" <?php echo ($sexo === 'F') ? 'checked' : ''; ?>> Femenino</label>
                    </div>
                    <?php if (isset($errores['sexo'])): ?><span class="error-texto"><?php echo $errores['sexo']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label for="telefono">Teléfono</label>
                    <input type="text" id="telefono" name="telefono" placeholder="0000-0000" value="<?php echo htmlspecialchars($telefono); ?>">
                </div>
            </div>
        </div>

        <div class="seccion-bloque">
            <div class="fila-doble">
                <div class="grupo-campo">
                    <label for="correo">Correo Electrónico <span class="req">*</span></label>
                    <input type="email" id="correo" name="correo" value="<?php echo htmlspecialchars($correo); ?>">
                    <?php if (isset($errores['correo'])): ?><span class="error-texto"><?php echo $errores['correo']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label for="institucion_estudio">Institución de Estudio <span class="req">*</span></label>
                    <input type="text" id="institucion_estudio" name="institucion_estudio" value="<?php echo htmlspecialchars($institucion_estudio); ?>">
                    <?php if (isset($errores['institucion_estudio'])): ?><span class="error-texto"><?php echo $errores['institucion_estudio']; ?></span><?php endif; ?>
                </div>
            </div>

            <div class="fila-triple">
                <div class="grupo-campo">
                    <label for="promedio">Promedio <span class="req">*</span></label>
                    <input type="number" id="promedio" name="promedio" step="0.01" min="0" max="10" value="<?php echo htmlspecialchars($promedio); ?>">
                    <?php if (isset($errores['promedio'])): ?><span class="error-texto"><?php echo $errores['promedio']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label for="ingreso_familiar">Ingreso Familiar <span class="req">*</span></label>
                    <input type="number" id="ingreso_familiar" name="ingreso_familiar" step="0.01" min="0" value="<?php echo htmlspecialchars($ingreso_familiar); ?>">
                    <?php if (isset($errores['ingreso_familiar'])): ?><span class="error-texto"><?php echo $errores['ingreso_familiar']; ?></span><?php endif; ?>
                </div>
                <div class="grupo-campo">
                    <label for="id_tipo">Tipo de Beca <span class="req">*</span></label>
                    <select id="id_tipo" name="id_tipo">
                        <option value="">-- Seleccione --</option>
                        <?php foreach ($becas as $beca): ?>
                            <?php 
                            // Formateamos el monto quitando decimales flotantes innecesarios (.00)
                            $monto = (float)$beca['monto_mensual']; 
                            ?>
                            <option value="<?php echo $beca['id_tipo']; ?>" <?php echo ($id_tipo == $beca['id_tipo']) ? 'selected' : ''; ?>>
                                <?php echo htmlspecialchars($beca['nombre']) . " — $" . $monto . "/mes"; ?>
                            </option>
                        <?php endforeach; ?>
                    </select>
                    <?php if (isset($errores['id_tipo'])): ?><span class="error-texto"><?php echo $errores['id_tipo']; ?></span><?php endif; ?>
                </div>
            </div>
        </div>

        <div class="botonera-formulario">
            <button type="button" class="btn-limpiar" onclick="window.location.href='inscripcion.php';">Limpiar Campos</button>
            <button type="submit" class="btn-enviar">Enviar Inscripción</button>
        </div>
    </form>
</div>
</body>
</html>