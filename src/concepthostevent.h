#ifndef CONCEPTHOSTEVENT_H
#define CONCEPTHOSTEVENT_H

#include <QObject>
#include <QQmlEngine>
#include "netcorehost.h"

class ConceptEvent: public QObject
{
    Q_OBJECT
    Q_PROPERTY(int intProperty READ intProperty NOTIFY intPropertyChanged FINAL)
private:
    int m_intProperty { 0 };

public:
    explicit ConceptEvent(QObject *parent = nullptr) {
    }

    int intProperty() const noexcept { return m_intProperty; }
    void setintProperty(int intProperty) noexcept { m_intProperty = intProperty; }

signals:
    void intPropertyChanged();

};

static void* ConceptHostEventInstance { nullptr }; // it can be only one instance

class ConceptHostEvent : public QObject
{
    Q_OBJECT
    Q_PROPERTY(NetCoreHost* netHost READ netHost WRITE setNetHost NOTIFY netHostChanged FINAL)

    QML_ELEMENT

private:
    NetCoreHost* m_netHost { nullptr };

    typedef void (CORECLR_DELEGATE_CALLTYPE* eventReceivedCallback)(int eventId);
    typedef void (CORECLR_DELEGATE_CALLTYPE* fireEventCallback)(eventReceivedCallback callback);
    eventReceivedCallback eventReceived;

public:
    explicit ConceptHostEvent(QObject *parent = nullptr) {
        ConceptHostEventInstance = this;
    }

    NetCoreHost* netHost() const noexcept { return m_netHost; }
    void setNetHost(const NetCoreHost* netHost) noexcept {
        if (m_netHost == netHost) return;

        m_netHost = const_cast<NetCoreHost *>(netHost);
        emit netHostChanged();

        fireEventCallback fireEventMethod = nullptr;
        m_netHost->getVoidPointerMethod("MyEventExternal", "FireEventCallback", false, (void**)&fireEventMethod);
        if (fireEventMethod == nullptr) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.FireEventCallback";
            return;
        }

        fireEventMethod(ConceptHostEvent::callbackTest);
    }

private:
    static void callbackTest(int eventId){
        auto instance = static_cast<ConceptHostEvent*>(ConceptHostEventInstance);
        ConceptEvent event;
        emit instance->eventReceivedFromNet(event);
    }

signals:
    void netHostChanged();
    void eventReceivedFromNet(const ConceptEvent& event);

};

#endif // CONCEPTHOSTEVENT_H
