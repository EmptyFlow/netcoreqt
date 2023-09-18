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
private:
    hostfxr_initialize_for_dotnet_command_line_fn init_for_cmd_line_fptr = nullptr;
    hostfxr_initialize_for_runtime_config_fn init_for_config_fptr = nullptr;
    hostfxr_get_runtime_delegate_fn get_delegate_fptr = nullptr;
    hostfxr_run_app_fn run_app_fptr = nullptr;
    hostfxr_close_fn close_fptr = nullptr;
    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
    QString loadedAssemblyName = "";
    QString loadedAssemblyNamespace = "";
    QString loadedAssemblyPath = "";

public:
    explicit NetCoreHost(QObject *parent = nullptr);

    bool loadAssemblyAndHost(const QString &assemblyName, const QString &assemblyNamespace);
    template <typename T>
    void getPointerMethod(const QString &className, const QString &methodName, bool haveDelegate, T delegate);

private:
    void *load_library(const char_t *path);
    void *get_export(void *h, const char *name);
    bool load_hostfxr(const char_t *assembly_path);
    load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t *config_path);
    char_t *stringToCharPointer(const QString &value) noexcept;

signals:

};

#endif // NETCOREHOST_H
