#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include "netcorehost.h"

int main(int argc, char *argv[])
{
    QGuiApplication app(argc, argv);

    NetCoreHost host;
    //if (!host.loadAssemblyAndHost("NetCoreQtLibrary", "NetCoreQtLibrary")) {
    auto root = QCoreApplication::applicationDirPath() + "/netcore";
    if (!host.loadAssemblyForSelfHosted(root, "NetCoreQtLibrary", "NetCoreQtLibrary")) {
        return 1;
    }

    qDebug() << "LOADED!!!!";

    host.startContext();

    qDebug() << "FINISHED!!!!";

    host.closeContext();

    return 0;

    /*QQmlApplicationEngine engine;
    const QUrl url(u"qrc:/netcoreqt/Main.qml"_qs);
    QObject::connect(
        &engine,
        &QQmlApplicationEngine::objectCreationFailed,
        &app,
        []() { QCoreApplication::exit(-1); },
        Qt::QueuedConnection);
    engine.load(url);

    return app.exec();*/
}
