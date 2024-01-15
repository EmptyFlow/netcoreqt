#ifndef NETCOREHOST_H
#define NETCOREHOST_H

#include <qglobal.h>
#include <QDebug>
#include <QCoreApplication>
#include <nethost.h>
#include <hostfxr.h>
#include <coreclr_delegates.h>
#ifdef Q_OS_WIN
#include <windows.h>
#include <minwindef.h>
#include <assert.h>
#include <iostream>
#endif
#include <QObject>

class NetCoreHost : public QObject
{
    Q_OBJECT
    Q_PROPERTY(bool contextLoaded READ contextLoaded NOTIFY contextLoadedChanged FINAL)

private:
    hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr = nullptr;
    hostfxr_initialize_for_runtime_config_fn init_for_config_fptr = nullptr;
    hostfxr_get_runtime_delegate_fn get_delegate_fptr = nullptr;
    hostfxr_run_app_fn run_app_fptr = nullptr;
    hostfxr_close_fn close_fptr = nullptr;
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    get_function_pointer_fn m_getFunctionPointer = nullptr;
    hostfxr_handle m_context = nullptr;
    bool m_contextLoaded { false };
    QString loadedAssemblyName = "";
    QString loadedAssemblyNamespace = "";
    QString loadedAssemblyPath = "";
    typedef void (CORECLR_DELEGATE_CALLTYPE* setGlobalInt32Delegate)(int objectId, int value);
    setGlobalInt32Delegate setGlobalInt32Pointer;

public:
    explicit NetCoreHost(QObject *parent = nullptr);

    bool contextLoaded() const noexcept { return m_contextLoaded; }

    Q_INVOKABLE bool loadAssemblyAndHost(const QString &assemblyName, const QString &assemblyNamespace);
    Q_INVOKABLE bool loadApplicationSelfHostedAssembly(const QString& rootPath, const QString &assemblyName, const QString &assemblyNamespace);
    Q_INVOKABLE bool loadApplicationAssembly(const QString& rootPath, const QString &assemblyName, const QString &assemblyNamespace);
    Q_INVOKABLE void startContext();
    template <typename T>
    bool getPointerMethod(const QString &className, const QString &methodName, bool haveDelegate, T delegate);
    bool getVoidPointerMethod(const QString &className, const QString &methodName, bool haveDelegate, void* delegate);
    bool getApplicationMethod(const QString &fullNamespace, const QString &className, const QString &methodName, void* delegate);
    bool initializeGlobalObject(const QString &className);
    Q_INVOKABLE void closeContext() const noexcept;

private:
    void *load_library(const char_t *path);
    void *get_export(void *h, const char *name);
    bool load_hostfxr(const char_t *assembly_path, const char_t * dotnet_root);
    load_assembly_and_get_function_pointer_fn loadNetAssembly(const char_t *config_path);
    char_t *stringToCharPointer(const QString &value) noexcept;

signals:
    void contextLoadedChanged();

public slots:
    void startLoadedContext();

};

#endif // NETCOREHOST_H
