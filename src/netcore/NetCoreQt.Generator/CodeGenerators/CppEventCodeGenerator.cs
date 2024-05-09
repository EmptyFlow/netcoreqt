using NetCoreQt.Generator.SchemaParsers;

namespace NetCoreQt.Generator.CodeGenerators {

    internal class CppEventCodeGenerator {

        public static string GenerateGuestEvent ( GenerateEvent item, string defaultNamespace ) {
            return $$"""
class {{item.Name}}: public QObject
{
    Q_OBJECT
    {{GetCppDecalrationEventProperties ( item.Properties )}}

private:
    {{GetCppPrivateProperties ( item.Properties )}}

public:
    explicit {{item.Name}}(QObject *parent = nullptr) {
    }

{{GetPropertiesMethods ( item.Properties )}}

signals:
{{GetPropertiesNotifiers ( item.Properties )}}

};

static void* {{item.Name}}Instance { nullptr };

class {{item.Name}}Host : public QObject
{
    Q_OBJECT
    Q_PROPERTY(NetCoreHost* netHost READ netHost WRITE setNetHost NOTIFY netHostChanged FINAL)

    QML_ELEMENT

private:
    NetCoreHost* m_netHost { nullptr };
    QQueue<{{item.Name}}*> m_receivedEvents { QQueue<{{item.Name}}*>() };
    QMutex m_mutex { QMutex() };

    typedef void (CORECLR_DELEGATE_CALLTYPE* eventReceivedCallback)(int eventId);
    typedef void (CORECLR_DELEGATE_CALLTYPE* fireEventCallback)(void* callback);
    typedef void (CORECLR_DELEGATE_CALLTYPE* completeEventDelegate)(int eventId);
{{GetDelegateDefinitions ( item.Properties )}}
    eventReceivedCallback eventReceived;
    completeEventDelegate completeEvent;
{{GetDelegateProperties ( item.Properties )}}

public:
    explicit ConceptHostEvent(QObject *parent = nullptr) {
        {{item.Name}}Instance = this;
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
        {{string.Join ( "\n", item.Properties.Select (GetSetProperty) )}}
        m_receivedEvents.append(newEvent);

        // destroy event on Net side
        completeEvent(eventId);
    }

private:
    void initializeMethods() {
        fireEventCallback fireEventMethod = nullptr;
        m_netHost->getMethod("{{defaultNamespace}}", "{{item.Name}}External", "FireEventCallback", &fireEventMethod);
        if (fireEventMethod == nullptr) {
            qDebug() << "Can't get pointer for {{item.Name}}External.FireEventCallback";
            return;
        }
        fireEventMethod((void*)&ConceptHostEvent::callbackEventReceived);

        auto completeProcessed = m_netHost->getMethod("{{defaultNamespace}}", "{{item.Name}}External", "CompleteEvent", &completeEvent);
        if (!completeProcessed) {
            qDebug() << "Can't get pointer for NetCoreQtImportGlobal.CompleteEvent";
            return;
        }
        {{string.Join ( "\n", item.Properties.Select ( a => GetMethodPointer ( a, defaultNamespace ) ) )}}
    }

    static void callbackEventReceived(int eventId){
        auto instance = static_cast<{{item.Name}}Host*>(ConceptHostEventInstance);
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
    void eventReceivedFromNet(const {{item.Name}}* event);

};
""";
        }

        private static string GetTypeName ( PropertyType propertyType ) {
            return propertyType switch {
                PropertyType.Int32 => "int",
                PropertyType.Int64 => "QVariant",
                PropertyType.Double => "double",
                PropertyType.String => "QString",
                _ => throw new NotSupportedException ( $"Property type {propertyType} not supported in C++!" )
            };
        }
        private static string GetCppDecalrationEventProperties ( List<GenerateEventProperty> properties ) {
            return string.Join (
            "\n",
            properties.Select ( GenerateReadWriteQProperty )
            .ToList ()
            );
        }

        private static string GenerateReadWriteQProperty ( GenerateEventProperty property ) {
            var name = property.Name;
            name = name[0].ToString ().ToLowerInvariant () + string.Join ( "", name.Skip ( 1 ) );
            return property.Type switch {
                PropertyType.Int32 => $"Q_PROPERTY(int {name} READ {name} WRITE set{name} NOTIFY {name}Changed FINAL)",
                PropertyType.Int64 => $"Q_PROPERTY(QVariant {name} READ {name} WRITE set{name} NOTIFY {name}Changed FINAL)",
                PropertyType.Double => $"Q_PROPERTY(double {name} READ {name} WRITE set{name} NOTIFY {name}Changed FINAL)",
                PropertyType.String => $"Q_PROPERTY(QString {name} READ {name} WRITE set{name} NOTIFY {name}Changed FINAL)",
                _ => ""
            };
        }

        private static string GetCppPrivateProperties ( List<GenerateEventProperty> properties ) {
            return string.Join (
                "\n",
                properties.Select ( GeneratePrivateProperty )
                .ToList ()
            );
        }

        private static string GeneratePrivateProperty ( GenerateEventProperty property ) {
            var name = property.Name;
            name = name[0].ToString ().ToLowerInvariant () + string.Join ( "", name.Skip ( 1 ) );
            return property.Type switch {
                PropertyType.Int32 => $"int m_{name} {{ 0 }}",
                PropertyType.Int64 => $"QVariant m_{name} {{ 0 }}",
                PropertyType.Double => $"double m_{name} {{ 0 }}",
                PropertyType.String => $"QString m_{name} {{ \"\" }}",
                _ => ""
            };
        }

        private static string GetPropertiesMethods ( List<GenerateEventProperty> properties ) {
            return string.Join (
                "\n",
                properties.Select ( GenerateReadWritePropertyMethod )
                .ToList ()
            );
        }

        private static string GenerateReadWritePropertyMethod ( GenerateEventProperty property ) {
            return
$$"""
    {{GetTypeName ( property.Type )}} {{property.Name}} () const noexcept { return m_{{property.Name}}; }
    void set{{property.Name}} ( {{GetTypeName ( property.Type )}} {{property.Name}} ) noexcept { m_{{property.Name}} = {{property.Name}}; }
""";
        }

        private static string GetPropertiesNotifiers ( List<GenerateEventProperty> properties ) {
            return string.Join (
                "\n",
                properties.Select ( GetPropertyNotifier )
                .ToList ()
            );
        }
        private static string GetPropertyNotifier ( GenerateEventProperty property ) {
            return
$$"""
    void {{property.Name}}Changed();
""";
        }

        private static string GetDelegateDefinitions ( List<GenerateEventProperty> properties ) {
            return string.Join (
                "\n",
                properties.Select ( GetDelegateDefinition )
                .ToList ()
            );
        }
        private static string GetDelegateDefinition( GenerateEventProperty property ) {
            return
$"""
    typedef int (CORECLR_DELEGATE_CALLTYPE* get{property.Name}Delegate)(int eventId);
""";
        }

        private static string GetDelegateProperties ( List<GenerateEventProperty> properties ) {
            return string.Join (
                "\n",
                properties.Select ( GetDelegateProperty )
                .ToList ()
            );
        }
        private static string GetDelegateProperty ( GenerateEventProperty property ) {
            return
$"""
    get{property.Name}Delegate get{property.Name};
""";
        }

        private static string GetMethodPointer(GenerateEventProperty property, string defaultNamespace) {
            return
$$"""
        auto get{{property.Name}}Processed = m_netHost->getMethod("{{defaultNamespace}}", "{{property.Name}}External", "Get{{property.Name}}", &get{{property.Name}});
        if (!get{{property.Name}}Processed) {
            qDebug() << "Can't get pointer for {{property.Name}}External.Get{{property.Name}}";
            return;
        }
""";
        }

        private static string GetSetProperty ( GenerateEventProperty property ) {
            return
$$"""
        newEvent->set{{property.Name}}(get{{property.Name}}(eventId));
""";
        }

    }

}
