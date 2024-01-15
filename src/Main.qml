import QtQuick
import QtQuick.Window
import AppBackend

Window {
    width: 640
    height: 480
    visible: true
    title: qsTr("Hello World")


    NetCoreHost {
        id: netHost
        Component.onCompleted: {
            const loaded = netHost.loadApplicationAssembly("../../dlls", "NetCoreQtLibrary", "NetCoreQtLibrary");
            if (!loaded) {
                console.log("Can't load library!");
                return;
            }

            //netHost.startContext();
        }
    }

    ConceptHostEvent {
        id: conceptHostEvent
        netHost: netHost
        onEventReceivedFromNet: function (event) {
            console.log(event.count);
            console.log(event.distance);
        }
    }
}
