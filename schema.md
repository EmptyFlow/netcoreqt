# Specification for schema

Type of definitions:
* `version` - version of schema
* `globalObject` - define the fields of a global object (global object is key-value dictionary)
* `type` - define a type structure (can contains also `method` and `voidmethod` definition inside it)
* `event` - defining the structure of the event
* `instance` - define the structure of an instance object

## Example of schema
```
version 1.0

globalObject
int32 Test 1
int64 Lalalushka 2
double Bluherka 3
string Pirdesh 4

type MainClass 0 [inherit 10]
int32 Lapatash 1
int32 Lapatash 0
voidmethod RunVoidMethod 0 (int32, int32)
method RunReturnValueMethod 0 (int32, int32) int32

instance InstanceExample 0
int32 Lapatash 1

event TestEvent 0
int32 Test 1
```
