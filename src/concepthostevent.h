#ifndef CONCEPTHOSTEVENT_H
#define CONCEPTHOSTEVENT_H

#include <QObject>
#include <QQmlEngine>
#include <QQueue>
#include <QMutex>
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
    QQueue<ConceptEvent*> m_receivedEvents { QQueue<ConceptEvent*>() };
    QMutex m_mutex { QMutex() };

    typedef void (CORECLR_DELEGATE_CALLTYPE* eventReceivedCallback)(int eventId);
    typedef void (CORECLR_DELEGATE_CALLTYPE* fireEventCallback)(void* callback);
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
        connect(this, &ConceptHostEvent::processQueuedEvents, this, &ConceptHostEvent::needProcessQueue, Qt::QueuedConnection);
    }

    NetCoreHost* netHost() const noexcept { return m_netHost; }
    void setNetHost(const NetCoreHost* netHost) noexcept {
        if (netHost == nullptr) return;
        if (m_netHost == netHost) return;

        m_netHost = const_cast<NetCoreHost *>(netHost);
        emit netHostChanged();

        if (!m_netHost->contextLoaded()) {
            connect(m_netHost, &NetCoreHost::contextLoadedChanged, this, &ConceptHostEvent::netContextLoadedChanged);
        } else {
            initializeMethods();
        }
    }

    void mapEvent(int eventId) {
        QMutexLocker locker(&m_mutex); // to be sure we don't have race condition issues

        auto newEvent = new ConceptEvent(this);
        newEvent->setcount(getCount(eventId));
        newEvent->setdistance(getDistance(eventId));
        m_receivedEvents.append(newEvent);

        // destroy event on Net side
        completeEvent(eventId);
    }

private:
    void initializeMethods() {
        fireEventCallback fireEventMethod = nullptr;
        m_netHost->getMethod("NetCoreQt.Generator.CodeSaver", "MyEventExternal", "FireEventCallback", &fireEventMethod);
        if (fireEventMethod == nullptr) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.FireEventCallback";
            return;
        }
        fireEventMethod((void*)&ConceptHostEvent::callbackEventReceived);

        auto completeProcessed = m_netHost->getMethod("NetCoreQt.Generator.CodeSaver", "MyEventExternal", "CompleteEvent", &completeEvent);
        if (!completeProcessed) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.CompleteEvent";
            return;
        }
        auto getCountProcessed = m_netHost->getMethod("NetCoreQt.Generator.CodeSaver", "MyEventExternal", "GetCount", &getCount);
        if (!getCountProcessed) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.GetCount";
            return;
        }
        auto getDistanceProcessed = m_netHost->getMethod("NetCoreQt.Generator.CodeSaver", "MyEventExternal", "GetDistance", &getDistance);
        if (!getDistanceProcessed) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.GetDistance";
            return;
        }
    }

    static void callbackEventReceived(int eventId){
        auto instance = static_cast<ConceptHostEvent*>(ConceptHostEventInstance);
        instance->mapEvent(eventId);
        emit instance->processQueuedEvents();
    }

private slots:
    void netContextLoadedChanged() {
        if (m_netHost == nullptr) return;

        if (m_netHost->contextLoaded()) initializeMethods();
    }

    void needProcessQueue() {
        if (m_receivedEvents.isEmpty()) return;

        QMutexLocker locker(&m_mutex); // to be sure we don't have race condition issues

        foreach (auto receivedEvent, m_receivedEvents) {
            emit eventReceivedFromNet(receivedEvent);
        }

        m_receivedEvents.clear();
    }

signals:
    void netHostChanged();
    void processQueuedEvents();
    void eventReceivedFromNet(const ConceptEvent* event);

};

#endif // CONCEPTHOSTEVENT_H
