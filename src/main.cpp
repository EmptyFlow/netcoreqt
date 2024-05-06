#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include "netcorehost.h"
#include "concepthostevent.h"

int main(int argc, char *argv[])
{
    QGuiApplication app(argc, argv);

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
