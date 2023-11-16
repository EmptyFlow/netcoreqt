#include "netcorehost.h"

NetCoreHost::NetCoreHost(QObject *parent)
    : QObject{parent}
{

}

bool NetCoreHost::loadAssemblyAndHost(const QString &assemblyName, const QString &assemblyNamespace)
{
    if (!load_hostfxr(nullptr, nullptr)) {
        qDebug() << "Failed to load net core host";
        Q_ASSERT(false);
        return 1;
    }
    auto rootPath = QCoreApplication::applicationDirPath();
    auto configPath = rootPath + "/" + assemblyName + ".runtimeconfig.json";

    char_t * configPathAsCString = stringToCharPointer(configPath);

    load_assembly_and_get_function_pointer = get_dotnet_load_assembly(configPathAsCString);

    Q_ASSERT(load_assembly_and_get_function_pointer != nullptr);
    if (load_assembly_and_get_function_pointer == nullptr) return false;

    loadedAssemblyName = assemblyName;
    loadedAssemblyNamespace = assemblyNamespace;
    loadedAssemblyPath = rootPath + "/" + assemblyName + ".dll";

    return true;
}

bool NetCoreHost::loadAssemblyForSelfHosted(const QString& rootPath, const QString &assemblyName, const QString &assemblyNamespace)
{
    auto configPath = rootPath + "/" + assemblyName + ".dll";
    char_t * configPathAsCString = stringToCharPointer(configPath);
    char_t * rootPathAsCString = stringToCharPointer(rootPath);

    if (!load_hostfxr(configPathAsCString, nullptr)) {
        qDebug() << "Failed to load net core host";
        Q_ASSERT(false);
        return 1;
    }

    //hostfxr_handle cxt = nullptr;
    m_context = nullptr;
    std::vector<const char_t*> args { configPathAsCString };
    hostfxr_initialize_parameters params { sizeof(hostfxr_initialize_parameters), rootPathAsCString, rootPathAsCString };
    int rc = init_for_cmd_line_fptr(args.size(), args.data(), &params, &m_context);
    if (rc != 0 || m_context == nullptr)
    {
        std::cerr << "Init failed: " << std::hex << std::showbase << rc << std::endl;
        close_fptr(m_context);
        return false;
    }

    // Get the function pointer to get function pointers
    rc = get_delegate_fptr(
        m_context,
        hdt_get_function_pointer,
        (void**)&m_getFunctionPointer);
    if (rc != 0 || m_getFunctionPointer == nullptr) {
        qDebug() << "Get delegate failed: " << rc;
        Q_ASSERT(false);
        return false;
    }

    return true;
}

void NetCoreHost::startContext()
{
    run_app_fptr(m_context);
}

bool NetCoreHost::initializeGlobalObject(const QString &className)
{
    if (!getPointerMethod("NetCoreQtImportGlobal", "SetGlobalInt32", false, setGlobalInt32Pointer)) return false;
    if (!getPointerMethod("NetCoreQtImportGlobal", "SetGlobalDouble", false, setGlobalDoublePointer)) return false;
    if (!getPointerMethod("NetCoreQtImportGlobal", "SetGlobalString", false, setGlobalStringPointer)) return false;

    return true;
}

void NetCoreHost::closeContext() const noexcept
{
    close_fptr(m_context);
}

template <typename T>
bool NetCoreHost::getPointerMethod(const QString &className, const QString &methodName, bool haveDelegate, T delegate)
{
    qDebug() << loadedAssemblyPath;
    qDebug() << loadedAssemblyNamespace + "." + className + ", " + loadedAssemblyName;

    auto rc = load_assembly_and_get_function_pointer(
        stringToCharPointer(loadedAssemblyPath),
        stringToCharPointer(loadedAssemblyNamespace + "." + className + ", " + loadedAssemblyName),
        stringToCharPointer(methodName),
        haveDelegate ? stringToCharPointer(loadedAssemblyNamespace + "." + className + "+" + className + "Delegate, " + loadedAssemblyName) : UNMANAGEDCALLERSONLY_METHOD,
        nullptr,
        (void**)&delegate);
    Q_ASSERT(rc == 0);
    Q_ASSERT(delegate != nullptr);

    return rc == 0 && delegate != nullptr;
}

void *NetCoreHost::load_library(const char_t *path)
{
#ifdef Q_OS_WIN
    HMODULE h = ::LoadLibraryW(path);
    assert(h != nullptr);
    return (void*)h;
#else
    void *h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
    assert(h != nullptr);
    return h;
#endif
}

void *NetCoreHost::get_export(void *h, const char *name)
{
#ifdef Q_OS_WIN
    void *f = ::GetProcAddress((HMODULE)h, name);
    assert(f != nullptr);
    return f;
#else
    void *f = dlsym(h, name);
    assert(f != nullptr);
    return f;
#endif
}

bool NetCoreHost::load_hostfxr(const char_t *assembly_path, const char_t * dotnet_root)
{
    get_hostfxr_parameters params { sizeof(get_hostfxr_parameters), assembly_path, dotnet_root };
    // Pre-allocate a large buffer for the path to hostfxr
    char_t buffer[MAX_PATH];
    size_t buffer_size = sizeof(buffer) / sizeof(char_t);
    int rc = get_hostfxr_path(buffer, &buffer_size, &params);
    if (rc != 0) return false;

    void *lib = load_library(buffer);
    init_for_cmd_line_fptr = (hostfxr_initialize_for_dotnet_command_line_fn)get_export(lib, "hostfxr_initialize_for_dotnet_command_line");
    init_for_config_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
    get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
    run_app_fptr = (hostfxr_run_app_fn)get_export(lib, "hostfxr_run_app");
    close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

    return (init_for_config_fptr && get_delegate_fptr && close_fptr && init_for_cmd_line_fptr);
}

load_assembly_and_get_function_pointer_fn NetCoreHost::get_dotnet_load_assembly(const char_t *config_path)
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

char_t *NetCoreHost::stringToCharPointer(const QString &value) noexcept
{
    char_t * result = new wchar_t[value.size()+1];;
    value.toWCharArray(result);
    result[value.size()] = L'\0';

    return result;
}

void NetCoreHost::startLoadedContext()
{
    startContext();
}
