#ifndef CONCEPTHOSTEVENT_H
#define CONCEPTHOSTEVENT_H

#include <QObject>
#include <QQmlEngine>
#include "netcorehost.h"

class ConceptEvent: public QObject
{
    Q_OBJECT
    Q_PROPERTY(int count READ count WRITE setcount NOTIFY countChanged FINAL)
    Q_PROPERTY(int distance READ distance WRITE setdistance NOTIFY distanceChanged FINAL)
private:
    int m_count { 0 };
    int m_distance { 0 };

public:
    explicit ConceptEvent(QObject *parent = nullptr) {
    }

    int count() const noexcept { return m_count; }
    void setcount(int count) noexcept { m_count = count; }

    int distance() const noexcept { return m_distance; }
    void setdistance(int distance) noexcept { m_distance = distance; }

signals:
    void countChanged();
    void distanceChanged();

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
    typedef void (CORECLR_DELEGATE_CALLTYPE* completeEventDelegate)(int eventId);
    typedef int (CORECLR_DELEGATE_CALLTYPE* getCountDelegate)(int eventId);
    typedef int (CORECLR_DELEGATE_CALLTYPE* getDistanceDelegate)(int eventId);
    eventReceivedCallback eventReceived;
    completeEventDelegate completeEvent;
    getCountDelegate getCount;
    getDistanceDelegate getDistance;

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
        fireEventMethod(ConceptHostEvent::callbackEventReceived);

        m_netHost->getVoidPointerMethod("MyEventExternal", "CompleteEvent", false, (void**)&completeEvent);
        if (completeEvent == nullptr) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.CompleteEvent";
            return;
        }
        m_netHost->getVoidPointerMethod("MyEventExternal", "GetCount", false, (void**)&getCount);
        if (getCount == nullptr) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.GetCount";
            return;
        }
        m_netHost->getVoidPointerMethod("MyEventExternal", "GetDistance", false, (void**)&getDistance);
        if (getDistance == nullptr) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.GetDistance";
            return;
        }
    }

    void mapEvent(int eventId, ConceptEvent& event) {
        event.setcount(getCount(eventId));
        event.setdistance(getDistance(eventId));
        // destroy event on Net side
        completeEvent(eventId);
    }

private:
    static void callbackEventReceived(int eventId){
        auto instance = static_cast<ConceptHostEvent*>(ConceptHostEventInstance);
        ConceptEvent event;
        instance->mapEvent(eventId, event);
        //emit signal to external subscribers
        emit instance->eventReceivedFromNet(event);
    }

signals:
    void netHostChanged();
    void eventReceivedFromNet(const ConceptEvent& event);

};

#endif // CONCEPTHOSTEVENT_H
