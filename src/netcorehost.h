#ifndef NETCOREHOST_H
#define NETCOREHOST_H

#include <QObject>

class NetCoreHost : public QObject
{
    Q_OBJECT
public:
    explicit NetCoreHost(QObject *parent = nullptr);

signals:

};

#endif // NETCOREHOST_H
