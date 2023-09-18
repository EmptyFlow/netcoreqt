#include "hostinghelpers.h"
#include <QDebug>
#include <QCoreApplication>
//#ifdef Q_OS_WIN
#include <windows.h>
#include <minwindef.h>
#include <assert.h>
#include <iostream>
//#endif

hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr = nullptr;
hostfxr_initialize_for_runtime_config_fn init_for_config_fptr = nullptr;
hostfxr_get_runtime_delegate_fn get_delegate_fptr = nullptr;
hostfxr_run_app_fn run_app_fptr = nullptr;
hostfxr_close_fn close_fptr = nullptr;
load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
QString loadedAssemblyName = "";
QString loadedAssemblyNamespace = "";
QString loadedAssemblyPath = "";

//#ifdef Q_OS_WIN
void *load_library(const char_t *path)
{
    HMODULE h = ::LoadLibraryW(path);
    assert(h != nullptr);
    return (void*)h;
}
void *get_export(void *h, const char *name)
{
    void *f = ::GetProcAddress((HMODULE)h, name);
    assert(f != nullptr);
    return f;
}
/*#else
void *load_library(const char_t *path)
{
    void *h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
    assert(h != nullptr);
    return h;
}
void *get_export(void *h, const char *name)
{
    void *f = dlsym(h, name);
    assert(f != nullptr);
    return f;
}
#endif*/

bool load_hostfxr(const char_t *assembly_path)
{
    get_hostfxr_parameters params { sizeof(get_hostfxr_parameters), assembly_path, nullptr };
    // Pre-allocate a large buffer for the path to hostfxr
    char_t buffer[MAX_PATH];
    size_t buffer_size = sizeof(buffer) / sizeof(char_t);
    int rc = get_hostfxr_path(buffer, &buffer_size, &params);
    if (rc != 0) return false;

    // Load hostfxr and get desired exports
    void *lib = load_library(buffer);
    init_for_cmd_line_fptr = (hostfxr_initialize_for_dotnet_command_line_fn)get_export(lib, "hostfxr_initialize_for_dotnet_command_line");
    init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
    get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
    run_app_fptr = (hostfxr_run_app_fn)get_export(lib, "hostfxr_run_app");
    close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

    return (init_for_config_fptr && get_delegate_fptr && close_fptr);
}

// Load and initialize .NET Core and get desired function pointer for scenario
load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t *config_path)
{
    // Load .NET Core
    void *load_assembly_and_get_function_pointer = nullptr;
    hostfxr_handle cxt = nullptr;
    int rc = init_for_config_fptr(config_path, nullptr, &cxt);
    if (rc != 0 || cxt == nullptr)
    {
        std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
        close_fptr(cxt);
        return nullptr;
    }

    // Get the load assembly function pointer
    rc = get_delegate_fptr(cxt, hdt_load_assembly_and_get_function_pointer, &load_assembly_and_get_function_pointer);
    if (rc != 0 || load_assembly_and_get_function_pointer == nullptr) {
        std::cerr << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;
    }

    close_fptr(cxt);
    return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
}

bool loadAssemblyAndHost(const QString &assemblyName, const QString &assemblyNamespace)
{
    if (!load_hostfxr(nullptr)) {
        qDebug() << "Failed to load net core host";
        return 1;
    }
    auto rootPath = QCoreApplication::applicationDirPath();
    auto configPath = rootPath + "/" + assemblyName + ".runtimeconfig.json";

    char_t * configPathAsCString = new wchar_t[configPath.size()+1];;
    configPath.toWCharArray(configPathAsCString);
    configPathAsCString[configPath.size()] = L'\0';

    //const string_t config_path = root_path + STR("DotNetLib.runtimeconfig.json");
    load_assembly_and_get_function_pointer = get_dotnet_load_assembly(configPathAsCString);

    Q_ASSERT(load_assembly_and_get_function_pointer != nullptr);
    if (load_assembly_and_get_function_pointer == nullptr) return false;

    loadedAssemblyName = assemblyName;
    loadedAssemblyNamespace = assemblyNamespace;
    loadedAssemblyPath = rootPath + "/" + assemblyName + ".dll";

    return true;
}

struct lib_args
{
    const char_t* message;
    int number;
};

typedef void (CORECLR_DELEGATE_CALLTYPE* testDelegateMethodType)(lib_args args);

void testDelegateMethod(lib_args args) {
    qDebug() << "delegate runned!!!!";
    qDebug() << args.message;
    qDebug() << args.number;
}

void runSinglePointerMethod(const QString &className, const QString &methodName)
{
    typedef void (CORECLR_DELEGATE_CALLTYPE* test_delegate_fn)(testDelegateMethodType arg);
    test_delegate_fn testDelegate = nullptr;

    qDebug() << loadedAssemblyPath;
    qDebug() << loadedAssemblyNamespace + "." + className + ", " + loadedAssemblyName;

    auto rc = load_assembly_and_get_function_pointer(
        stringToCharPointer(loadedAssemblyPath),
        stringToCharPointer(loadedAssemblyNamespace + "." + className + ", " + loadedAssemblyName),
        stringToCharPointer("TestDelegate"),
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr,
        (void**)&testDelegate);
    Q_ASSERT(rc == 0);
    Q_ASSERT(testDelegate != nullptr);
    testDelegate(&testDelegateMethod);
}

char_t *stringToCharPointer(const QString &value) noexcept
{
    char_t * result = new wchar_t[value.size()+1];;
    value.toWCharArray(result);
    result[value.size()] = L'\0';

    return result;
}
