<?php

// Valida si un campo de texto no esta vacio o compuesto solo por espacios
function validarRequerido($valor) {
    return trim($valor) !== '';
}

// Valida que el formato del DUI salvadoreño sea exactamente de 8 digitos, un guion y un digito
function validarDUI($dui) {
    return preg_match('/^\d{8}-\d{1}$/', $dui);
}

// Valida que el correo electronico tenga una estructura institucional o estandar correcta
function validarCorreo($correo) {
    return filter_var($correo, FILTER_VALIDATE_EMAIL) !== false;
}

// Valida que el promedio sea un numero decimal dentro del rango escolar de 0.00 a 10.00
function validarPromedio($promedio) {
    if (!is_numeric($promedio)) {
        return false;
    }
    $valor = floatval($promedio);
    return $valor >= 0 && $valor <= 10;
}

// Valida que los ingresos familiares o montos no sean valores negativos
function validarMontoPositivo($monto) {
    if (!is_numeric($monto)) {
        return false;
    }
    return floatval($monto) >= 0;
}
?>