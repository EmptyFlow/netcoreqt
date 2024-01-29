#ifndef NETCOREHOSTWORKER_H
#define NETCOREHOSTWORKER_H

#include <QObject>
#include <hostfxr.h>

class NetCoreHostWorker : public QObject
{
    Q_OBJECT

private:
    hostfxr_handle m_context = nullptr;
    hostfxr_run_app_fn m_runContext = nullptr;
    hostfxr_close_fn m_closeContext = nullptr;

public:
    explicit NetCoreHostWorker(QObject *parent = nullptr);

    void setupContext(hostfxr_handle context, hostfxr_run_app_fn runContext, hostfxr_close_fn closeContext) noexcept;

public slots:
    void needStartContext();
    void needCloseContext();

signals:
    void finished();

};

#endif // NETCOREHOSTWORKER_H
