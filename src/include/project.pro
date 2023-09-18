QT += quick network websockets quickcontrols2 concurrent

CONFIG += c++17

windows {
    LIBS += -lKernel32

    # added nethost dependency
    LIBS += -L$$PWD/'../../../Program Files/dotnet/packs/Microsoft.NETCore.App.Host.win-x64/7.0.10/runtimes/win-x64/native/' -lnethost
    INCLUDEPATH += $$PWD/'../../../Program Files/dotnet/packs/Microsoft.NETCore.App.Host.win-x64/7.0.10/runtimes/win-x64/native'
    DEPENDPATH += $$PWD/'../../../Program Files/dotnet/packs/Microsoft.NETCore.App.Host.win-x64/7.0.10/runtimes/win-x64/native'
}


SOURCES += main.cpp \
    hosting/hostinghelpers.cpp

RESOURCES += qml.qrc

qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

HEADERS +=  \
    hosting/hostinghelpers.h


#LIBS += -LC:/Program Files/dotnet/packs/Microsoft.NETCore.App.Host.win-x64/7.0.10/runtimes/win-x64/native/ -lnethost

#INCLUDEPATH += $$PWD/'../../../Program Files/dotnet/packs/Microsoft.NETCore.App.Host.win-x64/7.0.10/runtimes/win-x64/native'
#DEPENDPATH += $$PWD/'../../../Program Files/dotnet/packs/Microsoft.NETCore.App.Host.win-x64/7.0.10/runtimes/win-x64/native'
