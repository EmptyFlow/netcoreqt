#include <nethost.h>
#include <hostfxr.h>
#include <coreclr_delegates.h>
#include <QString>

extern hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr;
extern hostfxr_initialize_for_runtime_config_fn init_for_config_fptr;
extern hostfxr_get_runtime_delegate_fn get_delegate_fptr;
extern hostfxr_run_app_fn run_app_fptr;
extern hostfxr_close_fn close_fptr;
extern load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;
extern QString loadedAssemblyName;
extern QString loadedAssemblyNamespace;
extern QString loadedAssemblyPath;

void *load_library(const char_t *);
void *get_export(void *, const char *);
bool load_hostfxr(const char_t *assembly_path);
load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t *config_path);

char_t* stringToCharPointer(const QString& value) noexcept;
bool loadAssemblyAndHost(const QString& assemblyName, const QString& assemblyNamespace);
void runSinglePointerMethod(const QString& className, const QString& methodName);

