# Práctica Final - Sistema de Inscripción a Becas
### Tatiana Lissett Menéndez Mercado 19881837

Este repositorio contiene la entrega de la práctica final para el sistema de inscripción de becas "Beca Joven Lourdes 2026". El proyecto está compuesto por una solución de escritorio en C# (Windows Forms) y una aplicación web en PHP, ambas compartiendo e interactuando con la misma estructura de datos.


## Estructura del Repositorio
El repositorio sigue la estructura solicitada para la entrega:

* `database/`: Contiene los scripts de creación de esquemas y datos iniciales (Seed) para SQL Server y MySQL.
* `winforms-csharp/`: Contiene el proyecto en Visual Studio 2022 de Windows Forms con conexión local SQLite.
* `php-mysql/`: Contiene la plataforma web construida en PHP.


## Instrucciones de Instalación y Configuración

### 1. Base de Datos
Los scripts de estructura y carga de datos iniciales se encuentran listos para ser importados en sus respectivos motores dentro de la carpeta `database`:
* **Para SQL Server:** Ejecutar en orden `01_sqlserver_schema.sql` y luego `02_sqlserver_seed.sql`.
* **Para MySQL:** Ejecutar en orden `03_mysql_schema.sql` y luego `04_mysql_seed.sql`.

> **Nota:** Para el desarrollo local y pruebas ágiles de conectividad entre ambas plataformas, se utilizó una base de datos local SQLite (`becas.db`).

### 2. Configuración de Windows Forms (C#)
1. Abrir la carpeta `winforms-csharp/SisBecas` en Visual Studio.
2. Restaurar los paquetes de NuGet (en especial el proveedor oficial `System.Data.SQLite.Core`).
3. Verificar el archivo `App.config` donde se define la conexión a la base de datos local:
   ```xml
   <connectionStrings>
     <add name="ConexionBecas" connectionString="Data Source=|DataDirectory|\becas.db;Version=3;" providerName="System.Data.SQLite" />
   </connectionStrings>


## Capturas del Proyecto

Para visualizar las capturas de pantalla de ambas aplicaciones desarrolladas durante la práctica:

- **Aplicación de Escritorio (Windows Forms C#):**
  https://github.com/lisseatt/practica-final-becas-menendez/tree/23cde5685d7702f5980f6f2c3dc890d7400f4b47/winforms-csharp/capturas

- **Aplicación Web (PHP):**
  https://github.com/lisseatt/practica-final-becas-menendez/tree/23cde5685d7702f5980f6f2c3dc890d7400f4b47/php-mysql/capturas


## Lo que aprendí

En esta práctica pude comprender mejor cómo construir los formularios tanto en Windows Forms como en PHP. En Windows Forms aprendí a organizar los controles dentro de los formularios, como se usan las validaciones y conectar la aplicación con una base de datos de forma estructurada. Por otro lado, en la aplicación web reforcé el uso de formularios en PHP para recibir, validar y procesar la información enviada por el usuario, además de trabajar con la conexión a la base de datos y la generación dinámica de contenido. En conjunto, esta práctica me permitió comparar el desarrollo de aplicaciones de escritorio y web, identificando las ventajas y diferencias de cada enfoque.