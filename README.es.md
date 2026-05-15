# GitHub Copilot App — Preview Técnica

---

## Tabla de contenido

- [¿Qué es la GitHub Copilot App?](#qué-es-la-github-copilot-app)
- [El flujo completo, paso a paso](#el-flujo-completo-paso-a-paso)
- [⚠️ La jugada maestra de Microsoft/GitHub: un golpe en la mesa](#la-jugada-maestra-de-microsoftgithub-un-golpe-en-la-mesa)
- [Resumen del flujo completo](#resumen-del-flujo-completo)
- [Disponibilidad](#disponibilidad)

---

## ¿Qué es la GitHub Copilot App?

La **GitHub Copilot App** es una experiencia de escritorio nativa de GitHub para el desarrollo agéntico. Su propósito es arrancar desde el trabajo que ya tienes en GitHub (issues, PRs, sesiones previas), mantener ese trabajo aislado, guiarlo mientras avanza y aterrizar el cambio a través del proceso habitual de pull request.

En pocas palabras: el agente escribe código, ejecuta tests, abre PRs y gestiona revisiones mientras tú supervisas.

---

## El flujo completo, paso a paso

Las imágenes siguientes siguen el orden real de uso: primero los modos de operación, luego la configuración/ejecución de la sesión y finalmente el bloque completo de PR.

---

### Paso 1 — La pantalla de inicio: modos de operación

![Pantalla de inicio con modos](img/ghapp-33.png)

La pantalla principal de la GitHub Copilot App muestra el selector de **modo** de la sesión:

| Modo | Descripción |
|---|---|
| **Interactive** | Colaboración paso a paso (el agente consulta antes de actuar) |
| **Plan** | Planifica primero, ejecuta cuando estás listo |
| **Autopilot** | Ejecución end-to-end sin interrupciones |

Para casos de exploración o cuando no tienes claro el alcance, **Interactive** es el punto de partida natural. Para tareas bien definidas y repetibles, **Autopilot** maximiza la autonomía del agente.

---

### Paso 2 — Selección de modelos disponibles

![Selector de modelos](img/ghapp-32.png)

Al crear una sesión puedes elegir qué modelo utilizar. El desplegable **Auto** selecciona el modelo óptimo automáticamente con descuento de precio. Las opciones disponibles a fecha de la preview son:

| Modelo | Multiplicador de coste |
|---|---|
| Auto | Discount |
| Claude Opus 4.6 (1M · Internal) | 6× |
| Claude Opus 4.7 | 15× |
| Claude Sonnet 4.6 | 1× |
| GPT-5.3-Codex | 1× |
| GPT-5.4 | 1× |
| GPT-5.5 | 7.5× |

> La sesión de ejemplo de este tutorial utilizó **Claude Sonnet 4.6**, un equilibrio óptimo entre calidad y coste.

---

### Paso 3 — Repositorios disponibles en tu cuenta

![Lista de repositorios](img/ghapp-31.png)

Al añadir un proyecto a la app se despliega el selector de repositorios. En la cuenta `jmfloreszazo` aparecen entre otros:

- `aurora-ia-cobol_2_net10`
- `aks_troubleshooting`
- `meta-agents-fighting`
- `yarp_aigateway`
- `heuristiclegacyrefactoring`
- `demo_ontology_agents`
- `arcdoc_plus`
- (contribuciones externas como `dapr/components-contrib`)

La app conecta con cualquier repo de tu cuenta o de organizaciones a las que perteneces.

---

### Paso 4 — Crear un repositorio nuevo desde la app

![Crear repositorio](img/ghapp-30.png)

Si el repositorio no existe, se puede crear directamente desde la interfaz sin salir de la app:

- **Owner**: `jmfloreszazo`
- **Repository name**: `ghapp_demo`
- **Description**: `Simple demo of GH App`
- **Visibility**: Public / Private
- Opción de inicializar con un README

Una vez creado, queda vinculado al proyecto activo.

---

### Paso 5 — Dónde ejecutar la sesión: worktree o repo local

![Selector de worktree](img/ghapp-28.png)

Antes de lanzar la primera sesión, la app pregunta **dónde ejecutar el trabajo**:

- **New worktree** — crea una copia aislada del repo solo para esta sesión. Recomendado: el agente trabaja en su propia rama sin interferir con el código principal.
- **Local repository** — trabaja directamente sobre el repositorio ya clonado en la máquina.

> Para trabajo agéntico, `New worktree` es la opción segura: cada sesión vive en su propio espacio.

#### Subpunto técnico — qué diferencia hay realmente

`New worktree` usa `git worktree` para crear un directorio de trabajo adicional del mismo repositorio, normalmente asociado a una rama dedicada para la sesión del agente.

Diferencias clave:

- **Aislamiento de cambios**: cada worktree tiene su propio estado de archivos y su propia rama activa.
- **Menos riesgo operativo**: evitas mezclar cambios del agente con tu working directory principal (staged files, stashes o cambios locales).
- **Modelo Git correcto para paralelismo**: puedes tener varias sesiones/agentes trabajando en paralelo en ramas distintas sin pisarse.
- **Mismo historial, distinta carpeta**: los worktrees comparten el objeto Git del repo, pero cada uno tiene su checkout independiente.

`Local repository` en cambio ejecuta sobre tu checkout principal. Es útil para iteraciones rápidas, pero aumenta el riesgo de interferencia con trabajo manual en curso.

#### Subpunto — Estructura de archivos en disco (worktree)

![Estructura de carpetas](img/ghapp-21.png)

La app organiza el trabajo en el sistema de archivos de forma predecible:

```text
GitHubApp/
└── copilot-worktrees/
    └── ghapp_demo/
        └── jmfloreszazo-automatic-goggles/   <- worktree de la sesion
            └── RecursiveSearch/
                └── obj/
    ghapp_demo/                                 <- repo clonado base
    └── .git/
```

Cada sesión tiene su propio directorio aislado dentro de `copilot-worktrees`. No hay riesgo de que una sesión pise el trabajo de otra.

---

### Paso 6 — Configuración del proyecto: instrucciones y automatización

![Configuración del proyecto](img/ghapp-24.png)

En **Settings → Projects → ghapp_demo** se definen las reglas que el agente seguirá en todas las sesiones de ese repositorio:

```
Use .NET 10. Only use official Microsoft NuGet packages.
Do not use third-party NuGet packages.
```

Otros parámetros configurables:

- **Default branch**: `main`
- **Repository config file**: `.github/github-app.yml` (compartible con el equipo)
- **Remote control**: acceso a sesiones desde GitHub web y móvil (comando `/remote`)
- **Auto-start issue sessions**: arranca automáticamente una sesión cuando se abre un issue

---

### Paso 7 — Skills disponibles para el agente

![Skills del agente](img/ghapp-26.png)

La pestaña **Skills** muestra las capacidades especializadas que el agente puede usar. En este caso hay 30 skills instaladas (28 en el dispositivo + 2 integradas), todas habilitadas. Ejemplos:

- `airunway-aks-setup`
- `appinsights-instrumentation`
- `azure-ai`, `azure-aigateway`, `azure-cloud-migrate`
- `azure-compliance`, `azure-compute`, `azure-cost`
- `azure-deploy`, `azure-diagnostics`
- `azure-enterprise-infra-planner`
- `azure-hosted-copilot-sdk`

Las skills amplían lo que el agente sabe hacer de forma especializada, como seguir convenciones de proyecto o revisar código con criterios concretos.

---

### Paso 8 — Primera sesión: el prompt inicial

![Sesión arrancando](img/ghapp-22.png)

Con el repositorio configurado, se lanza la primera sesión con el prompt:

> *"Perform a recursive search on the input folder. Expose the folder path and the file name as CLI arguments."*

La app crea el worktree (`jmfloreszazo-automatic-goggles`), obtiene los últimos cambios del repo y el agente empieza a trabajar. En este punto la sesión aparece en el panel izquierdo como **New session** bajo el proyecto `ghapp_demo`.

---

### Paso 9 — El agente construye el código: diff en tiempo real

![Diff del código generado](img/ghapp-16.png)

El panel derecho muestra el **diff** de los cambios mientras el agente escribe. Aquí el agente generó `RecursiveSearch/Program.cs` con +77 líneas:

```csharp
using System.CommandLine;
using System.CommandLine.Parsing;

var folderOption = new Option<DirectoryInfo>("--folder")
{
    Description = "Root folder path to search in.",
    Required = true
};
folderOption.Aliases.Add("-f");

var fileNameOption = new Option<string>("--filename")
{
    Description = "File name (or pattern, e.g. *.txt) to search for.",
    Required = true
};
fileNameOption.Aliases.Add("-n");

var rootCommand = new RootCommand("Recursively searches for a file inside a folder.")
{
    folderOption,
    fileNameOption
};

rootCommand.SetAction((ParseResult result) =>
{
    var folder = result.GetValue(folderOption)!;
    var fileName = result.GetValue(fileNameOption)!;

    if (!folder.Exists)
```

El agente detectó que la API de `System.CommandLine 3.x preview` había cambiado (descripción como propiedad, no como parámetro del constructor) y corrigió el código automáticamente.

---

### Paso 10 — El agente resuelve errores de compilación por su cuenta

![Razonamiento del agente ante errores](img/ghapp-18.png)

Cuando el build falló, el agente mostró su razonamiento interno (*Thought for 24s*):

> *"The new preview of System.CommandLine (3.0.0-preview) has a different API... Option\<T\> constructor doesn't have description parameter in this version. IsRequired property doesn't exist. SetHandler extension method doesn't exist. InvokeAsync doesn't exist..."*

El agente buscó la API correcta para la versión 3.x, adaptó el código, recompiló y verificó que el build era exitoso antes de continuar. No requirió intervención manual.

---

### Paso 11 — Abrir herramientas (VS Code, Terminal, PowerShell) desde la app

![Selector de herramientas](img/ghapp-19.png)

El botón **Open** en la barra superior permite abrir el worktree activo directamente en diferentes herramientas:

- **Visual Studio Code**
- **Windows Terminal**
- **PowerShell**
- **Command Prompt**
- O añadir una aplicación personalizada

Esto permite cambiar rápidamente entre editor y terminales sin abandonar el contexto de la sesión.

---

### Paso 12 — El código en VS Code

![VS Code con el código generado](img/ghapp-14.png)

En VS Code se puede ver el proyecto completo generado por el agente:

```
JMFLORESZAZO-AUTOMATIC-GOGGLES/
└── RecursiveSearch/
    ├── bin/
    ├── obj/
    ├── Program.cs          ← app principal
    ├── RecursiveSearch.csproj
    ├── .gitignore
    └── README.md
```

El panel derecho de VS Code muestra el chat de la sesión activa para continuar interactuando con el agente desde el editor.

---

### Paso 13 — Uso de CLI dentro del entorno de la app

![Confirm folder trust](img/ghapp-01.png)

Este paso muestra que puedes usar CLI directamente (Terminal, PowerShell o Command Prompt) sin salir del entorno de la app. La primera vez que el agente necesita ejecutar código en el worktree, aparece un diálogo de **confirmación de confianza**:

```
C:\sources\Demos\GitHubApp\copilot-worktrees\ghapp_demo\jmfloreszazo-automatic-goggles
```

> *"Copilot may read files in this folder... With your permission, Copilot may execute code or bash commands in this folder. Executing untrusted code is unsafe."*

Opciones de confianza:
1. **Yes** — confiar para esta sesión
2. **Yes, and remember this folder for future sessions**
3. **No (Esc)**

Una vez concedida la confianza, puedes ejecutar comandos de CLI en el worktree desde la propia experiencia de la app.

---

### Paso 14 — Flujo de PR unificado (paso a paso)

1. **Aparece la opción Create PR**  
![Opción Create PR en el menú](img/ghapp-07.png)

Una vez listo el código y pasados los tests, aparece el botón **Create PR** con tres opciones:

- **Agent Merge** — el agente gestiona comentarios de revisión, CI y hace el merge cuando se cumplen las condiciones
- **Create PR** — abre un pull request estándar para revisión humana
- **Create draft PR** — PR en borrador, aún no listo para revisión

2. **El agente ejecuta los tests y valida resultados**  
![Tests pasando en terminal](img/ghapp-06.png)

El agente ejecutó `dotnet test` en el terminal integrado y el resultado fue:

```
Resumen de pruebas: total: 7; con errores: 0; correcto: 7; omitido: 0
Duración: 2,0 s
Compilación realizado correctamente en 3,7 s
```

Los 7 tests definidos cubren los escenarios clave:

| Test | Verifica |
|---|---|
| `FindsAllTxtFilesRecursively` | Patrón glob en directorios anidados |
| `FindsAllLogFilesRecursively` | Patrón glob con extensión diferente |
| `ExactFileNameMatch_ReturnsOnlyThatFile` | Coincidencia exacta de nombre |
| `NoMatchingFiles_ReturnsEmpty` | Sin resultados cuando no hay coincidencia |
| `WildcardStar_ReturnsAllFiles` | `*` devuelve todos los archivos |
| `EmptyFolder_ReturnsEmpty` | Carpeta raíz vacía no devuelve nada |
| `FindsFileInDeeplyNestedDirectory` | El traversal llega a subdirectorios profundos |

3. **Se prepara el PR desde la app**  
![PR abierto en GitHub](img/ghapp-12.png)

Desde la app queda preparado el PR con una descripción completa y bien estructurada.

4. **Se confirma Squash and merge**  
![Squash and merge](img/ghapp-03.png)

Desde el PR en GitHub, el merge se hace con **Squash and merge**.

5. **El PR queda marcado como Merged**  
![PR merged](img/ghapp-02.png)

Tras el merge, el PR aparece con estado **Merged** y se confirma la integración en `main`.

6. **Se revisa el estado final en la app**  
![Vista PR en la app](img/ghapp-15.png)

El panel derecho alterna entre **Changes**, **PR #1** y **Terminal**, con el historial completo de la sesión.

---

## ⚠️ La jugada maestra de Microsoft/GitHub: un golpe en la mesa

Seré directo: esto no es una herramienta más. Es una declaración de intenciones.

Llevamos meses viendo proliferar herramientas agent-first para desarrollo: **Claude Code** (Anthropic), **Cursor**, **Windsurf**, **Aider**, **Codeium**, **Devin**... Todas comparten el mismo enfoque: un agente que escribe código en tu máquina o en un sandbox remoto y te devuelve el resultado. Útiles, sí. Pero todas tienen el mismo problema estructural: **viven fuera de la plataforma donde ocurre el desarrollo real**.

GitHub Copilot App no. Y esa diferencia lo cambia todo.

### La fragmentación que nadie había resuelto

El flujo real de un desarrollador no es "escribir código". Es:

```
Issue → rama → código → tests → PR → revisión → comentarios → fix → merge
```

Con cualquier otra herramienta agent-first, ese flujo está roto en al menos tres sitios. El agente te genera código en el editor, pero luego tú tienes que abrir GitHub para crear el PR, volver al terminal para ejecutar los tests, responder comentarios manualmente y hacer el merge. La IA ayuda en el segmento más estrecho (escribir código) y te deja solo en el resto.

> **Nota importante** — Como habrás visto en los pasos anteriores, puedes ir saltando de una herramienta a otra sin ningún problema: trabajar **online** (GitHub web, móvil con `/remote`), en **local** (worktree aislado), desde **VS Code**, desde **Terminal/PowerShell**, o desde la propia **app**. La sesión es la misma en todos los lados, así que puedes moverte de una forma a otra sin ningún tipo de ruido ni pérdida de contexto.

### Lo que hace diferente a esta app

| Capacidad | Claude Code | Cursor / Windsurf | Devin | **GitHub Copilot App** |
|---|---|---|---|---|
| Escribe código agénticamente | ✅ | ✅ | ✅ | ✅ |
| Ejecuta tests en terminal integrado | ✅ | ✅ | ✅ | ✅ |
| Worktrees aislados por sesión | ❌ | ❌ | ✅ (sandbox) | ✅ |
| Abre PR desde la misma interfaz | ❌ | ❌ | ✅ | ✅ |
| Lee issues y contexto de GitHub | ❌ | ❌ | Parcial | ✅ nativo |
| Gestiona revisiones del PR | ❌ | ❌ | ❌ | ✅ (Agent Merge) |
| CI/CD integrado en el flujo | ❌ | ❌ | ❌ | ✅ |
| Abre VS Code / Terminal / PowerShell | N/A | N/A | ❌ | ✅ |
| Skills especializadas por proyecto | ❌ | ❌ | ❌ | ✅ |
| Funciona desde GitHub web y móvil | ❌ | ❌ | ❌ | ✅ (`/remote`) |

### La ventaja estructural: la plataforma

Aquí está la clave que ningún competidor puede replicar a corto plazo: **GitHub es la plataforma**. No un cliente externo que se conecta a la plataforma. La plataforma misma.

- **Claude Code** es brillante en el terminal, pero no sabe que existe un PR abierto con comentarios sin resolver. Necesitas cambiar de contexto.
- **Cursor y Windsurf** son excelentes editores aumentados con IA, pero su ámbito termina en el fichero. El PR, el issue, el CI... todo es externo.
- **Devin** fue el primero en demostrar el concepto de agente autónomo end-to-end, pero funciona en un entorno aislado que no es tu stack real, con acceso limitado al ecosistema GitHub y sin integración nativa en el flujo de revisión.
- **Antigravity, Bolt, v0**... están en el segmento de prototipado rápido de UI, un carril diferente.

GitHub Copilot App en cambio parte de donde ya está tu trabajo: **el issue ya existe, el repo ya existe, las branch policies ya existen, los reviewers ya existen**. El agente no necesita que le expliques el contexto; lo lee directamente.

### Agent-first como debía ser

La frase "agent-first" se ha devaluado por el uso. Todos dicen serlo. Pero agent-first de verdad significa que el agente puede completar una tarea de principio a fin sin que tú tengas que hacer de pegamento entre herramientas.

Esta app lo consigue porque puede:

1. Leer el issue → entender qué hay que hacer
2. Crear el worktree aislado → no rompe nada
3. Escribir el código → con razonamiento visible
4. Corregir errores de compilación → sin intervención
5. Ejecutar tests → verificar que todo funciona
6. Abrir el PR con descripción completa → en GitHub
7. Responder comentarios de revisión → Agent Merge
8. Hacer el merge cuando el CI pasa → end-to-end

Ninguna otra herramienta cubre los 8 puntos de forma nativa. Esta sí.

### El rol importa: la app se adapta a ti

Otro acierto de diseño que pasa desapercibido: **la app no impone un flujo único**. Dependiendo de tu rol y forma de trabajar puedes:

- Quedarte en la **app** si eres un tech lead que supervisa y revisa
- Saltar a **VS Code** si prefieres inspeccionar el código en tu editor
- Ir al **terminal** si necesitas ejecutar comandos propios
- Ir directamente a **GitHub.com** si tu flujo vive en el navegador
- Acceder desde **móvil** si solo quieres revisar el estado de una sesión

No es un walled garden. Es un hub que conecta con las herramientas que ya usas, pero que puede funcionar completamente solo si lo dejas.

### ✨ La nueva ventana de Agentes en VS Code (Preview)

![Ventana de Agentes en VS Code](img/new_feature.jpg)

Microsoft acaba de soltar otra pieza que encaja perfectamente en este flujo: la **Agents window** de VS Code, una ventana **dedicada y desacoplada del editor** pensada para un workflow agent-first nativo dentro de VS Code.

🔗 Documentación oficial: [code.visualstudio.com/docs/copilot/agents/agents-window](https://code.visualstudio.com/docs/copilot/agents/agents-window)

Qué aporta y por qué es relevante para lo que cuenta esta guía:

- **Ventana independiente del editor** — se abre con `code --agents` o desde el botón *Open in Agents*. Convive con tu VS Code normal sin ensuciar el espacio de trabajo principal.
- **Sesiones compartidas** — misma sesión de Copilot CLI, Copilot Cloud o Claude agent en la app, en VS Code y en la ventana de Agentes. Sin duplicar contexto, sin perder historial.
- **Multi-proyecto en paralelo** — listado de sesiones agrupado por workspace; saltas entre proyectos sin abrir una ventana por cada uno.
- **Worktree o folder isolation** — las mismas garantías de aislamiento que ya hemos visto en el Paso 5, pero seleccionables al crear cada sesión.
- **Sub-sesiones** — lanzar tareas paralelas dentro del mismo worktree sin contaminar el chat principal.
- **Sesiones remotas via SSH o dev tunnel** — el agente corre en otra máquina (hardware especializado, entornos específicos) y tú solo supervisas.
- **Panel de Changes con diff, Add Feedback, Commit/Merge/Discard** — revisión y aprobación integradas, sin saltar a otra herramienta.
- **Tasks, terminal integrada y navegador integrado** — validas el cambio (build, tests, `localhost`) sin salir de la ventana.
- **Customizaciones a mano** — Agents, Skills, Instructions, Hooks, MCP Servers y Plugins en un panel dedicado.

En la práctica refuerza exactamente la tesis de esta guía: **puedes elegir dónde quieres vivir** — la GitHub Copilot App, VS Code editor, la nueva Agents window, terminal, GitHub.com o móvil— y la sesión te sigue. Es la pieza que faltaba para un agent-first realmente integrado dentro del editor.

### Conclusión

Microsoft y GitHub tenían la plataforma. Siempre la tuvieron. Lo que les faltaba era la capa agéntica que la unificara. Ahora la tienen, y la han construido de forma que los competidores no pueden replicar simplemente añadiendo features: necesitarían tener GitHub. Y GitHub solo hay uno.

Esto no significa que Claude Code, Cursor o el resto vayan a desaparecer —cada uno tiene sus nichos y sus fortalezas—, pero en el segmento específico de **desarrollo agéntico integrado con el ciclo completo de vida del software**, la GitHub Copilot App acaba de poner el listón muy alto.

---

## Resumen del flujo completo

```
Configurar proyecto (instrucciones, skills, rama)
        ↓
Lanzar sesión con prompt en lenguaje natural
        ↓
El agente crea worktree aislado + escribe código
        ↓
Compilación y corrección automática de errores
        ↓
Ejecución de tests (todos pasan: 7/7)
        ↓
Commit + apertura de PR con descripción completa
        ↓
Review / Squash and merge
        ↓
Código en main ✓
```

---

## Disponibilidad

- **Copilot Pro / Pro+**: [solicitar acceso anticipado](https://gh.io/github-copilot-app)
- **Copilot Business / Enterprise**: acceso progresivo durante la semana del 14/05/2026 (requiere que el admin habilite previews y Copilot CLI en las políticas)
- **Documentación oficial**: [gh.io/github-copilot-app-docs-main](https://gh.io/github-copilot-app-docs-main)

---

🔗 **Fuente oficial**: [github.blog/changelog/2026-05-14-github-copilot-app-is-now-available-in-technical-preview](https://github.blog/changelog/2026-05-14-github-copilot-app-is-now-available-in-technical-preview/)

---

*Guía elaborada por Jose María Flores Zazo · [@jmfloreszazo](https://github.com/jmfloreszazo) · [jmfloreszazo.com](https://jmfloreszazo.com) · Mayo 2026*
