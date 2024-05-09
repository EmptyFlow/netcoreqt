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
            //const loaded = netHost.loadApplicationAssembly("../../dlls", "NetCoreQtLibrary", "NetCoreQtLibrary");
            //const loaded = netHost.loadApplicationSelfHostedAssembly("../../selfhosteddlls", "NetCoreQtLibrary", "NetCoreQtLibrary");
            const loaded = netHost.loadRuntimeAssembly("../../runtimeassemblydlls", "NetCoreQtLibrary", "NetCoreQtLibrary");
            if (!loaded) {
                console.log("Can't load library!");
                return;
            }
        }
    }

    Rectangle {
        color: "red"
        width: 100
        height: 100

        MouseArea {
            anchors.fill: parent
            onPressed: {
                netHost.startContextInSeparateThread();
            }
        }
    }

    ConceptHostEvent {
        id: conceptHostEvent
        netHost: netHost
        onEventReceivedFromNet: function (count, distance) {
            console.log("event Received!!!!");
            console.log(count);
            console.log(distance);
        }
    }
}
