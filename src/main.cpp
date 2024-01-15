#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include "netcorehost.h"
#include "concepthostevent.h"

int main(int argc, char *argv[])
{
    QGuiApplication app(argc, argv);

    /*NetCoreHost host;
    //if (!host.loadAssemblyAndHost("NetCoreQtLibrary", "NetCoreQtLibrary")) {
    //auto root = "C:/work/Experiments/hosting/bin/Debug";
    auto root = "../../dlls";
    //if (!host.loadAssemblyAndRun(root, "App", "App")) {
    if (!host.loadApplicationAssembly(root, "NetCoreQtLibrary", "NetCoreQtLibrary")) {
        return 1;
    }

    typedef unsigned char (CORECLR_DELEGATE_CALLTYPE* is_waiting_fn)(int objectId, int value);
    is_waiting_fn is_waiting;

    host.getApplicationMethod("NetCoreQtLibrary","NetCoreQtImportGlobal", "SetGlobalInt32", &is_waiting);

    is_waiting(0,0);

    qDebug() << "LOADED!!!!";

    host.startContext();

    qDebug() << "FINISHED!!!!";

    host.closeContext();

    return 0;*/

    QQmlApplicationEngine engine;

    qmlRegisterType<NetCoreHost>("AppBackend", 1, 0, "NetCoreHost");
    qmlRegisterType<ConceptHostEvent>("AppBackend", 1, 0, "ConceptHostEvent");
    qmlRegisterType<ConceptEvent>("AppBackend", 1, 0, "ConceptEvent");

    const QUrl url(u"qrc:/netcoreqt/Main.qml"_qs);
    QObject::connect(
        &engine,
        &QQmlApplicationEngine::objectCreationFailed,
        &app,
        []() { QCoreApplication::exit(-1); },
        Qt::QueuedConnection);
    engine.load(url);

    return app.exec();
}
