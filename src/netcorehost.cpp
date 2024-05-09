#include <QDir>
#include <QThread>
#include "netcorehost.h"

NetCoreHost::NetCoreHost(QObject *parent)
    : QObject{parent}
{

}

bool NetCoreHost::loadRuntimeAssembly(const QString& rootPath, const QString &assemblyName, const QString &assemblyNamespace)
{
    if (!load_hostfxr(nullptr, nullptr)) {
        qDebug() << "Failed to load net core host";
        Q_ASSERT(false);
        return 1;
    }
    QDir dir(rootPath);
    auto configPath = dir.absolutePath() + "/" + assemblyName + ".runtimeconfig.json";

    qDebug() << "loadRuntimeAssembly: configpath=" << configPath;
    qDebug() << "loadRuntimeAssembly: rootpath=" << rootPath;


    char_t * configPathAsCString = stringToCharPointer(configPath);

    load_assembly_and_get_function_pointer = loadNetAssembly(configPathAsCString);

    Q_ASSERT(load_assembly_and_get_function_pointer != nullptr);
    if (load_assembly_and_get_function_pointer == nullptr) return false;

    qDebug() << "loadRuntimeAssembly: assemblyname=" << assemblyName;
    qDebug() << "loadRuntimeAssembly: assemblynamespace=" << assemblyNamespace;
    qDebug() << "loadRuntimeAssembly: set type library";

    loadedAssemblyName = assemblyName;
    loadedAssemblyNamespace = assemblyNamespace;
    loadedAssemblyPath = configPath;

    m_contextLoaded = true;
    m_hostedType = "library";
    emit contextLoadedChanged();

    return true;
}

bool NetCoreHost::loadApplicationSelfHostedAssembly(const QString& rootPath, const QString &assemblyName, const QString &assemblyNamespace)
{
    QDir dir(rootPath);
    auto configPath = dir.absolutePath() + "/" + assemblyName + ".dll";
    char_t * configPathAsCString = stringToCharPointer(configPath);
    char_t * rootPathAsCString = stringToCharPointer(rootPath);

    qDebug() << "loadApplicationSelfHostedAssembly: configpath=" << configPath;
    qDebug() << "loadApplicationSelfHostedAssembly: rootpath=" << dir.absolutePath();

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

    qDebug() << "loadApplicationSelfHostedAssembly: assemblyname=" << assemblyName;
    qDebug() << "loadApplicationSelfHostedAssembly: assemblynamespace=" << assemblyNamespace;
    qDebug() << "loadApplicationSelfHostedAssembly: set type application";

    loadedAssemblyName = assemblyName;
    loadedAssemblyNamespace = assemblyNamespace;
    loadedAssemblyPath = configPath;

    m_contextLoaded = true;
    m_hostedType = "application";
    emit contextLoadedChanged();

    return true;
}

bool NetCoreHost::loadApplicationAssembly(const QString& rootPath, const QString &assemblyName, const QString &assemblyNamespace) {
    QDir dir(rootPath);
    auto fullRootPath = dir.absolutePath();

    auto configPath = fullRootPath + "/" + assemblyName + ".dll";
    char_t * configPathAsCString = stringToCharPointer(configPath);

    qDebug() << "loadApplicationAssembly: configpath=" << configPath;
    qDebug() << "loadApplicationAssembly: rootpath=" << dir.absolutePath();

    if (!load_hostfxr(configPathAsCString, nullptr)) {
        qDebug() << "Failed to load net core host";
        Q_ASSERT(false);
        return 1;
    }

    m_context = nullptr;
    std::vector<const char_t*> args { configPathAsCString };
    int rc = init_for_cmd_line_fptr(args.size(), args.data(), nullptr, &m_context);
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

    qDebug() << "loadApplicationAssembly: assemblyname=" << assemblyName;
    qDebug() << "loadApplicationAssembly: assemblynamespace=" << assemblyNamespace;
    qDebug() << "loadApplicationAssembly: set type application";

    loadedAssemblyName = assemblyName;
    loadedAssemblyNamespace = assemblyNamespace;

    m_contextLoaded = true;
    m_hostedType = "application";
    emit contextLoadedChanged();

    return true;
}

void NetCoreHost::startContext()
{
    run_app_fptr(m_context);
}

void NetCoreHost::startContextInSeparateThread()
{
    m_threadWorker = new NetCoreHostWorker();
    m_threadWorker->setupContext(m_context, run_app_fptr, close_fptr);

    QThread* thread = new QThread(m_threadWorker);

    connect(thread, &QThread::started, m_threadWorker, &NetCoreHostWorker::needStartContext);
    connect(m_threadWorker, &NetCoreHostWorker::finished, thread, &QThread::quit);

    m_threadWorker->moveToThread(thread);
    thread->start();
    qDebug() << "Thread started";
}

void NetCoreHost::closeContext() const noexcept
{
    close_fptr(m_context);
}

bool NetCoreHost::getLibraryMethod(const QString &fullNamespace, const QString &className, const QString &methodName, void *delegate)
{
    auto rc = load_assembly_and_get_function_pointer(
        stringToCharPointer(loadedAssemblyPath),
        stringToCharPointer(fullNamespace + "." + className + ", " + loadedAssemblyName),
        stringToCharPointer(methodName),
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr,
        (void**)delegate);
    Q_ASSERT(rc == 0);
    Q_ASSERT(delegate != nullptr);

    return rc == 0 && delegate != nullptr;
}

bool NetCoreHost::getMethod(const QString &fullNamespace, const QString &className, const QString &methodName, void *delegate)
{
    if (m_hostedType == "library") {
        return getLibraryMethod(fullNamespace, className, methodName, delegate);
    }
    if (m_hostedType == "application") {
        return getApplicationMethod(fullNamespace, className, methodName, delegate);
    }

    qDebug() << "Hosted type not specified! You need use one from loadX methods.";

    return false;
}

bool NetCoreHost::getApplicationMethod(const QString &fullNamespace, const QString &className, const QString &methodName, void* delegate)
{
    auto qualifiedName = fullNamespace + "." + className + ", " + loadedAssemblyName;
    auto rc = m_getFunctionPointer(
        stringToCharPointer(qualifiedName),
        stringToCharPointer(methodName),
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr, nullptr,(void**) delegate
    );

    if (rc != 0) return false;
    if (delegate == nullptr) return false;

    return true;
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

load_assembly_and_get_function_pointer_fn NetCoreHost::loadNetAssembly(const char_t *config_path)
{
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
