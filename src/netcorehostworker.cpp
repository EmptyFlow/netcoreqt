#include <QDebug>
#include <QThread>
#include "netcorehostworker.h"

NetCoreHostWorker::NetCoreHostWorker(QObject *parent)
    : QObject{parent}
{

}

void NetCoreHostWorker::setupContext(hostfxr_handle context, hostfxr_run_app_fn runContext, hostfxr_close_fn closeContext) noexcept
{
    m_context = context;
    m_runContext = runContext;
    m_closeContext = closeContext;
}

void NetCoreHostWorker::needStartContext()
{
    m_runContext(m_context);
    emit finished();
}

void NetCoreHostWorker::needCloseContext()
{
    m_closeContext(m_context);
}
